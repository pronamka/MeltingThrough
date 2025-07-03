using UnityEngine;

[CreateAssetMenu(fileName = "Blind Eye", menuName = "Game/Curses/Visual/Blind Eye")]
public class BlindEye : CurseData
{
    [Header("Blur Settings")]
    public float blurIntensity = 0.2f; 
    public float blurEdgeWidth = 0.3f; 
    public AnimationCurve blurFalloff = AnimationCurve.EaseInOut(0, 1, 1, 0);

    public override void ApplyVisualEffect(VisualEffectManager visualManager)
    {
        visualManager.ApplyEdgeBlur(blurIntensity, blurEdgeWidth, blurFalloff);
    }

    public override void RemoveVisualEffect(VisualEffectManager visualManager)
    {
        visualManager.RemoveEdgeBlur();
    }
}