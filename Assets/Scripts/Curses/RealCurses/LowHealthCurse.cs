using UnityEngine;

public class LowHealthCurse : AbstractCurse
{
    private const float intensityModifier = 0.2f;

    private Health playerHealth;
    private float initialHealth;

    public override string Name => "Fragility";
    public override string Description => "Reduces maximum health.";

    public override CurseType Type => CurseType.LowHealth;

    public override void Activate()
    {
        playerHealth = Player.GetComponent<Health>();
        initialHealth = playerHealth.maxHealth;
        playerHealth.maxHealth *= (1 - Intensity * intensityModifier);
    }

    public override void Deactivate()
    {
        playerHealth.maxHealth = initialHealth;
    }
}