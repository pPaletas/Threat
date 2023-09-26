using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    [SerializeField] private float _scaleSpeed = 50f;
    [SerializeField] private float _alhpaSpeed = 0.01f;
    private bool _exploded = false;

    private Material _mat;

    public void Explode()
    {
        Color cColor = _mat.color;
        cColor.a = 1f;

        _mat.color = cColor;

        _exploded = true;
    }

    private void Update()
    {
        if (!_exploded)
        {
            Vector3 currentScale = transform.localScale;
            currentScale.x = Mathf.Clamp(currentScale.x - _scaleSpeed * 0.01f * Time.deltaTime, 0f, 100000f);
            currentScale.z = Mathf.Clamp(currentScale.z - _scaleSpeed * 0.01f * Time.deltaTime, 0f, 100000f);

            transform.localScale = currentScale;

            Color currentColor = _mat.color;
            currentColor.a -= _alhpaSpeed * Time.deltaTime * 0.1f;
            currentColor.a = Mathf.Clamp01(currentColor.a);

            _mat.color = currentColor;
        }

        if (_exploded)
        {
            Vector3 currentScale = transform.localScale;
            currentScale.x += _scaleSpeed * Time.deltaTime;
            currentScale.z += _scaleSpeed * Time.deltaTime;

            transform.localScale = currentScale;

            Color currentColor = _mat.color;
            currentColor.a -= _alhpaSpeed * Time.deltaTime;
            currentColor.a = Mathf.Clamp01(currentColor.a);

            _mat.color = currentColor;

            if (currentColor.a <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Awake()
    {
        _mat = GetComponent<MeshRenderer>().material;
    }
}