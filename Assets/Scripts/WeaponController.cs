using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public WeaponData weaponData;
    public Transform firePoint;

    public int ammoInMag;
    public int ammo;

    [HideInInspector] public Animator animator;
    [HideInInspector] public Rigidbody weaponRigidbody;
    [HideInInspector] public Collider weaponCollider;
    [HideInInspector] public Outline weaponOutline;
    [HideInInspector] public bool isShooting = false;
    [HideInInspector] public bool isReloading = false;
    [HideInInspector] public float shootingSpread;
    [HideInInspector] public FirstPersonController player;

    private Transform mainCamera;
    private UIManager uiManager;

    private void Start()
    {
        animator = GetComponent<Animator>();
        weaponRigidbody = GetComponent<Rigidbody>();
        weaponCollider = GetComponent<Collider>();
        weaponOutline = GetComponent<Outline>();
        mainCamera = Camera.main.transform;
        uiManager = FindObjectOfType<UIManager>();

        ammo = weaponData.ammo;
        ammoInMag = weaponData.ammoInMag;
        shootingSpread = weaponData.shootingSpread;

        uiManager.UpdateAmmoText(ammoInMag, ammo);
    }

    private void FixedUpdate()
    {
        if (isShooting || !player) return;

        if (!animator.GetBool("Aim"))
        {
            shootingSpread = Mathf.Clamp(shootingSpread - weaponData.shootingSpreadDecreaseValue, weaponData.shootingSpread, weaponData.maxShootingSpread);
            uiManager.UpdateCrossHairSize(shootingSpread);
        }
    }

    public void Shoot()
    {
        if (!isShooting || ammoInMag <= 0 || Time.timeScale <= 0)
        {
            isShooting = false;
            return;
        }

        for (int i = 0; i < weaponData.bulletsPerShot; i++)
        {
            GameObject curBullet = Instantiate(weaponData.bulletPrefab, firePoint.position, firePoint.rotation);
            curBullet.transform.Rotate(Random.Range(-shootingSpread, shootingSpread), Random.Range(-shootingSpread, shootingSpread), 0);
            curBullet.GetComponent<Rigidbody>().velocity = curBullet.transform.forward * weaponData.bulletSpeed;

            if (curBullet.TryGetComponent(out BulletController bullet))
            {
                bullet.damage = weaponData.damage;
                bullet.ownerTag = player.tag;
            }

            Destroy(curBullet, 3);
        }

        ammoInMag--;
        uiManager.UpdateAmmoText(ammoInMag, ammo);

        if (!animator.GetBool("Aim"))
        {
            shootingSpread = Mathf.Clamp(shootingSpread + weaponData.shootingSpreadIncreaseValue, weaponData.shootingSpread, weaponData.maxShootingSpread);
            uiManager.UpdateCrossHairSize(shootingSpread);
        }
        else
        {
            shootingSpread = weaponData.aimShootingSpread;
            mainCamera.Rotate(-weaponData.verticalSpray, 0, 0);
            mainCamera.parent.Rotate(0, (Random.Range(0, 2) == 0 ? -weaponData.horizontalSpray.leftDirection : weaponData.horizontalSpray.rightDirection), 0);
        }

        animator.SetTrigger("Shoot");

        if (weaponData.fireRate > 0) Invoke(nameof(Shoot), 60.0f / weaponData.fireRate);
    }

    public void Reload()
    {
        if (ammoInMag >= weaponData.ammoInMag || ammo <= 0) return;

        animator.SetTrigger("Reload");
        animator.SetBool("Aim", false);
        isShooting = false;
        isReloading = true;
     }

    private void CompleteReloading()
    {
        int requiredAmmo = Mathf.Min(weaponData.ammoInMag - ammoInMag, ammo);
        ammoInMag += requiredAmmo;
        ammo -= requiredAmmo;

        uiManager.UpdateAmmoText(ammoInMag, ammo);
        isReloading = false;
    }
}
