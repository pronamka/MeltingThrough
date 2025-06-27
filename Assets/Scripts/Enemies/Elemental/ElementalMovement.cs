using UnityEngine;

public class ElementalMovement : MonoBehaviour
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
        timePassed += Time.deltaTime;
        HandleMovement();

        Vector2 newPosition = transform.position;
        float x = (newPosition - lastPosition).x;


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
    }

    private void MoveTowardsPlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

}
