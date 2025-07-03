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
    public float duration = -1f; // -1 для постоянного эффекта
    public bool stackable = false;

    [Header("Visual")]
    public Color uiColor = Color.white;
    public AudioClip pickupSound;

    // Виртуальные методы для разных типов эффектов
    public virtual void ApplyEffect(PlayerController player)
    {
        Debug.Log($"[CurseData] Applying {curseName} stat effect to player");
    }

    public virtual void RemoveEffect(PlayerController player)
    {
        Debug.Log($"[CurseData] Removing {curseName} stat effect from player");
    }

    // Методы для визуальных эффектов
    public virtual void ApplyVisualEffect(VisualEffectManager visualManager)
    {
        Debug.Log($"[CurseData] Applying {curseName} visual effect");
    }

    public virtual void RemoveVisualEffect(VisualEffectManager visualManager)
    {
        Debug.Log($"[CurseData] Removing {curseName} visual effect");
    }

    // Методы для UI эффектов
    public virtual void ApplyUIEffect(UIManager uiManager)
    {
        Debug.Log($"[CurseData] Applying {curseName} UI effect");
    }

    public virtual void RemoveUIEffect(UIManager uiManager)
    {
        Debug.Log($"[CurseData] Removing {curseName} UI effect");
    }
}