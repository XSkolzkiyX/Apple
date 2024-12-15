using System.Collections;
using UnityEngine;
using DG.Tweening;

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

    private void Start()
    {
        animator = GetComponent<Animator>();
        weaponRigidbody = GetComponent<Rigidbody>();
        weaponCollider = GetComponent<Collider>();
        weaponOutline = GetComponent<Outline>();
        mainCamera = Camera.main.transform;
        ammo = weaponData.ammo;
        ammoInMag = weaponData.ammoInMag;
        shootingSpread = weaponData.shootingSpread;
    }

    private void FixedUpdate()
    {
        if (isShooting || !player) return;
        if (!animator.GetBool("Aim"))
        {
            shootingSpread = Mathf.Clamp(shootingSpread - weaponData.shootingSpreadDecreaseValue, weaponData.shootingSpread, weaponData.maxShootingSpread);
            float crossHairSize = ((PlayerUI.resolution.y / 100.0f) * shootingSpread) * 2;
            player.playerUI.crossHair.sizeDelta = new Vector2(crossHairSize, crossHairSize);
        }
    }

    public void Shoot()
    {
        if (!isShooting || ammoInMag <= 0)
        {
            isShooting = false;
            return;
        }
        for (int i = 0; i < weaponData.bulletsPerShot; i++)
        {
            GameObject curBullet = Instantiate(weaponData.bulletPrefab, firePoint.position, firePoint.rotation);
            curBullet.transform.Rotate(Random.Range(-shootingSpread, shootingSpread), Random.Range(-shootingSpread, shootingSpread), 0);
            curBullet.GetComponent<Rigidbody>().velocity = curBullet.transform.forward * weaponData.bulletSpeed;
            curBullet.TryGetComponent(out BulletController bullet);
            bullet.damage = weaponData.damage;
            bullet.ownerTag = player.tag;
            Destroy(curBullet, 3);
        }
        ammoInMag--;
        player.playerUI.ammoText.text = $"{ammoInMag} / {ammo}";
        if (!animator.GetBool("Aim"))
        {
            shootingSpread = Mathf.Clamp(shootingSpread + weaponData.shootingSpreadIncreaseValue, weaponData.shootingSpread, weaponData.maxShootingSpread);
            float crossHairSize = ((PlayerUI.resolution.y / 100.0f) * shootingSpread) * 2;
            player.playerUI.crossHair.sizeDelta = new Vector2(crossHairSize, crossHairSize);
            //transform.DOShakeRotation(.1f, new Vector3(3, 0, 0));
        }
        else
        {
            mainCamera.Rotate(-weaponData.verticalSpray, 0, 0);
            mainCamera.parent.Rotate(0, (Random.Range(0, 2) == 0 ? -weaponData.horizontalSpray.leftDirection : weaponData.horizontalSpray.rightDirection), 0);
            //mainCamera.DORotate(new Vector3(mainCamera.eulerAngles.x - weaponData.verticalSpray, mainCamera.eulerAngles.y, 0), Time.deltaTime);
            //mainCamera.parent.DORotate(new Vector3(0, mainCamera.parent.eulerAngles.y + (Random.Range(0, 2) == 0 ? -weaponData.horizontalSpray.leftDirection : weaponData.horizontalSpray.rightDirection), 0), Time.deltaTime);
        }
        animator.SetTrigger("Shoot");
        if(weaponData.fireRate > 0) Invoke(nameof(Shoot), 60.0f / weaponData.fireRate);
    }

    public IEnumerator Reload()
    {
        if (ammoInMag >= weaponData.ammoInMag || ammo <= 0) yield break;
        animator.SetTrigger("Reload");
        animator.SetBool("Aim", false);
        isShooting = false;
        isReloading = true;
        yield return new WaitForSeconds(weaponData.reloadingDuration);
        int requiredAmmo = Mathf.Min(weaponData.ammoInMag - ammoInMag, ammo);
        ammoInMag += requiredAmmo;
        ammo -= requiredAmmo;
        player.playerUI.ammoText.text = $"{ammoInMag} / {ammo}";
        isReloading = false;
    }
}
