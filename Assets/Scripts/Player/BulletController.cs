using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float damage;
    [HideInInspector] public string ownerTag;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Bullet") || collider.CompareTag(ownerTag)) return;
        if (collider.TryGetComponent(out EnemyController enemy)) enemy.TakeDamage(damage);
        else if (collider.TryGetComponent(out FirstPersonController player)) player.TakeDamage(damage);
        else if(collider.TryGetComponent(out BarrelController barrel)) barrel.StartCoroutine(barrel.Explode());
        Destroy(gameObject);
    }
}
