using UnityEngine;

public class TrapMovement : MonoBehaviour
{
    [SerializeField] private float speed = 10f;

    private Rigidbody2D body;
    private EnemyState enemyState;

    private Animator animator;

    private float direction = 1f;
    private float old_direction = 1f;
    private float idle_time = 1f;
    private float timePassed = 0f;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        enemyState = GetComponent<EnemyState>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (enemyState.IsDead()) return;
        if (enemyState.isAttacking) return;
        timePassed += Time.deltaTime;
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (direction == 0 && timePassed > idle_time)
        {
            direction = -old_direction;
        }

        HandleHorizontalMovement(direction);
        if (direction != 0 && enemyState.IsOnEdge())
        {
            OnEdgeReached();
        }
    }

    private void HandleHorizontalMovement(float x)
    {
        if (x > 0.01f) transform.localScale = new Vector3(-1, 1, 1) * enemyState.scaleValue;
        else if (x < -0.01f) transform.localScale = new Vector3(1, 1, 1) * enemyState.scaleValue;

        body.linearVelocityX = x * speed;
    }

    private void OnEdgeReached()
    {
        old_direction = direction;
        direction = 0f;
        timePassed = 0f;
    }
}
