using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float health;
    
    public EnemyData enemyData;

    public WeaponData curWeapon;
    public int ammoInMag;
    public int ammo;

    [HideInInspector] public bool isShooting = false;
    [HideInInspector] public bool isReloading = false;

    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform weaponModel;
    [SerializeField] private float throwingForce;

    [SerializeField] private LayerMask visionMask;

    private Animator animator;
    private NavMeshAgent agent;
    private Collider mainCollider;
    private List<Collider> ragdollColliders = new List<Collider>();
    private Transform player;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        mainCollider = GetComponent<Collider>();
        player = FindFirstObjectByType<FirstPersonController>().transform;
        health = enemyData.health;
        agent.speed = enemyData.walkingSpeed;
        ammo = curWeapon.ammo;
        ammoInMag = curWeapon.ammoInMag;
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach(Collider collider in colliders)
        {
            if(collider != mainCollider)
            {
                ragdollColliders.Add(collider);
            }
        }
        TurnOffRagdoll();
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(weaponModel.position, (player.position - weaponModel.position).normalized,
            out RaycastHit hit, enemyData.attackDistance, visionMask) && player.transform.Equals(hit.transform))
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < enemyData.meleeDistance)
            {
                GoToTarget(transform.position + Vector3.Normalize(transform.position - player.position) * enemyData.attackDistance);
                if (isShooting)
                {
                    isShooting = false;
                    if (ammoInMag < curWeapon.ammoInMag) StartCoroutine(Reload());
                }
            }
            else if (distance < enemyData.attackDistance)
            {
                if (!agent.isStopped)
                {
                    agent.isStopped = true;
                    animator.SetFloat("Velocity", 0);
                }
                Rotate(player.position);
                if (!isShooting)
                {
                    isShooting = true;
                    Shoot();
                }
            }
            return;
        }
        if(isShooting)
        {
            isShooting = false;
            if (ammoInMag < curWeapon.ammoInMag) StartCoroutine(Reload());
        }
        GoToTarget(player.position);
    }

    private void Rotate(Vector3 target)
    {
        target -= transform.position;
        target.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target), enemyData.rotationSpeed * Time.deltaTime);
    }

    private void GoToTarget(Vector3 targetPosition)
    {
        agent.speed = enemyData.runningSpeed;
        if (agent.isStopped) agent.isStopped = false;
        animator.SetFloat("Velocity", agent.speed);
        agent.SetDestination(targetPosition);
    }

    public void TurnOnRagdoll()
    {
        foreach(Collider collider in ragdollColliders)
        {
            collider.enabled = true;
            collider.GetComponent<Rigidbody>().isKinematic = false;
        }
        mainCollider.enabled = false;
        animator.enabled = false;
    }

    public void TurnOffRagdoll()
    {
        foreach (Collider collider in ragdollColliders)
        {
            collider.enabled = false;
            collider.GetComponent<Rigidbody>().isKinematic = true;
        }
        mainCollider.enabled = true;
        animator.enabled = true;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log(gameObject.name + " is DEAD");
        Instantiate(curWeapon.weaponModel, weaponModel.position, weaponModel.rotation).GetComponent<Rigidbody>().velocity = weaponModel.forward * throwingForce;
        weaponModel.gameObject.SetActive(false);
        TurnOnRagdoll();
        Destroy(this);
    }

    public void Shoot()
    {
        if (!isShooting || ammoInMag <= 0)
        {
            isShooting = false;
            return;
        }
        for (int i = 0; i < curWeapon.bulletsPerShot; i++)
        {
            GameObject curBullet = Instantiate(curWeapon.bulletPrefab, firePoint.position, firePoint.rotation);
            curBullet.transform.Rotate(Random.Range(-curWeapon.aimShootingSpread, curWeapon.aimShootingSpread), Random.Range(-curWeapon.aimShootingSpread, curWeapon.aimShootingSpread), 0);
            curBullet.GetComponent<Rigidbody>().velocity = curBullet.transform.forward * curWeapon.bulletSpeed;
            curBullet.TryGetComponent(out BulletController bullet);
            bullet.damage = curWeapon.damage;
            bullet.ownerTag = transform.tag;
            Destroy(curBullet, 3);
        }
        ammoInMag--;
        if (ammoInMag <= 0) StartCoroutine(Reload());
        if (curWeapon.fireRate > 0) Invoke(nameof(Shoot), 60.0f / curWeapon.fireRate);
    }

    public IEnumerator Reload()
    {
        if (ammoInMag >= curWeapon.ammoInMag || ammo <= 0) yield break;
        animator.SetTrigger("Reload");
        isShooting = false;
        isReloading = true;
        yield return new WaitForSeconds(curWeapon.reloadingDuration);
        int requiredAmmo = Mathf.Min(curWeapon.ammoInMag - ammoInMag, ammo);
        ammoInMag += requiredAmmo;
        ammo -= requiredAmmo;
        isReloading = false;
    }
}
