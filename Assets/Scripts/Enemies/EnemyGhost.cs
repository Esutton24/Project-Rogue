using UnityEngine;

public class EnemyGhost : EnemyBase
{
    protected override void PerformAttack()
    {
        Vector3 origin = attackOrigin != null ? attackOrigin.position : transform.position;

        if (Physics.Raycast(origin, transform.forward, out RaycastHit hit, enemyData.attackRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<IDamageable>()?.TakeDamage(enemyData.damage);
                Debug.Log($"Ghost touched player for {enemyData.damage}");
            }
        }
    }
}