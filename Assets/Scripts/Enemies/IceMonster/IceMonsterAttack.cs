using UnityEngine;

public class IceMonsterAttack : MonoBehaviour
{
    [SerializeField] private float attackDistance;
    [SerializeField] private float damage;
    [SerializeField] private float cooldown;
    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private AudioClip attackSound;
    private EnemyState state;

    private AttackAction attackAction;

    private float timeSinceAttack = 0f;

    private Collider2D playerCollider;

    private void Awake()
    {
        state = GetComponent<EnemyState>();

        attackAction = new AttackAction(
            AnimationNames.EnemyPrimaryAttack,
            cooldown,
            //animationUtils.GetAnimationDuration(AnimationNames.EnemyPrimaryAttack),
            AnimationParameters.EnemyPrimaryAttack,
            attackSound
        );
    }

    private void Update()
    {
        timeSinceAttack += Time.deltaTime;
        if (state.CanPrimaryAttack() && timeSinceAttack > cooldown && PlayerNear())
        {
            Attack();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerCollider = other;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == playerCollider)
        {
            playerCollider = null;
        }
    }

    private bool PlayerNear()
    {
        return playerCollider != null;
    }

    private void Attack()
    {
        timeSinceAttack = 0;
        state.ManageAttackAnimationAndSound(attackAction);
    }

    private void DamagePlayer()
    {
        if (!state.isAttacking)
        {
            return;
        }
        if (playerCollider != null)
        {
            if (playerCollider.transform == null)
            {
                playerCollider = null;
                return;
            }
            playerCollider.transform.GetComponent<PlayerState>().TakeDamage(damage);
        }
    }
}
