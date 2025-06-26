using Unity.VisualScripting;
using UnityEngine;

public class ShortJumpCurse : AbstractCurse
{
    private const float intensityModifier = 0.3f;

    private PlayerMovement playerMovement;
    private float initialJumpForce;

    public override string Name => "Weak Jump";
    public override string Description => "Reduces jump strength.";

    public override CurseType Type => CurseType.ShortJump;

    public override void Activate()
    {
        playerMovement = Player.GetComponent<PlayerMovement>();
        initialJumpForce = playerMovement.jumpForce;
        playerMovement.jumpForce *= (1 - Intensity * intensityModifier);
    }

    public override void Deactivate()
    {
        playerMovement.jumpForce = initialJumpForce;
    }
}