using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f; // Horizontal movement speed
    public float jumpForce = 12f; // Jump force
    public float groundCheckRadius = 0.1f; // Radius for ground check
    public LayerMask groundLayer; // Layer for ground objects

    [Header("Coyote Time Settings")]
    public float coyoteTime = 0.1f; // Time in seconds the player can jump after leaving the ground
    private float coyoteTimeCounter;

    [Header("Jump Buffering Settings")]
    public float jumpBufferTime = 0.1f; // Time in seconds to buffer a jump
    private float jumpBufferCounter;

    [Header("Dash Settings")]
    public float dashSpeed = 20f; // Speed of the dash
    public float dashDuration = 0.2f; // How long the dash lasts
    private bool isDashing = false;
    private bool canDash = true;
    private float dashTimeLeft; // Timer for dash duration

    private float velocityXSmoothing;
    public float accelerationTime = 0.1f; // Time to reach full speed

    private Rigidbody2D rb;
    private bool isGrounded;
    private Transform groundCheck;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = transform.Find("GroundCheck"); // Create an empty GameObject for ground checking
    }

    void Update()
    {
        HandleJump();
        HandleDash();
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            float targetVelocity = moveInput * moveSpeed;
            rb.linearVelocity = new Vector2(Mathf.SmoothDamp(rb.linearVelocity.x, targetVelocity, ref velocityXSmoothing, accelerationTime), rb.linearVelocity.y);
        }
    }

    void HandleJump()
    {
        // Check if the player is grounded
        bool wasGrounded = isGrounded; // Store the previous grounded state
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Reset dash ability when landing
        if (isGrounded)
        {
            canDash = true; // Allow dashing again after touching the ground
        }

        // Update coyote time counter
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime; // Reset the counter when grounded
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime; // Decrease the counter when in the air
        }

        // Update jump buffer counter
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime; // Start the buffer when jump is pressed
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime; // Decrease the buffer over time
        }

        // Handle jumping with both coyote time and jump buffering
        if (jumpBufferCounter > 0 && (isGrounded || coyoteTimeCounter > 0))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0; // Reset the buffer after jumping
            coyoteTimeCounter = 0; // Prevent double jumping
        }
    }

    void HandleDash()
    {
        // Check if the player can dash and presses the X key
        if (Input.GetKeyDown(KeyCode.X) && canDash && !isDashing && !isGrounded)
        {
            StartDash();
        }

        // Handle dash duration
        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0)
            {
                StopDash();
            }
        }
    }

    void StartDash()
    {
        isDashing = true;
        canDash = false; // Disable dashing until the player touches the ground
        dashTimeLeft = dashDuration;

        // Get the direction of the dash based on input
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Normalize the direction to ensure consistent dash speed
        Vector2 dashDirection = new Vector2(horizontalInput, verticalInput).normalized;

        // Apply dash velocity
        rb.linearVelocity = dashDirection * dashSpeed;

        // Optional: Disable gravity during the dash
        rb.gravityScale = 0;
    }

    void StopDash()
    {
        isDashing = false;
        rb.linearVelocity = Vector2.zero; // Stop the dash abruptly
        rb.gravityScale = 5; // Restore gravity (or your original gravity scale)
    }

    private void OnDrawGizmosSelected()
    {
        // Draw ground check radius in the editor
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}