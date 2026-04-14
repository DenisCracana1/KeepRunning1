using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Refs")]
    public Rigidbody2D rb;
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundLayer;

    [Header("Movement")]
    public float moveAccel = 40f;
    public float maxRunSpeed = 9f;
    public float crouchSpeed = 3f;
    public float speedDecay = 6f;

    [Header("Gravity")]
    public float gravity = 30f;      // ajustado para que no sea tan bestia
    public float fallGravity = 40f;  // un poco más fuerte al caer

    [Header("Jump")]
    public float jumpForce = 20f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    public float bunnyHopMultiplier = 1.1f;

    [Header("Dash")]
    public float dashForce = 18f;
    public float dashDuration = 0.15f;
    public int maxDashes = 1;

    [Header("Wall")]
    public float wallSlideSpeed = -3f;
    public float wallJumpHorizontalForce = 12f;
    public float wallJumpVerticalForce = 16f;

    [Header("Climb")]
    public float climbSpeed = 4f;
    public float maxStamina = 3f;          // un poco más de margen
    public float wallClimbStaminaCostIdle = 0.5f;   // quieto agarrado
    public float wallClimbStaminaCostMove = 1.2f;   // moviéndote arriba/abajo
    public float wallClimbStaminaCostJump = 2.5f;   // saltando desde agarre

    [Header("Special Movements")]
    public float hyperBoost = 1.6f;
    public float superBoost = 1.3f;
    public float waveDashVerticalBoost = 8f;

    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isTouchingWall;
    [HideInInspector] public int wallDir;
    [HideInInspector] public bool isHyperDash;
    [HideInInspector] public int dashesLeft;
    [HideInInspector] public float currentStamina;

    float coyoteCounter;
    float jumpBufferCounter;
    bool jumpHeldLastFrame;

    public PlayerStateMachine stateMachine;

    public IdleState idleState;
    public RunState runState;
    public JumpState jumpState;
    public FallState fallState;
    public CrouchState crouchState;
    public DashState dashState;
    public WallSlideState wallSlideState;
    public WallJumpState wallJumpState;
    public ClimbState climbState;
    public Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        stateMachine = new PlayerStateMachine();
        var factory = new PlayerStateFactory(this, stateMachine);

        idleState = factory.Idle();
        runState = factory.Run();
        jumpState = factory.Jump();
        fallState = factory.Fall();
        crouchState = factory.Crouch();
        dashState = factory.Dash();
        wallSlideState = factory.WallSlide();
        wallJumpState = factory.WallJump();
        climbState = factory.Climb();
        anim = GetComponent<Animator>();

        dashesLeft = maxDashes;
        currentStamina = maxStamina;
    }

    private void Start()
    {
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        ReadJumpInput();
        CheckEnvironment();
        UpdateCoyoteAndBuffer();
        stateMachine.currentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        stateMachine.currentState.PhysicsUpdate();
    }

    void ReadJumpInput()
    {
        bool jumpNow = Input.GetKeyDown(KeyCode.Space);
        bool jumpHeld = Input.GetKey(KeyCode.Space);

        if (jumpNow)
        {
            jumpBufferCounter = jumpBufferTime;
        }

        if (!jumpHeld && jumpHeldLastFrame && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        jumpHeldLastFrame = jumpHeld;
    }

    void UpdateCoyoteAndBuffer()
    {
        if (isGrounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0)
            jumpBufferCounter -= Time.deltaTime;
    }

    public bool CanUseCoyoteJump()
    {
        return coyoteCounter > 0 && jumpBufferCounter > 0;
    }

    public void ConsumeJumpBuffer()
    {
        jumpBufferCounter = 0;
    }

    void ApplyGravity()
    {
        if (stateMachine.currentState == dashState ||
            stateMachine.currentState == climbState)
            return;

        float g = rb.linearVelocity.y < 0 ? fallGravity : gravity;
        rb.linearVelocity += Vector2.down * g * Time.fixedDeltaTime;
    }

    void CheckEnvironment()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, 0.1f, groundLayer);
        wallDir = isTouchingWall ? (int)Mathf.Sign(transform.localScale.x) : 0;

        if (isGrounded)
        {
            dashesLeft = maxDashes;
            currentStamina = maxStamina;
        }
    }

    public bool WantJump()
    {
        return jumpBufferCounter > 0;
    }

    public void ClearJumpIntent()
    {
        jumpBufferCounter = 0;
    }

    public void CheckFlip(float x)
    {
        if (x > 0 && transform.localScale.x < 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (x < 0 && transform.localScale.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }
}
