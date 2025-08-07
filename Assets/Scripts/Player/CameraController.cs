using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float minCameraAngle = -80;
    public float maxCameraAngle = 80;
    public float sensitivity = 1;

    float rotationX, rotationY;
    private Camera mainCamera;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mainCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        rotationX = Input.GetAxis("Mouse X") * sensitivity * Time.timeScale;
        rotationY = mainCamera.transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * sensitivity * Time.timeScale;

        transform.Rotate(0, rotationX, 0);

        if (rotationY > 180) rotationY -= 360;
        rotationY = Mathf.Clamp(rotationY, minCameraAngle, maxCameraAngle);
        if (rotationY < 0) rotationY += 360;

        mainCamera.transform.localEulerAngles = new Vector3(rotationY, 0, mainCamera.transform.localEulerAngles.z);

    }
}
