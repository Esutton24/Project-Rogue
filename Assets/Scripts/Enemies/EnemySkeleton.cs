using UnityEngine;

public class EnemySkeleton : EnemyBase
{
    [Header("Projectile")]
    [SerializeField] private GameObject boneProjectilePrefab;

    protected override void PerformAttack()
    {
        if (boneProjectilePrefab == null || attackOrigin == null) return;

        Vector3 spawnPos = attackOrigin.position + transform.forward * 0.6f;
        Vector3 aimDir = (target.position + Vector3.up * 0.8f - spawnPos).normalized;
        Quaternion rot = Quaternion.LookRotation(aimDir);

        GameObject proj = Instantiate(boneProjectilePrefab, spawnPos, rot);
        if (proj.TryGetComponent<Projectile>(out var p))
        {
            p.Damage = enemyData.damage;
            if (TryGetComponent<Collider>(out var col))
                p.OwnerCollider = col;
        }
    }
}