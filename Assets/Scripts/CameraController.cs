using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float resetSpeed = 2f;
    public float movementBobFrequency = 10f;
    public float movementBobAmplitude = 0.05f;
    public float idleBobFrequency = 2f;
    public float idleBobAmplitude = 0.01f;

    private Vector3 initialCameraPosition;
    private float verticalRotation;
    private float horizontalRotation;
    private float idleBobTimer;
    private float movementBobTimer;

    public bool IsMoving { get; set; }
    public bool IsGrounded { get; set; }

    private void Start()
    {
        initialCameraPosition = transform.localPosition;
    }

    private void Update()
    {
        HandleCameraRotation();
        HandleCameraBobbing();
    }

    private void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        verticalRotation = Mathf.Lerp(verticalRotation, 0, Time.deltaTime * resetSpeed);
        horizontalRotation = Mathf.Lerp(horizontalRotation, 0, Time.deltaTime * resetSpeed);

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            verticalRotation += mouseY * rotationSpeed;
            horizontalRotation += mouseX * rotationSpeed;
        }

        verticalRotation = Mathf.Clamp(verticalRotation, -15f, 15f);
        horizontalRotation = Mathf.Clamp(horizontalRotation, -15f, 15f);

        transform.localRotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
    }

    private void HandleCameraBobbing()
    {
        if (IsMoving && IsGrounded)
        {
            movementBobTimer += Time.deltaTime * movementBobFrequency;
            transform.localPosition = initialCameraPosition +
                                            new Vector3(0, Mathf.Sin(movementBobTimer) * movementBobAmplitude, 0);
        }
        else
        {
            idleBobTimer += Time.deltaTime * idleBobFrequency;
            transform.localPosition = initialCameraPosition +
                                            new Vector3(0, Mathf.Sin(idleBobTimer) * idleBobAmplitude, 0);
        }
    }
}
