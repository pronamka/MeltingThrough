using UnityEngine;

[CreateAssetMenu(fileName = "Defense Break Curse", menuName = "Game/Curses/Stat/Defense Break")]
public class DefenseBreakCurse : CurseData
{
    [Header("Defense Reduction Settings")]
    public float defenseReduction = 5f;

    private void OnEnable()
    {
        category = CurseCategory.Stat;
        curseType = CurseType.Defense;
    }

    public override void ApplyEffect(PlayerController player)
    {
        if (player == null) return;
        
        // Reduce defense
        player.ModifyDefense(-defenseReduction);
        Debug.Log($"[DefenseBreakCurse] Applied: Reduced defense by {defenseReduction}");
    }

    public override void RemoveEffect(PlayerController player)
    {
        if (player == null) return;
        
        // Restore defense
        player.ModifyDefense(defenseReduction);
        Debug.Log($"[DefenseBreakCurse] Removed: Restored defense by {defenseReduction}");
    }
}