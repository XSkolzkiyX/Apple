using Photon.Pun.Demo.PunBasics;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
public class WeaponSlot
{
    public GameObject slot;
    public Image icon;
}

[System.Serializable]
public class PlayerUI
{
    public static Vector2 resolution = new Vector2(1920, 1080);
    public RectTransform crossHair;
    public RectTransform alternateCrossHair;
    public TextMeshProUGUI ammoText;

    public List<WeaponSlot> weaponSlots;
    public Image curWeaponImage;
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

    public List<WeaponController> weapons;
    public Interaction interaction;
    public PlayerStats playerStats;
    public PlayerCamera playerCamera;
    public PlayerUI playerUI;
    public InputSettings controls;
    [HideInInspector] public WeaponController curWeapon;
    private int curWeaponIndex = 0;

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
        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].gameObject.SetActive(false);
            playerUI.weaponSlots[i].icon.sprite = weapons[i].weaponData.weaponIcon;
        }

        if (weapons[curWeaponIndex])
        {
            curWeapon = weapons[curWeaponIndex];
            curWeapon.gameObject.SetActive(true);
            curWeapon.player = this;
            playerUI.ammoText.text = $"{curWeapon.weaponData.ammoInMag} / ...";
            playerUI.ammoText.color = Color.white;
            playerUI.crossHair.gameObject.SetActive(true);
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

        for (int i = 1; i <= 9; i++)
            if (Input.GetKeyDown(KeyCode.Alpha0 + i) && i - 1 < weapons.Count)
                SelectWeapon(i - 1);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            int newActiveSlotIndex = curWeaponIndex + (scroll > 0 ? -1 : 1);
            newActiveSlotIndex = (newActiveSlotIndex + weapons.Count) % weapons.Count;

            SelectWeapon(newActiveSlotIndex);
        }

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

        if (Input.GetKeyDown(controls.interactKey)) Interact();
        //if (Input.GetKeyDown(controls.dropKey)) DropWeapon();

        //Weapon
            
        //if(curMeleeWeapon)
        //{
        //    if(Input.GetKeyDown(controls.shootKey))
        //    {
        //        curMeleeWeapon.Attack();
        //    }
        //}
        //
        //if (!curWeapon) return;

        if (Input.GetKeyDown(controls.shootKey) && !curWeapon.isShooting && !curWeapon.isReloading)
        {
            curWeapon.isShooting = true;
            curWeapon.Shoot();
        }
        else if (Input.GetKeyUp(controls.shootKey) && curWeapon.isShooting)
        {
            curWeapon.isShooting = false;
        }

        if (Input.GetKeyDown(controls.aimKey))
        {
            curWeapon.animator.SetBool("Aim", true);
            playerUI.crossHair.gameObject.SetActive(false);
            playerUI.alternateCrossHair.gameObject.SetActive(false);
            curWeapon.shootingSpread = curWeapon.weaponData.aimShootingSpread;
        }
        else if(Input.GetKeyUp(controls.aimKey))
        {
            curWeapon.animator.SetBool("Aim", false);
            playerUI.crossHair.gameObject.SetActive(true);
            playerUI.alternateCrossHair.gameObject.SetActive(true);
            curWeapon.shootingSpread = curWeapon.weaponData.shootingSpread;
        }

        if(Input.GetKeyDown(controls.reloadKey))
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
        //if (interactionObject.TryGetComponent(out Weapon weapon)) PickUpWeapon(weapon);
        if (interactionObject.TryGetComponent(out DoorController door)) door.ChangeDoorState();
        else if (interactionObject.TryGetComponent(out ButtonController button)) button.PressButton();
        else if (interactionObject.TryGetComponent(out BarrelController barrel)) barrel.StartCoroutine(barrel.Explode(3));
    }

    private void SelectWeapon(int newIndex)
    {
        //if (!interactionObject) return;
        //if (curWeapon || curMeleeWeapon) DropWeapon();
        //if(weapon.TryGetComponent(out WeaponController weaponController)) curWeapon = weaponController;
        //if(weapon.TryGetComponent(out MeleeWeaponController meleeWeaponController)) curMeleeWeapon = meleeWeaponController;
        curWeapon.gameObject.SetActive(false);
        playerUI.weaponSlots[curWeaponIndex].slot.SetActive(true);
        curWeaponIndex = newIndex;
        curWeapon = weapons[curWeaponIndex];
        curWeapon.gameObject.SetActive(true);
        curWeapon.player = this;
        playerUI.weaponSlots[curWeaponIndex].slot.SetActive(false);
        playerUI.curWeaponImage.sprite = curWeapon.weaponData.weaponIcon;
        //SetWeaponValues(curWeapon);
        if (curWeapon)
        {
            playerUI.ammoText.text = $"{curWeapon.ammoInMag} / ...";
            playerUI.ammoText.color = Color.white;
            playerUI.crossHair.gameObject.SetActive(true);
            playerUI.alternateCrossHair.gameObject.SetActive(true);
        }
        //else
        //{
        //    playerUI.ammoText.text = $"...";
        //    playerUI.ammoText.color = Color.gray;
        //    playerUI.crossHair.gameObject.SetActive(false);
        //    playerUI.alternateCrossHair.gameObject.SetActive(true);
        //}
    }

    private void SetWeaponValues(Weapon weapon)
    {
        weapon.player = this;
        weapon.transform.SetParent(interaction.weaponPlace);
        weapon.animator.enabled = true;
        weapon.weaponRigidbody.isKinematic = true;
        weapon.weaponCollider.enabled = false;
        weapon.weaponOutline.enabled = false;
    }

    //private void ResetWeaponValue(Weapon weapon)
    //{
    //    weapon.animator.enabled = false;
    //    weapon.transform.SetParent(null);
    //    weapon.weaponRigidbody.isKinematic = false;
    //    weapon.weaponCollider.enabled = true;
    //    weapon.player = null;
    //    weapon.weaponRigidbody.velocity = mainCamera.transform.forward * playerStats.throwingForce;
    //}

    //private void DropWeapon()
    //{
    //    if (curWeapon)
    //    {
    //        ResetWeaponValue(curWeapon);
    //        curWeapon.animator.SetBool("Aim", false);
    //        curWeapon.isReloading = false;
    //        curWeapon = null;
    //    }
    //    if (curMeleeWeapon)
    //    {
    //        ResetWeaponValue(curMeleeWeapon);
    //        curMeleeWeapon = null;
    //    }
    //    playerUI.crossHair.gameObject.SetActive(false);
    //    playerUI.alternateCrossHair.gameObject.SetActive(true);
    //    playerUI.ammoText.text = "...";
    //    playerUI.ammoText.color = Color.gray;
    //}

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
