using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Wealth")]
    public int coins;
    public int apples;

    [Space(10)]

    [Header("Stats")]
    public float health;
    [SerializeField] private float saturation;
    [SerializeField] private float saturationSpending;
    [SerializeField] private float saturationSpendingDelay;
    [SerializeField] private float damageDelay;
    [SerializeField] private float thirst;
    [SerializeField] private float thirstSpending;
    [SerializeField] private float thirstSpendingDelay;
    public float acceleration;
    public float walkingSpeed;
    public float runningSpeed;
    public float jumpForce;
    public byte countOfJumps;

    [Space(10)]

    [Header("Camera")]
    [SerializeField] private float minCameraAngle;
    [SerializeField] private float maxCameraAngle;
    [SerializeField] private float sensitivity;
    private Camera mainCamera;

    [Space(10)]

    [Header("Controls")]
    public KeyCode sprintKey;
    public KeyCode consumeKey;
    public KeyCode interactionKey;

    private bool isGrounded = false;
    private bool inInteraction = false;
    private GameObject interactionObject;
    private float speed;
    private Rigidbody rb;

    [HideInInspector] public float maxHealth;
    [HideInInspector] public byte maxCountOfJumps;
    private float maxSaturation;
    private float maxThirst;
    private float lastTimeDamaged;
    private float lastTimeConsumed;
    private float lastTimeDrank;

    private void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        speed = walkingSpeed;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        maxHealth = health;
        maxSaturation = saturation;
        maxThirst = thirst;
        maxCountOfJumps = countOfJumps;
        lastTimeDamaged = Time.time;
        lastTimeConsumed = Time.time;
    }

    private void Update()
    {
        if (Input.GetKeyDown(sprintKey))
        {
            speed = runningSpeed;
        }
        else if(Input.GetKeyUp(sprintKey))
        {
            speed = walkingSpeed;
        }

        float moveX = Input.GetAxis("Horizontal") * acceleration;
        //float moveY = isGrounded ? (Input.GetAxis("Jump") * jumpForce) : rb.velocity.y;
        float moveZ = Input.GetAxis("Vertical") * acceleration;
        if(Input.GetKeyDown(KeyCode.Space) && countOfJumps > 0)
        {
            rb.velocity = rb.velocity + transform.up * jumpForce;
            countOfJumps--;
        }
        //Movement
        rb.AddForce(transform.right * moveX + transform.forward * moveZ, ForceMode.Acceleration);
        
        //Jump and Limits
        rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -speed, speed), rb.velocity.y , Mathf.Clamp(rb.velocity.z, -speed, speed));

        float rotationX = Input.GetAxis("Mouse X") * sensitivity * Time.timeScale;
        float rotationY = mainCamera.transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * sensitivity * Time.timeScale;

        transform.Rotate(0, rotationX, 0);

        if (rotationY > 180) rotationY -= 360;
        rotationY = Mathf.Clamp(rotationY, minCameraAngle, maxCameraAngle);
        if (rotationY < 0) rotationY += 360;

        mainCamera.transform.localEulerAngles = new Vector3(rotationY, 0, mainCamera.transform.localEulerAngles.z);

        //Interact
        if (Input.GetKeyDown(consumeKey)) Eat();
        if (Input.GetKeyDown(interactionKey)) Interact();
    }

    private void FixedUpdate()
    {
        if (Time.time > lastTimeConsumed + saturationSpendingDelay)
        {
            saturation -= Time.deltaTime * saturationSpending;
            if (saturation <= 0)
            {
                saturation = 0;
                if (Time.time > lastTimeDamaged + damageDelay) TakeDamage(10);
            }
        }
        if (Time.time > lastTimeDrank + thirstSpendingDelay)
        {
            thirst -= Time.deltaTime * thirstSpending;
            if (thirst <= 0)
            {
                thirst = 0;
                TakeDamage(Time.deltaTime, false);
            }
        }
    }

    public void Eat()
    {
        if (apples <= 0) return;
        saturation = Mathf.Clamp(saturation + 1, 0, maxSaturation);
        apples--;
        lastTimeConsumed = Time.time;
    }

    public void Drink()
    {
        thirst = maxThirst;
        lastTimeDrank = Time.time;
    }

    public void TakeDamage(float damage, bool timer = true)
    {
        health -= damage;
        if (health <= 0) Die();
        if(timer) lastTimeDamaged = Time.time;
    }

    private void Die()
    {
        Debug.Log("You died.");
        SceneManager.LoadScene(0);
    }

    private void Interact()
    {
        if (!interactionObject) return;
        if (interactionObject.TryGetComponent(out ShopController shop)) shop.Buy(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isGrounded)
        {
            isGrounded = true;
            countOfJumps = maxCountOfJumps;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        switch (collider.tag)
        {
            case "Apple":
                apples++;
                Destroy(collider.gameObject);
                break;
            case "Water":
                Drink();
                break;
            case "Shop":
                inInteraction = true;
                interactionObject = collider.gameObject;
                break;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if(inInteraction && collider.gameObject.Equals(interactionObject))
        {
            inInteraction = false;
            interactionObject = null;
        }
    }
}
