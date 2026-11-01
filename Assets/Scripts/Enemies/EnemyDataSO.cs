using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy/Enemy Data")]
public class EnemyDataSO : ScriptableObject
{
    [Header("Identity")]
    public string enemyName = "Enemy";

    [Header("Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 3.5f;
    public float damage = 10f;

    [Header("Combat")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;

    [Header("Debug")]
    public Color gizmoColor = Color.red;
}