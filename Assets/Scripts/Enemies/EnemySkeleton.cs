using UnityEngine;

public class EnemySkeleton : EnemyBase
{
    [Header("Projectile")]
    [SerializeField] private GameObject boneProjectilePrefab;

    protected override void PerformAttack()
    {
        if (boneProjectilePrefab == null || attackOrigin == null) return;

        Vector3 spawnPos = attackOrigin.position + transform.forward * 1.0f;
        Vector3 aimDir = (target.position + Vector3.up * 0.8f - spawnPos).normalized;
        Quaternion rot = Quaternion.LookRotation(aimDir);

        GameObject proj = Instantiate(boneProjectilePrefab, spawnPos, rot);
        if (proj.TryGetComponent<Projectile>(out var p))
        {
            p.Damage = enemyData.damage;
            p.OwnerRoot = transform; // pass the root transform, not a single collider
        }
    }
}