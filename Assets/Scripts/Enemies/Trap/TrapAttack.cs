using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class TrapAttack : MonoBehaviour
{
    [SerializeField] private float attackDistance;
    [SerializeField] private float damage;
    [SerializeField] private float cooldown;
    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private bool facingLeft = true;
    [SerializeField] private Transform firePoint;

    [SerializeField] private AudioClip attackSound;

    private EnemyState state;

    private AttackAction attackAction;

    private float timeSinceAttack = 0f;

    private Collider2D playerCollider;

    [SerializeField] private GameObject beamPrefab;

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

        if (!facingLeft) transform.localScale = new Vector3(-1, 1, 1) * state.scaleValue;
        else transform.localScale = new Vector3(1, 1, 1) * state.scaleValue;
    }

    private void Update()
    {
        timeSinceAttack += Time.deltaTime;
        if (state.CanPrimaryAttack() && timeSinceAttack > cooldown)
        {
            RaycastHit2D hit = CheckForPlayer();

            if (hit.collider != null)
            {
                Attack();
            }
        }
    }

    private RaycastHit2D CheckForPlayer()
    {
        Vector2 direction = facingLeft ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, attackDistance, playerLayer);

        return hit;
    }

    /*private void OnTriggerEnter2D(Collider2D other)
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
    }*/

    private void Attack()
    {
        timeSinceAttack = 0;
        state.ManageAttackAnimationAndSound(attackAction);
    }

    private void SpawnBeam()
    {
        GameObject beam = Instantiate(beamPrefab, firePoint.position, Quaternion.identity);
        if (facingLeft)
        {
            Vector3 scale = beam.transform.localScale;
            scale.x = -scale.x;
            beam.transform.localScale = scale;
        }
        DamagePlayer();
    }

    private void DamagePlayer()
    {
        RaycastHit2D hit = CheckForPlayer();
        if (hit.collider == null || hit.collider.transform == null) return;
        hit.collider.transform.GetComponent<PlayerState>().TakeDamage(damage);
    }
}
