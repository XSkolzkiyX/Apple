using UnityEngine;

public class KnifeController : ItemContainer
{
    [SerializeField] private float damage;
    [SerializeField] private float attackRange;
    [SerializeField] private Transform attackPoint;

    public override void UseItem()
    {
        base.UseItem();
    }

    public void DealDamage()
    {
        Collider[] colliders = Physics.OverlapSphere(attackPoint.position, attackRange);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Resource resource)) resource.TakeDamage(damage);
        }
    }
}
