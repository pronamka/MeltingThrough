using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class PlayerState : MonoBehaviour
{
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private Animator animator;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] public float fallDamageMultiplier;
    private float fallSpeed = 0;

    public int scaleValue = 5;

    public bool isAttacking = false;
    private float attackTimeLeft = 0;

    private Health health;
    public PlayerMana mana { get; private set; }

    private InputAction changeStateAction;
    private bool isOnFire = false;

    private InputAction takeDamageAction;

    private string menuSceneName = "MenuScene";

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        health = GetComponent<Health>();
        mana = GetComponent<PlayerMana>();
    }

    private void Start()
    {
        changeStateAction = InputSystem.actions.FindAction("ChangeState");
        changeStateAction.Enable();

        takeDamageAction = InputSystem.actions.FindAction("TakeDamage");
        takeDamageAction.Enable();
    }

    private void Update()
    {
        if (transform.position.y < -1000)
        {
            TakeDamage(1000);
        }
        
        fallSpeed = body.linearVelocityY;

        if (takeDamageAction.triggered)
        {
            TakeDamage(20);
        }

        if (changeStateAction.triggered)
        {
            isOnFire = !isOnFire;
            animator.SetTrigger("transition");
            animator.SetBool("isOnFire", isOnFire);
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
        
    }

    private IEnumerator LoadNewScene()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(menuSceneName);
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
        return !isAttacking;
    }

    public bool CanBonusAttack()
    {
        return !isAttacking;
    }

    private bool IsMouseLeft()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        return mousePosition.x < transform.position.x;
    }

    public void StartAttack(float attackTime)
    {
        if (!IsMouseLeft()) transform.localScale = new Vector3(1, 1, 1) * scaleValue;
        else transform.localScale = new Vector3(-1, 1, 1) * scaleValue;

        isAttacking = true;
        attackTimeLeft = attackTime;
    }

    public void TakeDamage(float damage, bool collisionDamage = false)
    {
        if (IsDead()) return;
        health.TakeDamage(damage);
    }

    public bool IsDead()
    {
        return health.isDead;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (fallSpeed < -40)
            {
                float fallDamage = (Mathf.Abs(fallSpeed) - 40) / 10 * fallDamageMultiplier;
                TakeDamage(fallDamage);
            }

            fallSpeed = 0;
        }
    }
}
