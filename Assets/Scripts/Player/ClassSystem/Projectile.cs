using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 3f;

    public float Damage { get; set; } = 10f;
    public Collider OwnerCollider { get; set; }

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Force the collider to be a trigger so we detect enemies reliably
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        // Velocity-driven physics (much more reliable than Transform.Translate)
        rb.linearVelocity = transform.forward * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        // Ignore the shooter
        if (OwnerCollider != null && other == OwnerCollider) return;

        // Ignore other triggers (zones, pickups, etc.)
        if (other.isTrigger) return;

        // Deal damage if the thing is damageable
        other.GetComponent<IDamageable>()?.TakeDamage(Damage);

        Destroy(gameObject);
    }
}