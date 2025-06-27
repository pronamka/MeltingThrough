using UnityEngine;

public class WeaknessCurse : AbstractCurse
{
    private const float intensityModifier = 0.2f;

    private PlayerAttack playerAttack;
    private float initialDamage;

    public override string Name => "Weakness";
    public override string Description => "Reduces attack damage.";

    public override CurseType Type => CurseType.WeakAttack;

    public override void Activate()
    {
        playerAttack = Player.GetComponent<PlayerAttack>();
        initialDamage = playerAttack.primaryAttackDamage;
        playerAttack.primaryAttackDamage *= (1 - Intensity * intensityModifier);
    }

    public override void Deactivate()
    {
        playerAttack.primaryAttackDamage = initialDamage;
    }
}