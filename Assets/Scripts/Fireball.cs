using UnityEngine;
using UnityEngine.EventSystems;

public class Fireball : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float maxFlyingTime;
    [SerializeField] private float damage;
    private float timeFlying = 0;

    private Vector2 direction;

    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private Animator animator;

    private bool exploded;

    private FireballPool pool;

    private AnimationUtils animationUtils;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        animationUtils = new AnimationUtils(animator);
    }

    void Update()
    {
        if (exploded) return;
        if (timeFlying > maxFlyingTime) Explode();

        //transform.Translate(direction*speed*Time.deltaTime);
        timeFlying += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("AttackZones"))
        {
            return;
        }

        body.linearVelocity = new Vector2(0f, 0f);
        animator.SetTrigger("explode");
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            collision.GetComponent<EnemyState>().TakeDamage(damage);
        }
        Explode();
    }

    private void Explode()
    {
        exploded = true;
        boxCollider.enabled = false;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        Invoke(nameof(ReturnToPool), animationUtils.GetAnimationDuration(AnimationNames.FireballExplode));
    }

    private void ReturnToPool()
    {
        pool.ReturnFireball(this);
    }

    public void SetDirection(Vector2 newDirection, Vector2 additionalSpeed)
    {
        direction = newDirection.normalized;
        gameObject.SetActive(true);
        exploded = false;
        boxCollider.enabled = true;

        float atAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Deg2Rad;
        transform.rotation = Quaternion.AngleAxis(atAngle, Vector3.forward);

        body.linearVelocity = direction*speed;

        timeFlying = 0;
    }

    public void SetPool(FireballPool fireballPool)
    {
        pool = fireballPool;
    }
}
