using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Data")]
    [SerializeField] protected EnemyDataSO enemyData;

    [Header("References")]
    [SerializeField] protected Transform attackOrigin;

    protected Rigidbody rb;
    protected Transform target;
    protected float currentHealth;
    protected bool isDead;
    protected float lastAttackTime;

    public virtual void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"{enemyData.enemyName} took {amount} damage. HP: {currentHealth}");

        if (currentHealth <= 0f)
            Die();
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        currentHealth = enemyData.maxHealth;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            target = playerObj.transform;
        else
            Debug.LogWarning($"{name}: No GameObject tagged 'Player' found!");
    }

    protected virtual void FixedUpdate()
    {
        if (isDead || target == null) return;
        Chase();
    }

    protected virtual void Update()
    {
        if (isDead || target == null) return;
        AttackLogic();
    }

    protected virtual void Chase()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= enemyData.attackRange) return;

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, 10f * Time.fixedDeltaTime));

        rb.MovePosition(rb.position + direction * enemyData.moveSpeed * Time.fixedDeltaTime);
    }

    protected virtual void AttackLogic()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > enemyData.attackRange) return;
        if (Time.time < lastAttackTime + enemyData.attackCooldown) return;

        PerformAttack();
        lastAttackTime = Time.time;
    }

    /// <summary>
    /// Each enemy type implements its own attack here.
    /// </summary>
    protected abstract void PerformAttack();

    protected virtual void Die()
    {
        isDead = true;
        Debug.Log($"{enemyData.enemyName} died!");

        rb.linearVelocity = Vector3.zero;
        enabled = false;
        Destroy(gameObject, 1f);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (enemyData == null) return;
        Gizmos.color = enemyData.gizmoColor;
        Gizmos.DrawWireSphere(transform.position, enemyData.attackRange);
    }
}