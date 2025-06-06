using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

public class PlayerMovement: MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 2.5f;
    

    private InputAction moveAction;
    private InputAction jumpAction;

    private Rigidbody2D body;
    private BoxCollider2D boxCollider;

    [SerializeField] private LayerMask groundLayer;

    private Animator animator;

    private int scaleValue = 5;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }


    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 movement = moveAction.ReadValue<Vector2>();
        HandleHorizontalMovement(movement.x);
        HandleVerticalMovement(movement.y);

        animator.SetBool("isRunning", movement.x != 0);
        animator.SetBool("grounded", isGrounded());
    }

    private void HandleHorizontalMovement(float x)
    {
        if (x > 0.01f) transform.localScale = new Vector3(1, 1, 1) * scaleValue;
        else if (x < -0.01f) transform.localScale = new Vector3(-1, 1, 1) * scaleValue;

        body.linearVelocityX = x * speed;
    }

    private void HandleVerticalMovement(float y)
    {
        if (jumpAction.triggered && isGrounded())
        {
            body.linearVelocityY = jumpForce;
            animator.SetTrigger("isJumping");
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }
}
