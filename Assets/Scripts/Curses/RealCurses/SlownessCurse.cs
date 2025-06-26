using Unity.VisualScripting;
using UnityEngine;

public class SlownessCurse : AbstractCurse
{
    private const float intensityModifier = 0.3f;

    private PlayerMovement playerMovement;
    private float initialSpeed;

    public override string Name => "Slowness";
    public override string Description => "Reduces movement speed.";

    public override CurseType Type => CurseType.SlowMovement;

    public override void Activate()
    {
        playerMovement = Player.GetComponent<PlayerMovement>();
        initialSpeed = playerMovement.speed;
        playerMovement.speed *= (1 - Intensity * intensityModifier);
    }

    public override void Deactivate()
    {
        playerMovement.speed = initialSpeed;
    }
}
