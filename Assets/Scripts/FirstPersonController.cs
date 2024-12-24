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
    public float health;
    public float acceleration;
    public float walkingSpeed;
    public float runningSpeed;
    public float jumpForce;
    public float interactionDistance;
    public float throwingForce;
}

[System.Serializable]
public class PlayerCamera
{
    public float minCameraAngle;
    public float maxCameraAngle;
    public float sensitivity;
}

[System.Serializable]
public class PlayerUI
{
    public static Vector2 resolution = new Vector2(1920, 1080);
    public RectTransform crossHair;
    public RectTransform alternateCrossHair;
    public TextMeshProUGUI ammoText;
}

[System.Serializable]
public class Controls
{
    public KeyCode sprintKey;
    public KeyCode interactionKey;
    public KeyCode reloadingKey;
    public KeyCode dropKey;
    public KeyCode hackKey;
}

public class FirstPersonController : MonoBehaviour
{
    public float health;

    public WeaponController curWeapon;
    public Interaction interaction;
    public PlayerStats playerStats;
    public PlayerCamera playerCamera;
    public PlayerUI playerUI;
    public Controls controls;

    private bool isGrounded = false;
    private float speed;
    private Camera mainCamera;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public GameObject interactionObject;

    float moveX, moveY, moveZ;
    float rotationX, rotationY;

    private void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        speed = playerStats.walkingSpeed;
        health = playerStats.health;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (curWeapon)
        {
            curWeapon.player = this;
            playerUI.ammoText.text = $"{curWeapon.weaponData.ammoInMag} / {curWeapon.weaponData.ammo}";
            playerUI.ammoText.color = Color.white;
            playerUI.crossHair.gameObject.SetActive(true);
            playerUI.alternateCrossHair.gameObject.SetActive(true);
        }
        else
        {
            playerUI.ammoText.text = "...";
            playerUI.ammoText.color = Color.gray;
            playerUI.crossHair.gameObject.SetActive(false);
            playerUI.alternateCrossHair.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(controls.sprintKey))
        {
            speed = playerStats.runningSpeed;
        }
        else if(Input.GetKeyUp(controls.sprintKey))
        {
            speed = playerStats.walkingSpeed;
        }

        moveX = Input.GetAxis("Horizontal") * playerStats.acceleration * rb.mass;
        moveY = Input.GetAxis("Jump") * playerStats.jumpForce;
        moveY = isGrounded && moveY > 0 ? moveY : rb.velocity.y;
        moveZ = Input.GetAxis("Vertical") * playerStats.acceleration * rb.mass;

