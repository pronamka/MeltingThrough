using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public class CurseManager : MonoBehaviour
{
    private GameObject player;

    [Header("Settings")]
    [SerializeField] private Transform targetColumn;
    [Range(0f, 1f)]
    public float curseDropChance = 0.3f;
    [SerializeField] private CurseData[] availableCurses;

    [Header("References")]
    public Transform curseUIParent;
    public GameObject curseUIPrefab;

    private List<ActiveCurse> activeCurses = new List<ActiveCurse>();
    private List<CurseData> spawnedCurses = new List<CurseData>();

    [SerializeField] private float alterAccessDistance;

    public static CurseManager Instance { get; private set; }

    public System.Action<CurseData> OnCurseApplied;
    public System.Action<CurseData> OnCurseRemoved;

    private InputAction interactAction;

    [System.Serializable]
    public class ActiveCurse
    {
        public CurseData curseData;
        public float remainingTime;
        public int stackCount;

        public ActiveCurse(CurseData curse)
        {
            curseData = curse;
            remainingTime = curse.duration;
            stackCount = 1;
        }
    }

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        interactAction = InputSystem.actions.FindAction("Interact");
        interactAction.Enable();
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
    }

    private void Start()
    {
        ValidateSetup();
    }

    private void Update()
    {
        UpdateCurseTimers();
        if (Input.GetKeyDown(KeyCode.T))
        {
            Vector3 dropPos = Vector3.zero;
            TryDropCurse(dropPos + Vector3.up * 2f);
        }

        if (interactAction.triggered)
        {
            RemoveAllCurses();
        }
    }

    private void CreateCurseUIPanel()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        GameObject panel = new GameObject("CursesPanel");
        panel.transform.SetParent(canvas.transform, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(10, -10);
        rect.sizeDelta = new Vector2(300, 400);

        UnityEngine.UI.VerticalLayoutGroup layout = panel.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
        layout.spacing = 5;
        layout.padding = new RectOffset(5, 5, 5, 5);

        curseUIParent = panel.transform;
    }

    private void ValidateSetup()
    {
        if (availableCurses == null || availableCurses.Length == 0) { }
    }

    public void TryDropCurse(Vector3 position)
    {
        if (availableCurses == null || availableCurses.Length == 0)
        {
            return;
        }

        if (Random.value <= curseDropChance)
        {
            DropRandomCurse(position);
        }
    }

    public void DropRandomCurse(Vector3 position)
    {
        if (availableCurses == null || availableCurses.Length == 0)
        {
            return;
        }

        var availableForDrop = availableCurses
            .Where(c => c != null && c.dropPrefab != null && !spawnedCurses.Contains(c))
            .ToArray();

        if (availableForDrop.Length == 0)
        {
            return;
        }

        CurseData randomCurse = availableForDrop[Random.Range(0, availableForDrop.Length)];
        DropSpecificCurse(randomCurse, position);

        spawnedCurses.Add(randomCurse);
    }

    public void ResetSpawnedCurses()
    {
        spawnedCurses.Clear();
        CursePickup[] cursePickups = FindObjectsOfType<CursePickup>();
        foreach (var cursePickup in cursePickups)
        {
            Destroy(cursePickup.gameObject);
        }
    }

    public void DropSpecificCurse(CurseData curse, Vector3 position)
    {
        if (curse == null)
        {
            return;
        }

        if (curse.dropPrefab == null)
        {
            return;
        }

        GameObject drop = Instantiate(curse.dropPrefab, position, Quaternion.identity);
        CursePickup pickup = drop.GetComponent<CursePickup>();

        if (pickup != null)
        {
            pickup.Initialize(curse);
        }
        else
        {
            Destroy(drop);
        }
    }

    public void ApplyCurse(CurseData curse)
    {
        if (curse == null)
        {
            return;
        }

        ActiveCurse existingCurse = activeCurses.FirstOrDefault(ac => ac.curseData == curse);

        if (existingCurse != null)
        {
            if (curse.stackable)
            {
                existingCurse.stackCount++;
                ApplyCurseEffect(curse);
            }
            else
            {
                existingCurse.remainingTime = curse.duration;
            }
        }
        else
        {
            activeCurses.Add(new ActiveCurse(curse));
            ApplyCurseEffect(curse);
        }

        UpdateCurseUI();
        OnCurseApplied?.Invoke(curse);
    }

    private void ApplyCurseEffect(CurseData curse)
    {
        try
        {
            switch (curse.category)
            {
                case CurseCategory.Stat:
                    curse.ApplyEffect();
                    break;
                case CurseCategory.Visual:
                    VisualEffectManager visualManager = FindObjectOfType<VisualEffectManager>();
                    if (visualManager != null)
                        curse.ApplyVisualEffect(visualManager);
                    break;
                // UIManager code removed
                case CurseCategory.Gameplay:
                    curse.ApplyEffect();
                    break;
                case CurseCategory.Special:
                    curse.ApplyEffect();
                    VisualEffectManager vm = FindObjectOfType<VisualEffectManager>();
                    if (vm != null) curse.ApplyVisualEffect(vm);
                    // UIManager code removed
                    break;
            }
        }
        catch (System.Exception) { }
    }

    public void RemoveCurse(CurseData curse)
    {
        ActiveCurse activeCurse = activeCurses.FirstOrDefault(ac => ac.curseData == curse);
        if (activeCurse == null)
        {
            return;
        }

        if (curse.stackable && activeCurse.stackCount > 1)
        {
            activeCurse.stackCount--;
            RemoveCurseEffect(curse);
        }
        else
        {
            RemoveCurseEffect(curse);

            if (curse.stackable)
            {
                for (int i = 1; i < activeCurse.stackCount; i++)
                {
                    RemoveCurseEffect(curse);
                }
            }

            activeCurses.Remove(activeCurse);
        }

        UpdateCurseUI();
        OnCurseRemoved?.Invoke(curse);
    }

    private void RemoveCurseEffect(CurseData curse)
    {
        try
        {
            switch (curse.category)
            {
                case CurseCategory.Stat:
                    curse.RemoveEffect();
                    break;
                case CurseCategory.Visual:
                    VisualEffectManager visualManager = FindObjectOfType<VisualEffectManager>();
                    if (visualManager != null)
                        curse.RemoveVisualEffect(visualManager);
                    break;
                // UIManager code removed
                case CurseCategory.Gameplay:
                    curse.RemoveEffect();
                    break;
                case CurseCategory.Special:
                    curse.RemoveEffect();

                    VisualEffectManager vm = FindObjectOfType<VisualEffectManager>();
                    if (vm != null) curse.RemoveVisualEffect(vm);

                    // UIManager code removed
                    break;
            }
        }
        catch (System.Exception) { }
    }

    private void UpdateCurseTimers()
    {
        List<ActiveCurse> cursesToRemove = new List<ActiveCurse>();

        foreach (var activeCurse in activeCurses)
        {
            if (activeCurse.curseData.duration > 0)
            {
                activeCurse.remainingTime -= Time.deltaTime;

                if (activeCurse.remainingTime <= 0)
                {
                    cursesToRemove.Add(activeCurse);
                }
            }
        }

        foreach (var curse in cursesToRemove)
        {
            RemoveCurse(curse.curseData);
        }
    }

    private void UpdateCurseUI()
    {
        ClearColumn();
        foreach (var activeCurse in activeCurses)
        {
            AddCurseSpriteToColumn(activeCurse);
        }
    }

    private void AddCurseSpriteToColumn(ActiveCurse activeCurse)
    {
        if (targetColumn == null)
        {
            return;
        }

        if (activeCurse.curseData.curseSprite == null)
        {
            return;
        }

        GameObject curseSpriteObject = new GameObject($"Curse_{activeCurse.curseData.curseName}");
        curseSpriteObject.transform.SetParent(targetColumn, false);

        UnityEngine.UI.Image imageComponent = curseSpriteObject.AddComponent<UnityEngine.UI.Image>();
        imageComponent.sprite = activeCurse.curseData.curseSprite;
        imageComponent.preserveAspect = true;

        RectTransform rectTransform = curseSpriteObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(75f, 75f);

        UnityEngine.UI.LayoutElement layoutElement = curseSpriteObject.AddComponent<UnityEngine.UI.LayoutElement>();
        layoutElement.preferredWidth = 75f;
        layoutElement.preferredHeight = 75f;
    }

    private void ClearColumn()
    {
        if (targetColumn == null) return;

        for (int i = targetColumn.childCount - 1; i >= 0; i--)
        {
            Transform child = targetColumn.GetChild(i);
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    public List<CurseData> GetActiveCurses()
    {
        return activeCurses.Select(ac => ac.curseData).ToList();
    }

    public bool IsCurseActive(CurseData curse)
    {
        return activeCurses.Any(ac => ac.curseData == curse);
    }

    public void RemoveAllCurses()
    {
        GameObject[] alters = GameObject.FindGameObjectsWithTag("Alter");

        List<float> distances = new List<float>();
        for (int i = 0; i < alters.Length; i++)
        {
            distances.Add(Vector3.Distance(player.transform.position, alters[i].transform.position));
        }

        float minDistance = distances.Min();

        if (minDistance < alterAccessDistance)
        {
            var cursesToRemove = activeCurses.ToList();
            foreach (var curse in cursesToRemove)
            {
                RemoveCurse(curse.curseData);
            }

            foreach (var curse in cursesToRemove)
            {
                GiveBuff();
                player.GetComponent<PlayerHealth>().Heal(40);
            }
        }

        ResetSpawnedCurses();
    }

    public void GiveBuff()
    {
        int buffIndex = Random.Range(0, 6);
        switch (buffIndex)
        {
            case 0:
                player.GetComponent<Health>().maxHealth *= 1.2f;
                break;
            case 1:
                player.GetComponent<PlayerMana>().maxMana *= 1.2f;
                break;
            case 2:
                player.GetComponent<PlayerMana>().regenerationRate *= 1.1f;
                break;
            case 3:
                player.GetComponent<PlayerAttack>().primaryAttackDamage *= 1.2f;
                break;
            case 4:
                player.GetComponent<PlayerMovement>().speed *= 1.1f;
                break;
            case 5:
                player.GetComponent<PlayerMovement>().maxJumps += 1;
                break;
        }
    }

    public int GetActiveCursesCount()
    {
        return activeCurses.Count;
    }
}