using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class ElementalState : EnemyState
{
    public override void Update()
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
        animator.SetBool("walking", IsMoving());
    }

    public override bool CanPrimaryAttack()
    {
        return !isAttacking && !IsDead();
    }
}
