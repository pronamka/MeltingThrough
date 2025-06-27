using UnityEngine;

public class ManaRegenerationCurse : AbstractCurse
{
    private const float intensityModifier = 0.3f;

    private PlayerMana playerMana;
    private float initialManaRegenerationRate;

    public override string Name => "Exhaustion";
    public override string Description => "Slows mana regeneration.";

    public override CurseType Type => CurseType.SlowMana;

    public override void Activate()
    {
        playerMana = Player.GetComponent<PlayerMana>();
        initialManaRegenerationRate = playerMana.regenerationRate;
        playerMana.regenerationRate *= (1 - Intensity * intensityModifier);
    }

    public override void Deactivate()
    {
        playerMana.regenerationRate = initialManaRegenerationRate;
    }
}