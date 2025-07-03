using UnityEngine;

[CreateAssetMenu(fileName = "Dark Vision Curse", menuName = "Game/Curses/Visual/Dark Vision")]
public class DarkVisionCurse : CurseData
{
    [Header("Vignette Settings")]
    public float vignetteIntensity = 0.5f;
    public float vignetteSmoothness = 0.4f;

    private void OnEnable()
    {
        category = CurseCategory.Visual;
        curseType = CurseType.Vignette;
    }

    public override void ApplyVisualEffect(VisualEffectManager visualManager)
    {
        if (visualManager == null) return;
        
        visualManager.ApplyVignette(vignetteIntensity, vignetteSmoothness);
        Debug.Log($"[DarkVisionCurse] Applied: Vignette with intensity {vignetteIntensity}");
    }

    public override void RemoveVisualEffect(VisualEffectManager visualManager)
    {
        if (visualManager == null) return;
        
        visualManager.ApplyVignette(0f, 0f);
        Debug.Log($"[DarkVisionCurse] Removed: Vignette effect");
    }
}