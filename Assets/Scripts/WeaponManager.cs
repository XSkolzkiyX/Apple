using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public InputSettings inputSettings;
    public Transform weaponHolder;
    public WeaponController equippedWeapon;

    private UIManager uiManager;

    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    private void Update()
    {
        if (GameManager.Instance.IsPaused || !equippedWeapon) return;

        HandleWeaponInput();
    }

    private void HandleWeaponInput()
    {
        if (Input.GetKeyDown(inputSettings.fireKey) && !equippedWeapon.isReloading && !equippedWeapon.isShooting)
        {
            equippedWeapon.isShooting = true;
            equippedWeapon.Shoot();
        }
        else if (Input.GetKeyUp(inputSettings.fireKey)) equippedWeapon.isShooting = false;

        if (Input.GetKeyDown(inputSettings.aimKey))
        {
            equippedWeapon.animator.SetBool("Aim", true);
            uiManager.SetCrossHairActive(false, equippedWeapon.ammoInMag, equippedWeapon.ammo);
        }
        else if (Input.GetKeyUp(inputSettings.aimKey))
        {
            equippedWeapon.animator.SetBool("Aim", false);
            uiManager.SetCrossHairActive(true, equippedWeapon.ammoInMag, equippedWeapon.ammo);
        }

        if (Input.GetKeyDown(inputSettings.reloadKey) && !equippedWeapon.isReloading) equippedWeapon.Reload();

        if (Input.GetKeyDown(inputSettings.dropKey)) DropWeapon();
    }

    public void EquipWeapon(WeaponController newWeapon)
    {
        if (equippedWeapon) DropWeapon();

        equippedWeapon = newWeapon;
        equippedWeapon.player = FindObjectOfType<FirstPersonController>();
        equippedWeapon.transform.SetParent(weaponHolder);
        equippedWeapon.transform.localPosition = Vector3.zero;
        equippedWeapon.transform.localRotation = Quaternion.identity;

        equippedWeapon.weaponRigidbody.isKinematic = true;
        equippedWeapon.weaponCollider.enabled = false;
        equippedWeapon.weaponOutline.enabled = false;
        equippedWeapon.animator.enabled = true;

        uiManager.SetCrossHairActive(true, equippedWeapon.ammoInMag, equippedWeapon.ammo);
    }

    public void DropWeapon()
    {
        if (!equippedWeapon) return;

        equippedWeapon.transform.SetParent(null);
        equippedWeapon.weaponRigidbody.isKinematic = false;
        equippedWeapon.weaponCollider.enabled = true;
        equippedWeapon.animator.enabled = false;

        equippedWeapon.player = null;
        equippedWeapon = null;

        uiManager.SetCrossHairActive(false);
    }
}
