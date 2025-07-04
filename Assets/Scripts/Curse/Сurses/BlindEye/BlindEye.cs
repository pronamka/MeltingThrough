using UnityEngine;

[CreateAssetMenu(fileName = "Blind Eye", menuName = "Game/Curses/Visual/Blind Eye")]
public class BlindEye : CurseData
{
    [Header("Camera Size Settings")]
    public float cameraSize = 7f; 

    public override void ApplyVisualEffect(VisualEffectManager visualManager)
    {
        if (visualManager == null)
        {
            Debug.LogError("VisualEffectManager is null. Cannot apply visual effect.");
            return;
        }

        Debug.Log($"Applying Blind Eye curse with camera size: {cameraSize}.");
        visualManager.ApplyCameraSize(cameraSize);
    }

    public override void RemoveVisualEffect(VisualEffectManager visualManager)
    {
        if (visualManager == null)
        {
            Debug.LogError("VisualEffectManager is null. Cannot remove visual effect.");
            return;
        }

        Debug.Log("Removing Blind Eye curse effects.");
        visualManager.ResetCameraSize();
    }
}