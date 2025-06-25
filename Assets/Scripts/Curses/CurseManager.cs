using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CurseManager : MonoBehaviour
{
    [SerializeField] private List<Curse> activeCurses = new List<Curse>();
    [SerializeField] private float maxCurseValue = 10f;

    
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private Health playerHealth;
    private PlayerMana playerMana;
    private PlayerState playerState;

    
    private InputAction exchangeCursesAction;

    
    private float speedModifier = 1f;
    private float jumpModifier = 1f;
    private float damageModifier = 1f;
    private float healthModifier = 1f;
    private float manaRegenModifier = 1f;
    private float fallDamageModifier = 1f;
    private float attackSpeedModifier = 1f;

    
    public System.Action<List<Curse>> OnCursesChanged;
    public System.Action<int> OnRelicsAvailable;

    public static CurseManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

       
        FindPlayerComponents();

       
        SetupInput();
    }

    private void Start()
    {
        Debug.Log("CurseManager �������. ������� ������ ���������: M");
    }

    private void Update()
    {
      
        if (exchangeCursesAction != null && exchangeCursesAction.triggered)
        {
            ExchangeCursesForRelics();
        }
    }

    private void SetupInput()
    {
     
        exchangeCursesAction = new InputAction("ExchangeCurses", binding: "<Keyboard>/m");
        exchangeCursesAction.Enable();

        Debug.Log("�������� input ��� ������ ��������� (������� M)");
    }

    private void FindPlayerComponents()
    {
     
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            playerAttack = player.GetComponent<PlayerAttack>();
            playerHealth = player.GetComponent<Health>();
            playerMana = player.GetComponent<PlayerMana>();
            playerState = player.GetComponent<PlayerState>();

            Debug.Log($"������� ���������� ������: Movement={playerMovement != null}, Attack={playerAttack != null}, Health={playerHealth != null}, Mana={playerMana != null}, State={playerState != null}");
        }
        else
        {
            Debug.LogWarning("����� � ����� 'Player' �� ������!");
        }
    }

    public void AddCurse(CurseType type, float value)
    {
        string name = GetCurseName(type);
        string description = GetCurseDescription(type);

        Curse newCurse = new Curse(type, value, CalculateIntensity(value), name, description);
        activeCurses.Add(newCurse);

        ApplyCurse(newCurse);

        
        OnCursesChanged?.Invoke(activeCurses);
        OnRelicsAvailable?.Invoke(GetRelicsToReceive());

        Debug.Log($"��������� ���������: {name} (��������: {value}, �������������: {newCurse.intensity:F2})");
    }

    private string GetCurseName(CurseType type)
    {
        switch (type)
        {
            case CurseType.SlowMovement: return "��������� ����������";
            case CurseType.WeakAttack: return "��������� ��������";
            case CurseType.LowHealth: return "��������� ���������";
            case CurseType.SlowMana: return "��������� ���������";
            case CurseType.HeavyFall: return "��������� �������";
            case CurseType.ShortJump: return "��������� ������� ������";
            default: return "����������� ���������";
        }
    }

    private string GetCurseDescription(CurseType type)
    {
        switch (type)
        {
            case CurseType.SlowMovement: return "������� �������� ������������";
            case CurseType.WeakAttack: return "������� ���� �����";
            case CurseType.LowHealth: return "������� ������������ ��������";
            case CurseType.SlowMana: return "��������� ����������� ����";
            case CurseType.HeavyFall: return "����������� ���� �� �������";
            case CurseType.ShortJump: return "������� ���� ������";
            default: return "����������� ������";
        }
    }

    private float CalculateIntensity(float value)
    {
        return Mathf.Clamp(value / 2f, 0.1f, 1f);
    }

    private void ApplyCurse(Curse curse)
    {
        switch (curse.type)
        {
            case CurseType.SlowMovement:
                speedModifier -= curse.intensity * 0.3f;
                break;
            case CurseType.WeakAttack:
                damageModifier -= curse.intensity * 0.25f;
                break;
            case CurseType.LowHealth:
                healthModifier -= curse.intensity * 0.2f;
                break;
            case CurseType.SlowMana:
                manaRegenModifier -= curse.intensity * 0.3f;
                break;
            case CurseType.HeavyFall:
                fallDamageModifier += curse.intensity * 0.5f;
                break;
            case CurseType.ShortJump:
                jumpModifier -= curse.intensity * 0.2f;
                break;
        }

        ClampModifiers();
        Debug.Log($"��������� ��������� {curse.name}. ������������: ��������={speedModifier:F2}, ������={jumpModifier:F2}, ����={damageModifier:F2}");
    }

    private void ClampModifiers()
    {
        speedModifier = Mathf.Clamp(speedModifier, 0.1f, 2f);
        jumpModifier = Mathf.Clamp(jumpModifier, 0.1f, 2f);
        damageModifier = Mathf.Clamp(damageModifier, 0.1f, 2f);
        healthModifier = Mathf.Clamp(healthModifier, 0.1f, 2f);
        manaRegenModifier = Mathf.Clamp(manaRegenModifier, 0.1f, 2f);
        fallDamageModifier = Mathf.Clamp(fallDamageModifier, 0.5f, 5f);
    }

    public float GetTotalCurseValue()
    {
        float total = 0f;
        foreach (var curse in activeCurses)
        {
            total += curse.value;
        }
        return total;
    }

    public int GetRelicsToReceive()
    {
        return Mathf.FloorToInt(GetTotalCurseValue());
    }

    public void ExchangeCursesForRelics()
    {
        int relicsCount = GetRelicsToReceive();
        if (relicsCount <= 0)
        {
            Debug.Log("������������ ��������� ��� ������");
            return;
        }

        Debug.Log($"���������� {activeCurses.Count} ��������� (����� ��������: {GetTotalCurseValue():F1}) �� {relicsCount} ��������");

        ClearAllCurses();
        /*
        if (RelicManager.Instance != null)
        {
            RelicManager.Instance.AddRandomRelics(relicsCount);
        }
        else
        {
            Debug.LogWarning("RelicManager �� ������!");
        }
        */
        Debug.Log($"�������� ��������� �� {relicsCount} ��������!");
    }

    private void ClearAllCurses()
    {
        activeCurses.Clear();
        ResetModifiers();
        OnCursesChanged?.Invoke(activeCurses);
        OnRelicsAvailable?.Invoke(0);
    }

    private void ResetModifiers()
    {
        speedModifier = 1f;
        jumpModifier = 1f;
        damageModifier = 1f;
        healthModifier = 1f;
        manaRegenModifier = 1f;
        fallDamageModifier = 1f;
        attackSpeedModifier = 1f;

        Debug.Log("��� ��������� �����, ��������� �������������");
    }

    public List<Curse> GetActiveCurses()
    {
        return new List<Curse>(activeCurses);
    }

    
    public float GetSpeedModifier() => speedModifier;
    public float GetJumpModifier() => jumpModifier;
    public float GetDamageModifier() => damageModifier;
    public float GetHealthModifier() => healthModifier;
    public float GetManaRegenModifier() => manaRegenModifier;
    public float GetFallDamageModifier() => fallDamageModifier;
    public float GetAttackSpeedModifier() => attackSpeedModifier;

    public void RefreshPlayerComponents()
    {
        FindPlayerComponents();
    }

    private void OnDestroy()
    {
        if (exchangeCursesAction != null)
        {
            exchangeCursesAction.Disable();
            exchangeCursesAction.Dispose();
        }
    }

    
    [ContextMenu("�������� �������� ���������")]
    public void AddTestCurse()
    {
        CurseType[] types = (CurseType[])System.Enum.GetValues(typeof(CurseType));
        CurseType randomType = types[Random.Range(0, types.Length)];
        float randomValue = Random.Range(0.3f, 2f);
        AddCurse(randomType, randomValue);
    }

    [ContextMenu("�������� ��� ���������")]
    public void ClearAllCursesDebug()
    {
        ClearAllCurses();
    }

    [ContextMenu("�������� ���������� � ����������")]
    public void ShowCursesInfo()
    {
        Debug.Log($"�������� ���������: {activeCurses.Count}");
        Debug.Log($"����� ��������: {GetTotalCurseValue():F1}");
        Debug.Log($"�������� � ���������: {GetRelicsToReceive()}");

        foreach (var curse in activeCurses)
        {
            Debug.Log($"- {curse.name}: {curse.value:F1} (�������������: {curse.intensity:F2})");
        }
    }
}