using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [Header("Drop Settings")]
    public bool canDropCurse = true;
    [Range(0f, 1f)]
    public float dropChanceOverride = -1f; // -1 = использовать настройки CurseManager

    private void OnDestroy()
    {
        DropCurseOnDeath();
    }

    // Вызывайте этот метод при смерти врага
    public void OnDeath()
    {
        DropCurseOnDeath();
    }

    private void DropCurseOnDeath()
    {
        if (!canDropCurse) return;

        if (CurseManager.Instance == null) return;

        // Используем собственный шанс дропа или общий
        if (dropChanceOverride >= 0)
        {
            if (Random.value <= dropChanceOverride)
            {
                CurseManager.Instance.DropRandomCurse(transform.position);
            }
        }
        else
        {
            CurseManager.Instance.TryDropCurse(transform.position);
        }
    }

    // Для тестирования
    [ContextMenu("Test Drop Curse")]
    public void TestDropCurse()
    {
        OnDeath();
    }
}