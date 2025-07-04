using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class EnemyState : MonoBehaviour
{
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    protected Animator animator;

    [SerializeField] private LayerMask groundLayer;

    public int scaleValue = 5;

    public bool isAttacking = false;
    protected float attackTimeLeft = 0;

    protected Health health;

    [SerializeField] private Transform groundCheck;

    private float onEdgeCheckingDistance = 1f;

    [SerializeField] private float collisionDamage;
    [SerializeField] private float collisionDamageCooldown = 5f;
    private float timeSinceCollisionDamage = 0f;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        health = GetComponent<Health>();
    }

    public virtual void Update()
    {
        if (IsDead())
        {
            return;
        }

        timeSinceCollisionDamage += Time.deltaTime;
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

    public virtual bool CanPrimaryAttack()
    {
        return IsGrounded() && !isAttacking && !IsDead();
    }

    public void StartAttack(float attackTime)
    {
        body.linearVelocityX = 0;
        isAttacking = true;
        attackTimeLeft = attackTime;
    }

    public virtual void TakeDamage(float damage)
    {
        if (IsDead()) return;
        isAttacking = false;
        attackTimeLeft = 0;
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

    public void ManageAttackAnimationAndSound(AttackAction attackAction)
    {
        StartAttack(attackAction.AnimationDuration);
        animator.SetTrigger(attackAction.AnimationTrigger);
        SoundManager.instance.PlaySound(attackAction.AnimationSound);
    }

    public void OnChildTrigger(TriggerData data)
    {
        Debug.Log($"{data.type}; {data.playerCollider}");
        if (data.type == EnemyPlayerOverlapDetector.ColliderType.Body)
        {
            if (timeSinceCollisionDamage < collisionDamageCooldown) return;
            data.playerCollider.GetComponent<PlayerState>().TakeDamage(collisionDamage, true);
            timeSinceCollisionDamage = 0;
        }
    }

    public bool IsStunned()
    {
        return health.IsBeingHurt();
    }
}
