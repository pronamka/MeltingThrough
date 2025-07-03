using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [Header("Drop Settings")]
    public bool canDropCurse = true;
    [Range(0f, 1f)]
    public float dropChanceOverride = -1f; // -1 = ������������ ��������� CurseManager

    private void OnDestroy()
    {
        DropCurseOnDeath();
    }

    // ��������� ���� ����� ��� ������ �����
    public void OnDeath()
    {
        DropCurseOnDeath();
    }

    private void DropCurseOnDeath()
    {
        if (!canDropCurse) return;

        if (CurseManager.Instance == null) return;

        // ���������� ����������� ���� ����� ��� �����
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

    // ��� ������������
    [ContextMenu("Test Drop Curse")]
    public void TestDropCurse()
    {
        OnDeath();
    }
}