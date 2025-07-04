using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public float speed = 10f;
    [SerializeField] public float jumpForce = 2.5f;

    [Header("Dash Settings")]
    [SerializeField] public float dashForce = 20f;
    [SerializeField] public float dashDuration = 0.2f;
    [SerializeField] public float dashCooldown = 1f;

    [Header("Double Jump Settings")]
    [SerializeField] public float doubleJumpForce = 15f;
    [SerializeField] public int maxJumps = 2;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction; 

    private Rigidbody2D body;
    private PlayerState playerState;
    private Animator animator;

    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound; 

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector2 dashDirection;
    private float originalGravityScale;

    private int currentJumps = 0;
    private bool wasGroundedLastFrame = false;

    
    private Vector2 lastMovementDirection = Vector2.right;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        playerState = GetComponent<PlayerState>();
        animator = GetComponent<Animator>();
        originalGravityScale = body.gravityScale;
    }

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        dashAction = InputSystem.actions.FindAction("Dash"); 

        
        if (dashAction == null)
        {
            Debug.LogWarning("Dash action не найден! Убедитесь что в Input Actions есть действие 'Dash' привязанное к Shift");
        }
    }

    private void Update()
    {
        if (playerState.IsDead()) return;

        
        if (playerState.IsGrounded() && !wasGroundedLastFrame)
        {
            currentJumps = 0;
        }
        wasGroundedLastFrame = playerState.IsGrounded();

        HandleDash();

        if (!isDashing)
        {
            HandleMovement();
        }
        else
        {
            animator.SetBool("grounded", false);
            animator.SetBool("isRunning", false);
        }
    }

    private void HandleDash()
    {
        
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        
        if (dashAction != null && dashAction.triggered && dashCooldownTimer <= 0 && !isDashing)
        {
            Vector2 movement = moveAction.ReadValue<Vector2>();


            Vector2 direction = Vector2.zero;

            if (Mathf.Abs(movement.x) > 0.1f)
            {

                direction = movement.x > 0 ? Vector2.right : Vector2.left;
                lastMovementDirection = direction;
            }
            else
            {

                direction = lastMovementDirection;
            }

            StartDash(direction);
        }


        if (isDashing)
        {
            dashTimer -= Time.deltaTime;


            RaycastHit2D hit = Physics2D.Raycast(transform.position, dashDirection, 1f);
            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                EndDash();
            }

            
            if (dashTimer <= 0f)
            {
                EndDash();
            }
        }
    }

    private void StartDash(Vector2 direction)
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;
        dashDirection = direction;
        dashTimer = dashDuration;

        
        animator.SetTrigger("isJumping"); 
        animator.SetBool("grounded", false);

        if (dashSound != null)
            SoundManager.instance.PlaySound(dashSound);

        
        body.linearVelocity = Vector2.zero;
        body.gravityScale = 0f;
        body.AddForce(direction * dashForce, ForceMode2D.Impulse);

        Debug.Log($"Дэш в направлении: {direction}");
    }

    private void EndDash()
    {
        isDashing = false;
        dashTimer = 0f;
        body.gravityScale = originalGravityScale;

        
        body.linearVelocity = new Vector2(body.linearVelocity.x * 0.5f, body.linearVelocity.y);
    }

    private void HandleMovement()
    {
        Vector2 movement = moveAction.ReadValue<Vector2>();
        HandleHorizontalMovement(movement.x);
        HandleVerticalMovement(movement.y);

        
        if (Mathf.Abs(movement.x) > 0.1f)
        {
            lastMovementDirection = movement.x > 0 ? Vector2.right : Vector2.left;
        }

        animator.SetBool("isRunning", playerState.IsRunning());
        animator.SetBool("grounded", playerState.IsGrounded());
    }

    private void HandleHorizontalMovement(float x)
    {
        if (x > 0.01f) transform.localScale = new Vector3(1, 1, 1) * playerState.scaleValue;
        else if (x < -0.01f) transform.localScale = new Vector3(-1, 1, 1) * playerState.scaleValue;

        body.linearVelocityX = x * speed;
    }

    private void HandleVerticalMovement(float y)
    {
        if (jumpAction.triggered)
        {
            if (playerState.IsGrounded())
            {
                
                body.linearVelocityY = jumpForce;
                currentJumps = 1;
                animator.SetTrigger("isJumping");
                SoundManager.instance.PlaySound(jumpSound);
            }
            else if (currentJumps < maxJumps)
            {
              
                body.linearVelocityY = doubleJumpForce;
                currentJumps++;
                animator.SetTrigger("isJumping");
                SoundManager.instance.PlaySound(jumpSound);
            }
        }
    }

    public bool IsDashing()
    {
        return isDashing;
    }

    
    public bool CanDash()
    {
        return dashCooldownTimer <= 0 && !isDashing;
    }

    public float GetDashCooldownProgress()
    {
        return 1f - (dashCooldownTimer / dashCooldown);
    }
}