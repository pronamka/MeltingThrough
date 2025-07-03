using UnityEngine;

[CreateAssetMenu(fileName = "Weakened Curse", menuName = "Game/Curses/Stat/Weakened")]
public class WeakenedCurse : CurseData
{
    [Header("Health Reduction Settings")]
    public float healthReduction = 20f;

    private void OnEnable()
    {
        category = CurseCategory.Stat;
        curseType = CurseType.Health;
    }

    public override void ApplyEffect(PlayerController player)
    {
        if (player == null) return;
        
        // Reduce max health
        player.ModifyMaxHealth(-healthReduction);
        Debug.Log($"[WeakenedCurse] Applied: Reduced max health by {healthReduction}");
    }

    public override void RemoveEffect(PlayerController player)
    {
        if (player == null) return;
        
        // Restore max health
        player.ModifyMaxHealth(healthReduction);
        Debug.Log($"[WeakenedCurse] Removed: Restored max health by {healthReduction}");
    }
}