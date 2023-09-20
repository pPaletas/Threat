using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class VolumetricLight : MonoBehaviour
{
    [Range(4, 64), SerializeField] private int _resolution = 16;
    [Range(0.01f, 10f), SerializeField] private float _displacement = 1f;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private bool _straight = true;

    private Light _spotLight;
    private MeshFilter _meshFilter;
    private Mesh _mesh;
    private MeshRenderer _meshRenderer;

    private bool _active = true;

    public Light SpotLight { get => _spotLight; }

    public void SetColor(Color color)
    {
        _spotLight.color = color;
        _meshRenderer.material.color = color;
    }

    private Vector3 GetRotatedVector(Vector3 vector, float angle, Vector3 axis)
    {
        return Quaternion.AngleAxis(angle, axis) * vector;
    }

    private void DisplayVolumetricLight()
    {
        float currentAngle = 0f;
        float _angleStep = 360f / _resolution;

        float radius = Mathf.Cos((_spotLight.spotAngle * 0.5f) * Mathf.Deg2Rad) * _displacement;

        Vector3[] vertices = new Vector3[(_resolution * 2) + 2];
        Vector2[] uvs = new Vector2[vertices.Length];
        // 2 triangulos tienen 3 aristas
        int[] triangles = new int[_resolution * 6];

        // El indice actual
        int verticesIndex = 0;
        int triangleIndex = -6;
        int uvHorizontal = 0;

        for (int i = 0; i <= _resolution; i++)
        {
            SetUpPairOfVertices(vertices, uvs, triangles, radius, currentAngle, verticesIndex, triangleIndex, uvHorizontal);

            verticesIndex += 2;
            triangleIndex += 6;
            currentAngle -= _angleStep;
            uvHorizontal += 1 / _resolution;
        }

        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.uv = uvs;
    }

    private void SetUpPairOfVertices(Vector3[] vertices, Vector2[] uvs, int[] triangles, float radius, float currentAngle, int verticesIndex, int triangleIndex, int uvHorizontal)
    {
        // Si son los ultimos vertices, entonces los triangulos deberan finalizar con los primeros
        if (verticesIndex >= _resolution * 2)
        {
            triangles[triangleIndex] = verticesIndex - 2;
            triangles[triangleIndex + 1] = verticesIndex - 1;
            triangles[triangleIndex + 2] = 0;
            triangles[triangleIndex + 3] = 0;
            triangles[triangleIndex + 4] = verticesIndex - 1;
            triangles[triangleIndex + 5] = 1;
        }
        // Desde los primeros vertices establecidos, no podemos empezar a generar caras
        else
        {
            vertices[verticesIndex] = GetRotatedVector(Vector3.right, currentAngle, Vector3.forward) * radius;

            Vector3 unit = vertices[verticesIndex].normalized;

            // if (verticesIndex == 0) Debug.Log(unit);

            // unit = transform.TransformPoint(unit) - transform.position;

            Vector3 vertexDirection = Quaternion.AngleAxis(_spotLight.spotAngle * 0.5f, Vector3.Cross(Vector3.forward, unit)) * Vector3.forward;
            // Debug.DrawRay(transform.position, transform.forward, Color.blue);
            // Debug.DrawRay(transform.position, unit, Color.green);
            // Debug.DrawRay(transform.position + transform.forward * 0.5f, Quaternion.AngleAxis(_spotLight.spotAngle * 0.5f, Vector3.Cross(Vector3.forward, unit)) * Vector3.forward);

            if (_straight)
            {
                Vector3 noYNextVertex = vertexDirection;
                noYNextVertex = Quaternion.AngleAxis(transform.eulerAngles.x, -Vector3.right) * vertexDirection;

                vertices[verticesIndex] = (noYNextVertex * _displacement);
            }
            vertices[verticesIndex] = (vertexDirection * _displacement);

            float dist = _spotLight.range;

            Vector3 vertexWorldPos = transform.position + vertices[verticesIndex];
            Vector3 localNextVertexDirection = transform.rotation * vertexDirection;

            bool isColliding = Physics.Raycast(transform.position, localNextVertexDirection, out RaycastHit hit, _spotLight.range - _displacement, _groundMask);

            if (isColliding)
            {
                dist = hit.distance;
            };

            vertices[verticesIndex + 1] = vertexDirection * (dist);

            if (triangleIndex >= 0)
            {
                triangles[triangleIndex] = verticesIndex - 2;
                triangles[triangleIndex + 1] = verticesIndex - 1;
                triangles[triangleIndex + 2] = verticesIndex;
                triangles[triangleIndex + 3] = verticesIndex;
                triangles[triangleIndex + 4] = verticesIndex - 1;
                triangles[triangleIndex + 5] = verticesIndex + 1;
            }

            uvs[verticesIndex] = new Vector2(uvHorizontal, 0);
            uvs[verticesIndex + 1] = new Vector2(uvHorizontal, dist / _spotLight.range);
        }
    }

    private void Awake()
    {
        if (!gameObject.TryGetComponent<MeshFilter>(out _meshFilter)) _meshFilter = gameObject.AddComponent<MeshFilter>();

        if (!gameObject.TryGetComponent<MeshRenderer>(out _meshRenderer)) _meshRenderer = gameObject.AddComponent<MeshRenderer>();

        _spotLight = GetComponent<Light>();

        _mesh = new Mesh();
        _meshFilter.mesh = _mesh;

        _meshRenderer.material = Resources.Load<Material>("VolumetricLight");
        _meshRenderer.material.color = _spotLight.color;
        _meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    private void LateUpdate()
    {
        if (_active)
            DisplayVolumetricLight();
    }
}