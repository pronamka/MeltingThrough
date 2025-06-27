using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class TrapState : EnemyState
{
    public virtual void Update()
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
    }

    public override bool CanPrimaryAttack()
    {
        return !isAttacking && !IsDead();
    }
}
