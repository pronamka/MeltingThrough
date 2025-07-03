using UnityEngine;

[CreateAssetMenu(fileName = "Nightmare Curse", menuName = "Game/Curses/Special/Nightmare")]
public class NightmareCurse : CurseData
{
    [Header("Multiple Effects")]
    public float healthReduction = 10f;
    public float speedReduction = 1f;
    public Color nightmareColor = new Color(0.5f, 0.1f, 0.5f, 1f); // Dark purple
    public float colorIntensity = 0.4f;
    public float vignetteIntensity = 0.3f;

    private void OnEnable()
    {
        category = CurseCategory.Special;
        curseType = CurseType.Composite;
    }

    public override void ApplyEffect(PlayerController player)
    {
        if (player == null) return;
        
        // Apply stat debuffs
        player.ModifyMaxHealth(-healthReduction);
        player.ModifySpeed(-speedReduction);
        Debug.Log($"[NightmareCurse] Applied: Reduced health by {healthReduction}, speed by {speedReduction}");
    }

    public override void RemoveEffect(PlayerController player)
    {
        if (player == null) return;
        
        // Remove stat debuffs
        player.ModifyMaxHealth(healthReduction);
        player.ModifySpeed(speedReduction);
        Debug.Log($"[NightmareCurse] Removed: Restored health by {healthReduction}, speed by {speedReduction}");
    }

    public override void ApplyVisualEffect(VisualEffectManager visualManager)
    {
        if (visualManager == null) return;
        
        // Apply visual effects
        visualManager.ApplyColorFilter(nightmareColor, colorIntensity);
        visualManager.ApplyVignette(vignetteIntensity, 0.4f);
        Debug.Log($"[NightmareCurse] Applied: Nightmare visual effects");
    }

    public override void RemoveVisualEffect(VisualEffectManager visualManager)
    {
        if (visualManager == null) return;
        
        // Remove visual effects
        visualManager.ApplyColorFilter(Color.white, 0f);
        visualManager.ApplyVignette(0f, 0f);
        Debug.Log($"[NightmareCurse] Removed: Nightmare visual effects");
    }
}