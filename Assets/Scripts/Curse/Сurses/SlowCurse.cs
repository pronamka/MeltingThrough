using UnityEngine;

[CreateAssetMenu(fileName = "Slow Curse", menuName = "Game/Curses/Stat/Slow")]
public class SlowCurse : CurseData
{
    [Header("Speed Reduction Settings")]
    public float speedReduction = 2f;

    private void OnEnable()
    {
        category = CurseCategory.Stat;
        curseType = CurseType.Speed;
    }

    public override void ApplyEffect(PlayerController player)
    {
        if (player == null) return;
        
        // Reduce movement speed
        player.ModifySpeed(-speedReduction);
        Debug.Log($"[SlowCurse] Applied: Reduced speed by {speedReduction}");
    }

    public override void RemoveEffect(PlayerController player)
    {
        if (player == null) return;
        
        // Restore movement speed
        player.ModifySpeed(speedReduction);
        Debug.Log($"[SlowCurse] Removed: Restored speed by {speedReduction}");
    }
}