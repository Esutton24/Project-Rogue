using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 3f;

    public float Damage { get; set; } = 10f;
    public Transform OwnerRoot { get; set; }

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = transform.forward * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        // Ignore ANY collider that belongs to the shooter (root + all children)
        if (OwnerRoot != null &&
            (other.transform == OwnerRoot || other.transform.IsChildOf(OwnerRoot)))
        {
            return;
        }

        // Ignore pure trigger zones (pickups, detectors) that can't take damage
        var damageable = other.GetComponent<IDamageable>();
        if (other.isTrigger && damageable == null) return;

        damageable?.TakeDamage(Damage);
        Destroy(gameObject);
    }
}