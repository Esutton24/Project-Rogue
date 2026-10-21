using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Orbit Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -4f);
    [SerializeField] private float rotationSpeed = 100f;

    [Header("Vertical Limits")]
    [SerializeField] private float minVerticalAngle = -20f;
    [SerializeField] private float maxVerticalAngle = 60f;

    [Header("Sensitivity")]
    [SerializeField] private float mouseLookMultiplier = 1f;
    [SerializeField] private float controllerLookMultiplier = 2.5f;

    [Header("Smoothing")]
    [SerializeField] private float yawSmoothTime = 0.15f;
    [SerializeField] private float pitchSmoothTime = 0.15f;

    [Header("Locking")]
    [SerializeField] private bool lockPositionToTarget = true;
    [SerializeField] private float followSmoothTime = 0.1f; // only used if locking is off

    private PlayerInputActions inputActions;

    private float targetYaw;
    private float currentYaw;
    private float yawSmoothVelocity;

    private float targetPitch;
    private float currentPitch;
    private float pitchSmoothVelocity;

    private Vector3 positionDampVelocity;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        targetYaw = angles.y;
        currentYaw = targetYaw;

        // Normalize pitch to signed range so negative starts work correctly
        targetPitch = angles.x;
        if (targetPitch > 180f) targetPitch -= 360f;
        targetPitch = Mathf.Clamp(targetPitch, minVerticalAngle, maxVerticalAngle);
        currentPitch = targetPitch;
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector2 lookInput = inputActions.Player.Look.ReadValue<Vector2>();

        float multiplier = lookInput.magnitude > 1.5f
            ? mouseLookMultiplier
            : controllerLookMultiplier;

        // Accumulate target angles from raw input
        targetYaw += lookInput.x * rotationSpeed * multiplier * Time.deltaTime;
        targetPitch -= lookInput.y * rotationSpeed * multiplier * Time.deltaTime;
        targetPitch = Mathf.Clamp(targetPitch, minVerticalAngle, maxVerticalAngle);

        // Smooth current angles toward targets
        currentYaw = Mathf.SmoothDampAngle(currentYaw, targetYaw, ref yawSmoothVelocity, yawSmoothTime);
        currentPitch = Mathf.SmoothDamp(currentPitch, targetPitch, ref pitchSmoothVelocity, pitchSmoothTime);

        // Build orbit rotation and derive position
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        Vector3 desiredPosition = target.position + rotation * offset;

        if (lockPositionToTarget)
        {
            transform.position = desiredPosition;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(
                transform.position, desiredPosition, ref positionDampVelocity, followSmoothTime
            );
        }

        transform.rotation = rotation;
    }

    void OnDestroy()
    {
        inputActions?.Dispose();
    }
}