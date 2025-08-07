using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelController : MonoBehaviour
{
    [SerializeField] private float explosionRange;
    [SerializeField] private float explosionForce;
    [SerializeField] private float damage;
    [SerializeField] private GameObject explosionPrefab; 

    public IEnumerator Explode(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        Destroy(Instantiate(explosionPrefab, transform.position, Quaternion.identity), 3f);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRange);
        foreach(Collider collider in colliders)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (collider.TryGetComponent(out Rigidbody rb)) rb.AddExplosionForce(explosionForce, transform.position, explosionRange);
            if (collider.TryGetComponent(out FirstPersonController player)) player.TakeDamage(Mathf.Clamp01(1 - (distance / explosionRange)) * damage);
            else if (collider.TryGetComponent(out EnemyController enemy)) enemy.TakeDamage(Mathf.Clamp01(1 - (distance / explosionRange)) * damage);
        }
        Destroy(gameObject);
    }
}
