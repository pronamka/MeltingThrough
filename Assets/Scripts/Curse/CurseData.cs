using UnityEngine;

[CreateAssetMenu(fileName = "New Curse", menuName = "Game/Curse")]
public class CurseData : ScriptableObject
{
    [Header("Basic Info")]
    public string curseName = "Новое проклятие";
    [TextArea(3, 5)]
    public string description = "Описание эффекта проклятия";
    public Sprite curseSprite;
    public GameObject dropPrefab;

    [Header("Properties")]
    public CurseCategory category = CurseCategory.Stat;
    public CurseType curseType = CurseType.Health;
    public float value = 1f;
    public float duration = -1f; 
    public bool stackable = false;

    [Header("Visual")]
    public Color uiColor = Color.white;
    public AudioClip pickupSound;

    public virtual void ApplyEffect()
    {
        Debug.Log($"[CurseData] Applying {curseName} stat effect to player");
    }

    public virtual void RemoveEffect()
    {
        Debug.Log($"[CurseData] Removing {curseName} stat effect from player");
    }

    
    public virtual void ApplyVisualEffect(VisualEffectManager visualManager)
    {
        Debug.Log($"[CurseData] Applying {curseName} visual effect");
    }

    public virtual void RemoveVisualEffect(VisualEffectManager visualManager)
    {
        Debug.Log($"[CurseData] Removing {curseName} visual effect");
    }

    
    public virtual void ApplyUIEffect(UIManager uiManager)
    {
        Debug.Log($"[CurseData] Applying {curseName} UI effect");
    }

    public virtual void RemoveUIEffect(UIManager uiManager)
    {
        Debug.Log($"[CurseData] Removing {curseName} UI effect");
    }
}