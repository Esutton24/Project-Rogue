using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("Rotation")]
    [Tooltip("How fast the player turns to face movement / camera")]
    [SerializeField] private float turnSpeed = 10f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 1.05f;

    [Header("Gravity")]
    [SerializeField] private float gravityMultiplier = 2f;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    private Rigidbody rb;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private bool jumpRequested;
    private float baseMoveSpeed;
    private float baseJumpForce;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new PlayerInputActions();
        baseMoveSpeed = moveSpeed;
        baseJumpForce = jumpForce;
    }

    void OnEnable()
    {
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Jump.performed += OnJumpRequested;
        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Jump.performed -= OnJumpRequested;
        inputActions.Player.Disable();
    }

    void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void OnJumpRequested(InputAction.CallbackContext context)
    {
        if (context.performed)
            jumpRequested = true;
    }

    public void ApplyClassStats(float speedMult, float jumpMult)
    {
        moveSpeed = baseMoveSpeed * speedMult;
        jumpForce = baseJumpForce * jumpMult;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    void FixedUpdate()
    {
        // --- Camera-relative move vector ---
        Vector3 camForward = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
        Vector3 camRight = cameraTransform != null ? cameraTransform.right : Vector3.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = (camRight * moveInput.x) + (camForward * moveInput.y);

        // --- Apply velocity ---
        rb.linearVelocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);

        // --- Rotate capsule to face move direction, or camera when idle ---
        Vector3 lookDir = Vector3.zero;

        if (move.sqrMagnitude > 0.01f)
        {
            lookDir = move.normalized; // face where we are running
        }
        else if (cameraTransform != null)
        {
            Vector3 camFlat = cameraTransform.forward;
            camFlat.y = 0f;
            if (camFlat.sqrMagnitude > 0.001f)
                lookDir = camFlat.normalized; // face where camera looks when standing still
        }

        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, turnSpeed * Time.fixedDeltaTime));
        }

        // --- Extra gravity while airborne ---
        if (!IsGrounded())
            rb.AddForce(Physics.gravity * (gravityMultiplier - 1f), ForceMode.Acceleration);

        // --- Jump ---
        if (jumpRequested && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpRequested = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}