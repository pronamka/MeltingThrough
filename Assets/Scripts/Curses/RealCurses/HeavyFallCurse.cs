using Unity.VisualScripting;
using UnityEngine;

public class HeavyFallCurse : AbstractCurse
{
    private const float intensityModifier = 0.3f;

    private PlayerState playerState;
    private float initialFallDamageMultiplier;

    public override string Name => "Gravity";
    public override string Description => "Increases fall damage.";

    public override CurseType Type => CurseType.HeavyFall;

    public override void Activate()
    {
        playerState = Player.GetComponent<PlayerState>();
        initialFallDamageMultiplier = playerState.fallDamageMultiplier;
        playerState.fallDamageMultiplier *= (1 - Intensity * intensityModifier);
    }

    public override void Deactivate()
    {
        playerState.fallDamageMultiplier = initialFallDamageMultiplier;
    }
}