using UnityEngine;

public class BossState : EnemyState
{
    public override void Update()
    {
        if (IsDead())
        {
            return;
        }

        //timeSinceCollisionDamage += Time.deltaTime;
        if (isAttacking)
        {
            attackTimeLeft -= Time.deltaTime;
            if (attackTimeLeft <= 0)
            {
                isAttacking = false;
                attackTimeLeft = 0;
            }
        }
        animator.SetBool("grounded", IsGrounded());
    }

    public override void TakeDamage(float damage)
    {
        if (IsDead()) return;
        if (isAttacking)
        {
            health.TakeDamage(damage, false);
            return;
        }
        health.TakeDamage(damage);
    }
}
