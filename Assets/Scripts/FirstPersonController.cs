using DG.Tweening.Core.Easing;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Interaction
{
    public LayerMask interactionLayer;
    public LayerMask itemLayer;
    public Transform weaponPlace;
}

[System.Serializable]
public class PlayerStats
{
    [Header("General")]
    public float health;
    [Space(10)]

    [Header("Movement")]
    public float acceleration;
    public float walkingSpeed;
    public float runningSpeed;
    public float jumpForce;
    [Space(10)]

    [Header("Fall Damage Settings")]
    public float groundCheckDistance = 1.1f;
    public float safeFallDistance = 3f;
    public float fallDamageMultiplier = 10f;
}

[System.Serializable]
public class PlayerCamera
{
    [Header("General")]
    public float minCameraAngle;
    public float maxCameraAngle;
    public float sensitivity;
    [Space(10)]

    [Header("Lean")]
    public float leanAngle;
    public float leanSpeed;
    public Transform leanPoint;
    [Space(10)]

    [Header("Shake/Bob")]
    public float walkBobAmplitude = 0.05f;
    public float walkBobFrequency = 2.0f;
    public float idleBobAmplitude = 0.01f;
    public float idleBobFrequency = 0.5f;
    public float bobRecoverySpeed = 5.0f;
}

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class FirstPersonController : MonoBehaviour
{
    public float health;

    public PlayerStats playerStats;
    public PlayerCamera playerCamera;
    public InputSettings controls;

    private Rigidbody rb;
    private CapsuleCollider playerCollider;
    private Camera mainCamera;

    private bool isGrounded = false;
    private float currentSpeed;
    private bool isCrouching = false;

    private float originalHeight;
    private float crouchHeight;

    private float moveX, moveY, moveZ;
    private float rotationX, rotationY;

    private Vector3 initialCameraPosition;
    private float bobTime;
    private bool isMoving => rb.velocity.magnitude > 0.1f;

    private float lastGroundedHeight;
    private bool wasGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        mainCamera = Camera.main;
        initialCameraPosition = mainCamera.transform.localPosition;

        currentSpeed = playerStats.walkingSpeed;
        health = playerStats.health;

        originalHeight = playerCollider.height;
        crouchHeight = originalHeight / 2f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (GameManager.Instance.IsPaused) return;

        HandleMovementInput();
        HandleViewRotation();
        HandleCrouch();
    }

    private void FixedUpdate()
    {
        PerformMovement();
        CheckGroundState();
        HandleCameraBobbing();
    }

    private void HandleMovementInput()
    {
        moveX = Input.GetAxis("Horizontal") * currentSpeed * rb.mass;
        moveZ = Input.GetAxis("Vertical") * currentSpeed * rb.mass;

        if (Input.GetKey(controls.sprintKey) && !isCrouching)
        {
            currentSpeed = playerStats.runningSpeed;
        }
        else
        {
            currentSpeed = playerStats.walkingSpeed;
        }

        moveY = isGrounded && Input.GetButtonDown("Jump") ? playerStats.jumpForce : rb.velocity.y;
    }

    private void PerformMovement()
    {
        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;
        rb.AddForce(moveDirection * playerStats.acceleration, ForceMode.Force);

        rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -currentSpeed, currentSpeed), moveY, Mathf.Clamp(rb.velocity.z, -currentSpeed, currentSpeed));
    }

    private void HandleViewRotation()
    {
        rotationX = Input.GetAxis("Mouse X") * playerCamera.sensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * playerCamera.sensitivity;

        rotationY = Mathf.Clamp(rotationY, playerCamera.minCameraAngle, playerCamera.maxCameraAngle);

        transform.Rotate(0, rotationX, 0);
        mainCamera.transform.localEulerAngles = new Vector3(rotationY, 0, 0);

        float targetLeanAngle = playerCamera.leanAngle * Input.GetAxis("Lean");
        float currentLeanAngle = playerCamera.leanPoint.localEulerAngles.z;

        if (currentLeanAngle > 180) currentLeanAngle -= 360;

        float newLeanAngle = Mathf.Lerp(currentLeanAngle, targetLeanAngle, Time.deltaTime * playerCamera.leanSpeed);
        playerCamera.leanPoint.localEulerAngles = new Vector3(0, 0, newLeanAngle);
    }

    private void HandleCameraBobbing()
    {
        float bobAmplitude = isMoving ? playerCamera.walkBobAmplitude : playerCamera.idleBobAmplitude;
        float bobFrequency = isMoving ? playerCamera.walkBobFrequency : playerCamera.idleBobFrequency;

        if (isMoving)
        {
            bobTime += Time.deltaTime * bobFrequency;
        }
        else
        {
            bobTime = Mathf.Lerp(bobTime, 0, Time.deltaTime * playerCamera.bobRecoverySpeed);
        }

        float verticalOffset = Mathf.Sin(bobTime) * bobAmplitude;
        float horizontalOffset = Mathf.Cos(bobTime * 2) * bobAmplitude * 0.5f;

        mainCamera.transform.localPosition = new Vector3(initialCameraPosition.x + horizontalOffset,
                                               initialCameraPosition.y + verticalOffset,
                                               initialCameraPosition.z);
    }


    private void HandleCrouch()
    {
        if (Input.GetKey(controls.crouchKey) && !isCrouching)
        {
            isCrouching = true;
            playerCollider.height = crouchHeight;
            currentSpeed = playerStats.walkingSpeed / 2;
        }
        else if (isCrouching)
        {
            isCrouching = false;
            playerCollider.height = originalHeight;
            currentSpeed = playerStats.walkingSpeed;
        }
    }

    private void CheckGroundState()
    {
        Vector3[] rayOrigins = new Vector3[]
        {
        transform.position + Vector3.down + Vector3.left * 0.5f,
        transform.position + Vector3.down + Vector3.right * 0.5f,
        transform.position + Vector3.down + Vector3.forward * 0.5f,
        transform.position + Vector3.down + Vector3.back * 0.5f
        };

        RaycastHit[] hits = new RaycastHit[rayOrigins.Length];
        isGrounded = false;

        for (int i = 0; i < rayOrigins.Length; i++)
        {
            if (Physics.Raycast(rayOrigins[i], Vector3.down, out hits[i], playerStats.groundCheckDistance))
            {
                Debug.DrawLine(rayOrigins[i], hits[i].point, Color.green, 1f);
                isGrounded = true;
                break;
            }
        }

        if (isGrounded && !wasGrounded && lastGroundedHeight - transform.position.y > playerStats.safeFallDistance)
            TakeDamage((lastGroundedHeight - transform.position.y - playerStats.safeFallDistance) * playerStats.fallDamageMultiplier);

        if (!isGrounded && wasGrounded)
            lastGroundedHeight = transform.position.y;

        wasGrounded = isGrounded;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
