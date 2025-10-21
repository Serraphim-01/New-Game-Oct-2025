using UnityEngine;

public class EnhancedBallMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveForce = 15f;
    public float maxSpeed = 8f;
    public float jumpForce = 10f;
    public float airControl = 0.5f;

    [Header("Smoothness Settings")]
    public float rotationSpeed = 3f;
    public float speedSmoothing = 10f;
    public float groundCheckDistance = 0.6f;
    public LayerMask groundLayer;

    [Header("Camera Reference")]
    public Camera playerCamera;

    private Rigidbody rb;
    private bool isGrounded;
    private Vector3 moveDirection;
    private Vector3 currentVelocity;

    void Awake()
    {
        // Ensure the player has the "Player" tag
        if (string.IsNullOrEmpty(gameObject.tag) || gameObject.tag != "Player")
        {
            gameObject.tag = "Player";
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // If no camera assigned, use main camera
        if (playerCamera == null)
            playerCamera = Camera.main;

        // Optimize Rigidbody for smooth ball movement
        if (rb != null)
        {
            rb.drag = 1f;
            rb.angularDrag = 1f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    void Update()
    {
        GetInput();
        CheckGrounded();
        HandleJump();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    void GetInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Get camera-relative direction
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;
    }

    void HandleMovement()
    {
        if (moveDirection.magnitude >= 0.1f)
        {
            float currentMoveForce = moveForce;

            // Reduce control in air
            if (!isGrounded)
                currentMoveForce *= airControl;

            // Apply force
            rb.AddForce(moveDirection * currentMoveForce, ForceMode.Force);

            // Smooth speed limiting
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (horizontalVelocity.magnitude > maxSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
                rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
            }
        }

        // Additional drag when no input for quicker stopping
        if (moveDirection.magnitude < 0.1f && isGrounded)
        {
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0f, rb.velocity.y, 0f), speedSmoothing * Time.fixedDeltaTime);
        }
    }

    void HandleRotation()
    {
        if (moveDirection.magnitude >= 0.1f && rb.velocity.magnitude > 0.5f)
        {
            // Calculate rotation based on movement direction
            Vector3 rollDirection = rb.velocity.normalized;
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, rollDirection).normalized;

            // Apply smooth rotation
            float rotationAmount = rb.velocity.magnitude * rotationSpeed * Time.fixedDeltaTime;
            Quaternion deltaRotation = Quaternion.AngleAxis(rotationAmount, rotationAxis);

            rb.MoveRotation(rb.rotation * deltaRotation);
        }
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void CheckGrounded()
    {
        RaycastHit hit;
        isGrounded = Physics.SphereCast(transform.position, 0.4f, Vector3.down, out hit, groundCheckDistance, groundLayer);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * groundCheckDistance, 0.4f);
    }
}
