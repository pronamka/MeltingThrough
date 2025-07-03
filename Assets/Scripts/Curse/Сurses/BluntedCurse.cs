using UnityEngine;

[CreateAssetMenu(fileName = "Blunted Curse", menuName = "Game/Curses/Stat/Blunted")]
public class BluntedCurse : CurseData
{
    [Header("Damage Reduction Settings")]
    public float damageReduction = 3f;

    private void OnEnable()
    {
        category = CurseCategory.Stat;
        curseType = CurseType.Damage;
    }

    public override void ApplyEffect(PlayerController player)
    {
        if (player == null) return;
        
        // Reduce damage output
        player.ModifyDamage(-damageReduction);
        Debug.Log($"[BluntedCurse] Applied: Reduced damage by {damageReduction}");
    }

    public override void RemoveEffect(PlayerController player)
    {
        if (player == null) return;
        
        // Restore damage output
        player.ModifyDamage(damageReduction);
        Debug.Log($"[BluntedCurse] Removed: Restored damage by {damageReduction}");
    }
}