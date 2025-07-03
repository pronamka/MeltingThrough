using UnityEngine;

[CreateAssetMenu(fileName = "Red Vision Curse", menuName = "Game/Curses/Visual/Red Vision")]
public class RedVisionCurse : CurseData
{
    [Header("Red Filter Settings")]
    public Color redFilterColor = Color.red;
    public float filterIntensity = 0.3f;

    private void OnEnable()
    {
        category = CurseCategory.Visual;
        curseType = CurseType.ColorFilter;
    }

    public override void ApplyVisualEffect(VisualEffectManager visualManager)
    {
        if (visualManager == null) return;
        
        visualManager.ApplyColorFilter(redFilterColor, filterIntensity);
        Debug.Log($"[RedVisionCurse] Applied: Red color filter with intensity {filterIntensity}");
    }

    public override void RemoveVisualEffect(VisualEffectManager visualManager)
    {
        if (visualManager == null) return;
        
        visualManager.ApplyColorFilter(Color.white, 0f);
        Debug.Log($"[RedVisionCurse] Removed: Red color filter");
    }
}