using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CurseManager : MonoBehaviour
{
    [SerializeField] private List<AbstractCurse> activeCurses = new List<AbstractCurse>();
    [SerializeField] private float maxCurseValue = 10f;

    private InputAction exchangeCursesAction;


    public System.Action<List<AbstractCurse>> OnCursesChanged;
    public System.Action<int> OnRelicsAvailable;

    public static CurseManager Instance { get; private set; }

    private GameObject player;
    private CurseFactory curseFactory;

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

        player = GameObject.FindGameObjectWithTag("Player");
        curseFactory = new CurseFactory(player);

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

    public void AddCurse(CurseType type, float value)
    {
        AbstractCurse newCurse = curseFactory.createCurse(type, 1);

        activeCurses.Add(newCurse);

        newCurse.Activate();


        OnCursesChanged?.Invoke(activeCurses);
        OnRelicsAvailable?.Invoke(GetRelicsToReceive());

        Debug.Log($"��������� ���������: {name} (��������: {value}, �������������: {newCurse.Intensity:F2})");
    }

    public void DeactivateAll()
    {
        for (int i = activeCurses.Count; i >= 0; i--)
        {
            activeCurses[i].Deactivate();
        }
    }

    public float GetTotalCurseValue()
    {
        float total = 0f;
        foreach (var curse in activeCurses)
        {
            total += curse.Value;
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
        DeactivateAll();
        activeCurses.Clear();
        OnCursesChanged?.Invoke(activeCurses);
        OnRelicsAvailable?.Invoke(0);
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

    /*[ContextMenu("�������� ���������� � ����������")]
    public void ShowCursesInfo()
    {
        Debug.Log($"�������� ���������: {activeCurses.Count}");
        Debug.Log($"����� ��������: {GetTotalCurseValue():F1}");
        Debug.Log($"�������� � ���������: {GetRelicsToReceive()}");

        foreach (var curse in activeCurses)
        {
            Debug.Log($"- {curse.name}: {curse.value:F1} (�������������: {curse.intensity:F2})");
        }
    }*/
}