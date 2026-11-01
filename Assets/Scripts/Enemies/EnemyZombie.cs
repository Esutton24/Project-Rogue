using UnityEngine;

public class EnemyZombie : EnemyBase
{
    protected override void PerformAttack()
    {
        Vector3 origin = attackOrigin != null ? attackOrigin.position : transform.position;

        // Short-range sphere swipe
        Collider[] hits = Physics.OverlapSphere(origin, enemyData.attackRange * 0.6f);

        foreach (var c in hits)
        {
            if (c.CompareTag("Player"))
            {
                c.GetComponent<IDamageable>()?.TakeDamage(enemyData.damage);
                Debug.Log($"Zombie clawed player for {enemyData.damage}");
                return;
            }
        }
    }
}