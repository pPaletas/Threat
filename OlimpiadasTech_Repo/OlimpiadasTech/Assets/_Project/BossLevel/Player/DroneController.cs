using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DroneController : MonoBehaviour
{
    [Range(-1, 1)]
    public float Thrust, Tilt, Lift, Rotate, RotateX;

    [Space(10)]
    public Rigidbody rb;
    public Animator anim;
    public Transform camTransform;
    public Transform body;

    [Space(10)]
    public float lift = 5;
    public float speed = 5;

    public float rotationSpeed = 5;
    public float rotationSmoothness = 5;
    public float xMaxRotation = 45;
    public float maxBodyRotation = 45;

    [Space(10)]
    public float angle;
    public float angleX;
    public float angleXSmoothness = 5f;

    private float currentAngleX = 0f;

    private Quaternion rotation;
    private Quaternion rotationX;
    private Quaternion rotationZ;

    private CharacterInput _input;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        _input = GetComponent<CharacterInput>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Rotate = _input.look.x;
        RotateX = _input.look.y;
        Thrust = _input.move.y;
        Tilt = _input.move.x;
    }

    private void FixedUpdate()
    {
        angle += _input.look.x * rotationSpeed;
        angleX -= _input.look.y * rotationSpeed;

        angleX = Mathf.Clamp(angleX, -xMaxRotation, xMaxRotation);
        currentAngleX = Mathf.Lerp(currentAngleX, angleX, Time.deltaTime * angleXSmoothness);

        if (angle >= 360)
            angle = 0;
        else if (angle <= -360)
            angle = 0;

        rotation = Quaternion.AngleAxis(angle, Vector3.up);
        rotationX = Quaternion.AngleAxis(currentAngleX, Vector3.left);

        Quaternion targetRot = rotation * rotationX;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSmoothness * Time.deltaTime);

        float bodyRot = -(targetRot.eulerAngles - transform.eulerAngles).y;
        Quaternion targetBodyRot = Quaternion.AngleAxis(bodyRot, Vector3.forward) * Quaternion.identity;

        body.localRotation = Quaternion.Slerp(body.localRotation, targetBodyRot, rotationSmoothness * Time.deltaTime);

        var fDir = camTransform.forward * Thrust * speed;
        var rDir = camTransform.right * Tilt * speed;

        Debug.DrawRay(transform.position, (fDir + rDir) * 100f);

        rb.AddForce(fDir + rDir, ForceMode.VelocityChange);

        var cv3 = rb.velocity;

        // cv3.x = Mathf.Clamp(cv3.x, -speed, speed);
        // cv3.y = Mathf.Clamp(cv3.y, -speed, speed);
        // cv3.z = Mathf.Clamp(cv3.z, -speed, speed);

        cv3 = Vector3.ClampMagnitude(cv3, speed);

        rb.velocity = cv3;
    }
}
