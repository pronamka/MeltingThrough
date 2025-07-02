using Unity.VisualScripting;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float playerDetectionDistance = 10f;

    private Rigidbody2D body;
    private EnemyState enemyState;

    private Animator animator;

    private float direction = 1f;
    private float old_direction = 1f;
    private float idle_time = 1f;
    private float timePassed = 0f;

    private Transform player;
    private bool playerDetected;

    private Vector2 lastPosition;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        enemyState = GetComponent<EnemyState>();
        animator = GetComponent<Animator>();

        lastPosition = transform.position;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (enemyState.IsDead()) return;
        if (enemyState.isAttacking) return;
        if (enemyState.IsStunned()) return;
        timePassed += Time.deltaTime;
        HandleMovement();

        Vector2 newPosition = transform.position;
        float x = (newPosition - lastPosition).x;
        if (x != 0)
        {
            animator.SetBool("walking", true);
        }
        else
        {
            animator.SetBool("walking", false);
        }


        if (x > 0.01f) transform.localScale = new Vector3(-1, 1, 1) * enemyState.scaleValue;
        else if (x < -0.01f) transform.localScale = new Vector3(1, 1, 1) * enemyState.scaleValue;
        lastPosition = newPosition;
    }

    private void HandleMovement()
    {
        if (playerDetected)
        {
            MoveTowardsPlayer();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer < playerDetectionDistance)
        {
            playerDetected = true;
            MoveTowardsPlayer();
        }
        else
        {
            MoveIdly();
        }
    }

    private void MoveIdly()
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

    private void MoveTowardsPlayer()
    {
        Vector2 newPosition = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        Debug.Log($"{enemyState.IsOnEdge()}; {transform.localScale.x}; {transform.position.x} > {newPosition.x} = {transform.position.x > newPosition.x}");
        if (enemyState.IsOnEdge() && transform.localScale.x > 0 && transform.position.x > newPosition.x)
        {
            return;
        }

        if (enemyState.IsOnEdge() && transform.localScale.x < 0 && transform.position.x < newPosition.x)
        {
            return;
        }
        transform.position = new Vector2(newPosition.x, transform.position.y);
    }

}
