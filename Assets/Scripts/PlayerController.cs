using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Height Zones")]
    public float heightA = 0f; // Height for zone A
    public float heightB = 10f; // Height for zone B
    public float heightC = 20f; // Height for zone C
    private float currentHeight;
    private string currentZone;
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
    public float maxHorizontalSpeed = 15f; // Maximum horizontal speed after dashing
    public float maxVerticalSpeed = 15f;
    private bool isDashing = false;
    private bool canDash = true;
    private float dashTimeLeft; // Timer for dash duration

    private float velocityXSmoothing;
    public float accelerationTime = 0.1f; // Time to reach full speed

    private Rigidbody2D rb;
    private bool isGrounded;
    private Transform groundCheck;

    [Header("Tracer Settings")]
    public GameObject tracerPrefab; // Prefab for the tracer effect
    public float tracerSpawnInterval = 0.05f; // Time between spawning tracers
    public float tracerHeightVariation = 0.2f; // Random height variation for tracers
    public float tracerFadeDuration = 0.5f; // Time in seconds for the tracer to fade out
    private float tracerSpawnTimer;

    private Animator animator;

    private SpriteRenderer playerSpriteRenderer; // Reference to the player's SpriteRenderer

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = transform.Find("GroundCheck"); // Create an empty GameObject for ground checking

        // Get the player's SpriteRenderer component
        playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();

        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        HandleJump();
        HandleDash();
        HandleOpacity();
        HandleHeight();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            float targetVelocity = moveInput * moveSpeed;
            rb.linearVelocity = new Vector2(Mathf.SmoothDamp(rb.linearVelocity.x, targetVelocity, ref velocityXSmoothing, accelerationTime), rb.linearVelocity.y);
        }

        // Max speed clamping for vertical
        if (rb.linearVelocity.y > maxVerticalSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxVerticalSpeed);
        }
        else if (rb.linearVelocity.y < -maxVerticalSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxVerticalSpeed);
        }

        Vector3 scale = transform.localScale;

        if (rb.linearVelocity.x > 0.1f)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        else if (rb.linearVelocity.x < -0.1f)
        {
            scale.x = -Mathf.Abs(scale.x);
        }

        transform.localScale = scale;
    }

    void UpdateAnimations()
    {
        float speed = Mathf.Abs(rb.linearVelocity.x);
        float verticalVelocity = rb.linearVelocity.y;

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalVelocity", verticalVelocity);
    }

    void HandleHeight()
    {
        currentHeight = transform.position.y;

        // Check if the player has stopped falling (vertical velocity is close to zero)
        bool hasStoppedFalling = Mathf.Abs(rb.linearVelocity.y) < 0.1f;

        // Determine the current height zone
        if (currentHeight < heightB)
        {
            if (currentZone != "A")
            {
                // Handle transitions into zone A
                if (hasStoppedFalling)
                {
                    if (currentZone == "B")
                    {
                        AudioManager.Instance.PlayBtoA();
                    }
                    else if (currentZone == "C")
                    {
                        AudioManager.Instance.PlayCtoA();
                    }

                    currentZone = "A";
                }
            }
        }
        else if (currentHeight < heightC)
        {
            if (currentZone != "B")
            {
                // Handle transitions into zone B
                if (hasStoppedFalling)
                {
                    if (currentZone == "A")
                    {
                        AudioManager.Instance.PlayAtoB();
                    }
                    else if (currentZone == "C")
                    {
                        AudioManager.Instance.PlayCtoB();
                    }

                    currentZone = "B";
                }
            }
        }
        else
        {
            if (currentZone != "C")
            {
                // Handle transitions into zone C
                if (hasStoppedFalling)
                {
                    if (currentZone == "B")
                    {
                        AudioManager.Instance.PlayBtoC();
                    }

                    currentZone = "C";
                }
            }
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

            // Spawn tracers at regular intervals
            tracerSpawnTimer -= Time.deltaTime;
            if (tracerSpawnTimer <= 0)
            {
                SpawnTracer();
                SpawnTracer();
                SpawnTracer();
                tracerSpawnTimer = tracerSpawnInterval;
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
        rb.gravityScale = 5; // Restore gravity (or your original gravity scale)

        // Preserve horizontal momentum after dashing
        float horizontalVelocity = rb.linearVelocity.x;
        horizontalVelocity = Mathf.Clamp(horizontalVelocity, -maxHorizontalSpeed, maxHorizontalSpeed); // Clamp to max speed

        // Preserve vertical momentum if moving downward (to allow sliding)
        float verticalVelocity = rb.linearVelocity.y;
        verticalVelocity = Mathf.Clamp(verticalVelocity, -maxVerticalSpeed, maxVerticalSpeed); // Clamp to max speed
        if (verticalVelocity < 0)
        {
            verticalVelocity = Mathf.Min(verticalVelocity, -1f); // Ensure the player doesn't stick to the ground
        }

        rb.linearVelocity = new Vector2(horizontalVelocity, verticalVelocity);
    }
    void SpawnTracer()
    {
        if (tracerPrefab != null)
        {
            Vector3 spawnPosition = transform.position;

            // Instantiate the tracer
            GameObject tracer = Instantiate(tracerPrefab, spawnPosition, Quaternion.identity);

            // Get the player's sprite renderer to compare sizes
            SpriteRenderer playerSprite = GetComponent<SpriteRenderer>();
            SpriteRenderer tracerSprite = tracer.GetComponent<SpriteRenderer>();

            if (playerSprite != null && tracerSprite != null)
            {
                // Calculate the scale needed to match the player's size
                Vector3 newScale = tracer.transform.localScale;
                newScale.x = (playerSprite.bounds.size.x / tracerSprite.bounds.size.x) * Mathf.Sign(newScale.x);
                newScale.y = playerSprite.bounds.size.y / tracerSprite.bounds.size.y;

                // Apply the scale
                tracer.transform.localScale = newScale / 50f;
            }

            // Rotate the tracer to match the player's direction
            float playerDirection = Mathf.Sign(rb.linearVelocity.x); // 1 for right, -1 for left
            if (playerDirection != 0) // Only rotate if the player is moving horizontally
            {
                // We already handled x scale above, so just flip the existing scale
                tracer.transform.localScale = new Vector3(
                    Mathf.Abs(tracer.transform.localScale.x) * playerDirection,
                    tracer.transform.localScale.y,
                    tracer.transform.localScale.z);
            }

            // Set the fade duration for the tracer
            TracerFade tracerFade = tracer.GetComponent<TracerFade>();
            if (tracerFade != null)
            {
                tracerFade.fadeDuration = tracerFadeDuration;
            }
        }
    }

    void HandleOpacity()
    {
        // Set the player's opacity based on their state
        if (!isGrounded && !canDash)
        {
            // Player is in the air and can't dash: set opacity to 75%
            SetPlayerOpacity(0.75f);
        }
        else
        {
            // Player is on the ground or can dash: set opacity to 100%
            SetPlayerOpacity(1f);
        }
    }

    void SetPlayerOpacity(float alpha)
    {
        // Update the player's sprite color with the new alpha value
        Color color = playerSpriteRenderer.color;
        color.a = alpha;
        playerSpriteRenderer.color = color;
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