        //Pick Up Weapon

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward,
            out RaycastHit hit, playerStats.interactionDistance, interaction.interactionLayer))
        {
            Debug.DrawLine(mainCamera.transform.position, hit.point, Color.red, 1f);
            if (!hit.transform.gameObject.Equals(interactionObject))
            {
                if (interactionObject) interactionObject.GetComponent<Outline>().enabled = false;
                interactionObject = hit.transform.gameObject;
                interactionObject.GetComponent<Outline>().enabled = true;
            }
        }
        else if (interactionObject)
        {
            interactionObject.GetComponent<Outline>().enabled = false;
            interactionObject = null;
        }

        if (Input.GetKeyDown(controls.interactionKey)) Interact();
        if (Input.GetKeyDown(controls.dropKey)) DropWeapon();

        //Weapon
        if (!curWeapon) return;
        if(Input.GetMouseButtonDown(1))
        {
            curWeapon.animator.SetBool("Aim", true);
            playerUI.crossHair.gameObject.SetActive(false);
            playerUI.alternateCrossHair.gameObject.SetActive(false);
            curWeapon.shootingSpread = curWeapon.weaponData.aimShootingSpread;
        }
        else if(Input.GetMouseButtonUp(1))
        {
            curWeapon.animator.SetBool("Aim", false);
            playerUI.crossHair.gameObject.SetActive(true);
            playerUI.alternateCrossHair.gameObject.SetActive(true);
            curWeapon.shootingSpread = curWeapon.weaponData.shootingSpread;
        }

        if(Input.GetMouseButtonDown(0) && !curWeapon.isShooting && !curWeapon.isReloading)
        {
            curWeapon.isShooting = true;
            curWeapon.Shoot();
        }
        else if(Input.GetMouseButtonUp(0) && curWeapon.isShooting)
        {
            curWeapon.isShooting = false;
        }

        if(Input.GetKeyDown(controls.reloadingKey))
        {
            curWeapon.StartCoroutine(curWeapon.Reload());
        }
    }

    private void FixedUpdate()
    {
        //Movement
        rb.AddForce(transform.right * moveX + transform.forward * moveZ);

        //Jump and Limits
        rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -speed, speed), moveY, Mathf.Clamp(rb.velocity.z, -speed, speed));

        rotationX = Input.GetAxis("Mouse X") * playerCamera.sensitivity * Time.timeScale;
        rotationY = mainCamera.transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * playerCamera.sensitivity * Time.timeScale;

        transform.Rotate(0, rotationX, 0);

        if (rotationY > 180) rotationY -= 360;
        rotationY = Mathf.Clamp(rotationY, playerCamera.minCameraAngle, playerCamera.maxCameraAngle);
        if (rotationY < 0) rotationY += 360;

        mainCamera.transform.localEulerAngles = new Vector3(rotationY, 0, mainCamera.transform.localEulerAngles.z);
    }

    private void Interact()
    {
        if (!interactionObject) return;
        if (interactionObject.TryGetComponent(out WeaponController weapon)) PickUpWeapon(weapon);
        else if (interactionObject.TryGetComponent(out DoorController door)) door.ChangeDoorState();
        else if (interactionObject.TryGetComponent(out ButtonController button)) button.PressButton();
        else if (interactionObject.TryGetComponent(out BarrelController barrel)) barrel.StartCoroutine(barrel.Explode(3));
    }

    private void PickUpWeapon(WeaponController weapon)
    {
        if (!interactionObject) return;
        if (curWeapon) DropWeapon();
        curWeapon = weapon;
        curWeapon.player = this;
        curWeapon.transform.SetParent(interaction.weaponPlace);
        curWeapon.animator.enabled = true;
        //curWeapon.transform.position = weapon.weaponPlace.position;
        //curWeapon.transform.rotation = weapon.weaponPlace.rotation;
        curWeapon.weaponRigidbody.isKinematic = true;
        curWeapon.weaponCollider.enabled = false;
        curWeapon.weaponOutline.enabled = false;
        interactionObject.layer = interaction.itemLayer;
        interactionObject = null;

        playerUI.ammoText.text = $"{curWeapon.ammoInMag} / {curWeapon.ammo}";
        playerUI.ammoText.color = Color.white;
        playerUI.crossHair.gameObject.SetActive(true);
        playerUI.alternateCrossHair.gameObject.SetActive(true);
    }

    private void DropWeapon()
    {
        if (!curWeapon) return;
        curWeapon.animator.enabled = false;
        curWeapon.transform.SetParent(null);
        curWeapon.weaponRigidbody.isKinematic = false;
        curWeapon.weaponCollider.enabled = true;
        curWeapon.player = null;
        curWeapon.weaponRigidbody.velocity = mainCamera.transform.forward * playerStats.throwingForce;
        curWeapon.gameObject.layer = interaction.interactionLayer;
        curWeapon.animator.SetBool("Aim", false);
        curWeapon.isReloading = false;
        curWeapon = null;
        playerUI.crossHair.gameObject.SetActive(false);
        playerUI.alternateCrossHair.gameObject.SetActive(true);
        playerUI.ammoText.text = "...";
        playerUI.ammoText.color = Color.gray;
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
        Debug.Log("Player is DEAD");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!isGrounded)
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}
