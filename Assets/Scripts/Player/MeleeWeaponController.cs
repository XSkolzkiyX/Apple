using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponController : Weapon
{
    public float damage;
    public float attackRange;

    public void Attack()
    {
        animator.SetTrigger("Attack");
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out EnemyController enemy)) enemy.TakeDamage(damage);
        }
    }
}
