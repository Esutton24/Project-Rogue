using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private PlayerClassManager classManager;
    [SerializeField] private PlayerAim playerAim;
    [SerializeField] private Transform attackOrigin;

    private PlayerInputActions inputActions;
    private float[] cooldownTimers;
    private bool[] attackReady;

    void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    void Start()
    {
        if (classManager != null)
            classManager.OnClassChanged += OnClassChanged;
        InitializeCooldowns();
    }

    void OnEnable()
    {
        inputActions.Player.Attack.performed += OnAttack;
        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Attack.performed -= OnAttack;
        inputActions.Player.Disable();
    }

    void Update()
    {
        TickCooldowns();
    }

    void OnClassChanged(PlayerClassSO newClass)
    {
        InitializeCooldowns();
    }

    void InitializeCooldowns()
    {
        if (classManager?.CurrentClass == null) return;

        int count = classManager.CurrentClass.attacks.Count;
        cooldownTimers = new float[count];
        attackReady = new bool[count];

        for (int i = 0; i < count; i++)
            attackReady[i] = true;
    }

    void TickCooldowns()
    {
        if (cooldownTimers == null) return;

        for (int i = 0; i < cooldownTimers.Length; i++)
        {
            if (!attackReady[i])
            {
                cooldownTimers[i] -= Time.deltaTime;
                if (cooldownTimers[i] <= 0f)
                    attackReady[i] = true;
            }
        }
    }

    void OnAttack(InputAction.CallbackContext context)
    {
        TryUseAttack(0);
    }

    void TryUseAttack(int index)
    {
        var cls = classManager?.CurrentClass;
        if (cls == null || index >= cls.attacks.Count) return;
        if (!attackReady[index]) return;

        AttackDataSO attack = cls.attacks[index];
        ExecuteAttack(attack);

        attackReady[index] = false;
        cooldownTimers[index] = attack.cooldown;
    }

    void ExecuteAttack(AttackDataSO attack)
    {
        switch (attack.attackType)
        {
            case AttackType.Melee:
                PerformMelee(attack);
                break;
            case AttackType.Ranged:
                PerformRanged(attack);
                break;
            case AttackType.Area:
                PerformArea(attack);
                break;
        }

        if (attack.attackEffect != null)
            Instantiate(attack.attackEffect, attackOrigin.position, attackOrigin.rotation);

        if (attack.attackSound != null)
            AudioSource.PlayClipAtPoint(attack.attackSound, transform.position);
    }

    void PerformMelee(AttackDataSO attack)
    {
        if (playerAim == null) return;

        Vector3 origin = attackOrigin.position;
        Vector3 aimDir = (playerAim.AimPoint - origin).normalized;

        if (Physics.Raycast(origin, aimDir, out RaycastHit hit, attack.range))
        {
            hit.collider.GetComponent<IDamageable>()?.TakeDamage(attack.damage);
            Debug.Log($"Melee hit {hit.collider.name} for {attack.damage}");
        }
    }

    void PerformRanged(AttackDataSO attack)
    {
        if (attack.projectilePrefab == null || playerAim == null) return;

        // Offset spawn along the aim direction so it doesn't clip the player
        Vector3 aimDir = (playerAim.AimPoint - attackOrigin.position).normalized;
        Vector3 spawnPos = attackOrigin.position + aimDir * 0.6f;
        Quaternion aimRot = playerAim.GetAimRotation(spawnPos);

        GameObject proj = Instantiate(attack.projectilePrefab, spawnPos, aimRot);

        if (proj.TryGetComponent<Projectile>(out var p))
        {
            p.Damage = attack.damage;
            if (TryGetComponent<Collider>(out var col))
                p.OwnerCollider = col;
        }
    }

    void PerformArea(AttackDataSO attack)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attack.range);
        foreach (var c in hits)
        {
            c.GetComponent<IDamageable>()?.TakeDamage(attack.damage);
        }
    }

    void OnDestroy()
    {
        inputActions?.Dispose();
        if (classManager != null)
            classManager.OnClassChanged -= OnClassChanged;
    }
}