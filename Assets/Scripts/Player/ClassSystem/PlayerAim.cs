using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera cam;

    [Header("Raycast")]
    [SerializeField] private float maxAimDistance = 100f;
    [Tooltip("Layers the aim ray can hit. MUST exclude Player.")]
    [SerializeField] private LayerMask aimLayers;

    [Header("World Reticle (Optional)")]
    [SerializeField] private Transform worldReticle;

    public Vector3 AimPoint { get; private set; }
    public bool HasTarget { get; private set; }

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        // Default fallback so something is always valid
        AimPoint = transform.position + transform.forward * maxAimDistance;
    }

    void LateUpdate()
    {
        UpdateAim();
    }

    void UpdateAim()
    {
        Vector3 screenCenter = new Vector3(0.5f, 0.5f, 0f);
        Ray ray = cam.ViewportPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit hit, maxAimDistance, aimLayers))
        {
            AimPoint = hit.point;
            HasTarget = true;

            if (worldReticle != null)
            {
                worldReticle.position = hit.point + hit.normal * 0.05f;
                worldReticle.rotation = Quaternion.LookRotation(hit.normal);
                worldReticle.gameObject.SetActive(true);
            }
        }
        else
        {
            AimPoint = ray.origin + ray.direction * maxAimDistance;
            HasTarget = false;

            if (worldReticle != null)
                worldReticle.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Returns a rotation pointing from spawnPos toward the current aim point.
    /// </summary>
    public Quaternion GetAimRotation(Vector3 spawnPos)
    {
        Vector3 dir = (AimPoint - spawnPos).normalized;
        if (dir.sqrMagnitude < 0.0001f) dir = transform.forward;
        return Quaternion.LookRotation(dir);
    }
}