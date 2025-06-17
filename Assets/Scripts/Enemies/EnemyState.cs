using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyState : MonoBehaviour
{
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private Animator animator;

    [SerializeField] private LayerMask groundLayer;

    public int scaleValue = 5;

    public bool isAttacking = false;
    private float attackTimeLeft = 0;

    private Health health;

    [SerializeField] private Transform groundCheck;

    private float onEdgeCheckingDistance = 1f;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        health = GetComponent<Health>();
    }

    private void Update()
    {
        if (IsDead())
        {
            return;
        }

        if (isAttacking)
        {
            attackTimeLeft -= Time.deltaTime;
            if (attackTimeLeft <= 0)
            {
                isAttacking = false;
                attackTimeLeft = 0;
            }
        }
        animator.SetBool("walking", IsRunning());
        animator.SetBool("grounded", IsGrounded());
    }

    public bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }

    public bool IsRunning()
    {
        return body.linearVelocityX != 0;
    }

    public bool IsMoving()
    {
        return (body.linearVelocityX != 0) || (body.linearVelocityY != 0);
    }

    public bool CanPrimaryAttack()
    {
        return IsGrounded() && !IsMoving() && !isAttacking;
    }

    private bool IsPlayerLeft()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        return mousePosition.x < transform.position.x;
    }

    public void StartAttack(float attackTime)
    {
        if (!IsPlayerLeft()) transform.localScale = new Vector3(1, 1, 1) * scaleValue;
        else transform.localScale = new Vector3(-1, 1, 1) * scaleValue;

        isAttacking = true;
        attackTimeLeft = attackTime;
    }

    public void TakeDamage(float damage)
    {
        health.TakeDamage(damage);
    }

    public bool IsDead()
    {
        return health.isDead;
    }

    public bool IsOnEdge()
    {
        if (!IsGrounded())
        {
            return false;
        }
        RaycastHit2D isOnEdge = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer);
        return isOnEdge.collider==null;
    }
}
