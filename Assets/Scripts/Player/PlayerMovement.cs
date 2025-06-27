using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public float speed = 10f;
    [SerializeField] public float jumpForce = 2.5f;

    private InputAction moveAction;
    private InputAction jumpAction;

    private Rigidbody2D body;
    private PlayerState playerState;

    private Animator animator;

    [SerializeField] private AudioClip jumpSound;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        playerState = GetComponent<PlayerState>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    private void Update()
    {
        if (playerState.IsDead()) return;
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 movement = moveAction.ReadValue<Vector2>();
        HandleHorizontalMovement(movement.x);
        HandleVerticalMovement(movement.y);

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
        if (jumpAction.triggered && playerState.IsGrounded())
        {
            body.linearVelocityY = jumpForce;
            animator.SetTrigger("isJumping");
            SoundManager.instance.PlaySound(jumpSound);
        }
    }
}
