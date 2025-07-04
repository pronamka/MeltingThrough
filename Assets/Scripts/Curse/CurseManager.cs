using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CurseManager : MonoBehaviour
{
    [Header("Settings")]
    [Range(0f, 1f)]
    public float curseDropChance = 0.3f;
    [SerializeField] private CurseData[] availableCurses;

    [Header("References")]
    public Transform curseUIParent;
    public GameObject curseUIPrefab;

    [Header("Debug")]
    [SerializeField] private bool debugMode = true;
    [SerializeField] private KeyCode testDropKey = KeyCode.T;

    private List<ActiveCurse> activeCurses = new List<ActiveCurse>();
    private PlayerController player;

    
    public static CurseManager Instance { get; private set; }

    public System.Action<CurseData> OnCurseApplied;
    public System.Action<CurseData> OnCurseRemoved;

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
        InitializeReferences();
        ValidateSetup();

        if (debugMode)
        {
            Debug.Log($"[CurseManager] Initialized by {System.Environment.UserName} at {System.DateTime.Now:HH:mm:ss}");
        }
    }

    private void Update()
    {
        UpdateCurseTimers();

        
        if (debugMode && Input.GetKeyDown(testDropKey))
        {
            Vector3 dropPos = player != null ? player.transform.position : Vector3.zero;
            TryDropCurse(dropPos + Vector3.up * 2f);
        }
    }

    private void InitializeReferences()
    {
     
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
        }

     
        if (curseUIParent == null)
        {
            CreateCurseUIPanel();
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

        if (debugMode)
        {
            Debug.Log("[CurseManager] Auto-created CursesPanel");
        }
    }

    private void ValidateSetup()
    {
        List<string> warnings = new List<string>();

        if (availableCurses == null || availableCurses.Length == 0)
        {
            warnings.Add("No curses assigned to CurseManager!");
        }

        if (player == null)
        {
            warnings.Add("PlayerController not found in scene!");
        }

        if (warnings.Count > 0 && debugMode)
        {
            Debug.LogWarning($"[CurseManager] Setup Warnings:\n{string.Join("\n", warnings)}");
        }

        if (debugMode)
        {
            Debug.Log($"[CurseManager] Setup completed with {availableCurses?.Length ?? 0} available curses");
        }
    }

    
    public void TryDropCurse(Vector3 position)
    {
        if (availableCurses == null || availableCurses.Length == 0)
        {
            if (debugMode) Debug.LogWarning("[CurseManager] No curses available for drop!");
            return;
        }

        if (Random.value <= curseDropChance)
        {
            DropRandomCurse(position);
        }
        else if (debugMode)
        {
            Debug.Log($"[CurseManager] Curse drop failed (chance: {curseDropChance:P0})");
        }
    }

    
    public void DropRandomCurse(Vector3 position)
    {
        if (availableCurses == null || availableCurses.Length == 0)
        {
            if (debugMode) Debug.LogWarning("[CurseManager] No curses available for random drop!");
            return;
        }

      
        var availableForDrop = availableCurses.Where(c => c != null && c.dropPrefab != null).ToArray();

        if (availableForDrop.Length == 0)
        {
            if (debugMode) Debug.LogWarning("[CurseManager] No curses have drop prefabs assigned!");
            return;
        }

      
        CurseData randomCurse = availableForDrop[Random.Range(0, availableForDrop.Length)];
        DropSpecificCurse(randomCurse, position);
    }

   
    public void DropSpecificCurse(CurseData curse, Vector3 position)
    {
        if (curse == null)
        {
            if (debugMode) Debug.LogError("[CurseManager] Trying to drop null curse!");
            return;
        }

        if (curse.dropPrefab == null)
        {
            if (debugMode) Debug.LogError($"[CurseManager] Curse '{curse.curseName}' has no drop prefab assigned!");
            return;
        }

        
        GameObject drop = Instantiate(curse.dropPrefab, position, Quaternion.identity);
        CursePickup pickup = drop.GetComponent<CursePickup>();

        if (pickup != null)
        {
            pickup.Initialize(curse);
            if (debugMode) Debug.Log($"[CurseManager] Dropped curse: {curse.curseName}");
        }
        else
        {
            Debug.LogError($"[CurseManager] Drop prefab for '{curse.curseName}' doesn't have CursePickup component!");
            Destroy(drop);
        }
    }

   
    public void ApplyCurse(CurseData curse)
    {
        if (curse == null)
        {
            if (debugMode) Debug.LogError("[CurseManager] Trying to apply null curse!");
            return;
        }

        /*if (player == null)
        {
            if (debugMode) Debug.LogError("[CurseManager] Player not found when applying curse!");
            return;
        }*/

        ActiveCurse existingCurse = activeCurses.FirstOrDefault(ac => ac.curseData == curse);

        if (existingCurse != null)
        {
            Debug.Log("1");
            if (curse.stackable)
            {
                existingCurse.stackCount++;
                ApplyCurseEffect(curse);
                if (debugMode) Debug.Log($"[CurseManager] Stacked curse '{curse.curseName}', count: {existingCurse.stackCount}");
            }
            else
            {
                existingCurse.remainingTime = curse.duration;
                if (debugMode) Debug.Log($"[CurseManager] Refreshed curse '{curse.curseName}'");
            }
        }
        else
        {
            Debug.Log("2");

            activeCurses.Add(new ActiveCurse(curse));
            ApplyCurseEffect(curse);
            if (debugMode) Debug.Log($"[CurseManager] Applied new curse: {curse.curseName}");
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
                    curse.ApplyEffect(player);
                    break;
                case CurseCategory.Visual:
                    
                    VisualEffectManager visualManager = FindObjectOfType<VisualEffectManager>();
                    if (visualManager != null)
                        curse.ApplyVisualEffect(visualManager);
                    break;
                case CurseCategory.UI:
                    
                    UIManager uiManager = FindObjectOfType<UIManager>();
                    if (uiManager != null)
                        curse.ApplyUIEffect(uiManager);
                    break;
                case CurseCategory.Gameplay:
                    curse.ApplyEffect(player);
                    break;
                case CurseCategory.Special:
                    
                    curse.ApplyEffect(player);

                    VisualEffectManager vm = FindObjectOfType<VisualEffectManager>();
                    if (vm != null) curse.ApplyVisualEffect(vm);

                    UIManager um = FindObjectOfType<UIManager>();
                    if (um != null) curse.ApplyUIEffect(um);
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[CurseManager] Error applying curse effect '{curse.curseName}': {e.Message}");
        }
    }

    public void RemoveCurse(CurseData curse)
    {
        ActiveCurse activeCurse = activeCurses.FirstOrDefault(ac => ac.curseData == curse);
        if (activeCurse == null)
        {
            if (debugMode) Debug.LogWarning($"[CurseManager] Trying to remove non-active curse: {curse.curseName}");
            return;
        }

        if (curse.stackable && activeCurse.stackCount > 1)
        {
            activeCurse.stackCount--;
            RemoveCurseEffect(curse);
            if (debugMode) Debug.Log($"[CurseManager] Reduced curse stack '{curse.curseName}', count: {activeCurse.stackCount}");
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

            if (debugMode) Debug.Log($"[CurseManager] Removed curse: {curse.curseName}");
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
                    curse.RemoveEffect(player);
                    break;
                case CurseCategory.Visual:
                    VisualEffectManager visualManager = FindObjectOfType<VisualEffectManager>();
                    if (visualManager != null)
                        curse.RemoveVisualEffect(visualManager);
                    break;
                case CurseCategory.UI:
                    UIManager uiManager = FindObjectOfType<UIManager>();
                    if (uiManager != null)
                        curse.RemoveUIEffect(uiManager);
                    break;
                case CurseCategory.Gameplay:
                    curse.RemoveEffect(player);
                    break;
                case CurseCategory.Special:
                    curse.RemoveEffect(player);

                    VisualEffectManager vm = FindObjectOfType<VisualEffectManager>();
                    if (vm != null) curse.RemoveVisualEffect(vm);

                    UIManager um = FindObjectOfType<UIManager>();
                    if (um != null) curse.RemoveUIEffect(um);
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[CurseManager] Error removing curse effect '{curse.curseName}': {e.Message}");
        }
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
        if (curseUIParent == null || curseUIPrefab == null)
        {
            return;
        }

        
        foreach (Transform child in curseUIParent)
        {
            Destroy(child.gameObject);
        }

        
        foreach (var activeCurse in activeCurses)
        {
            GameObject uiElement = Instantiate(curseUIPrefab, curseUIParent);
            CurseUIElement curseUI = uiElement.GetComponent<CurseUIElement>();

            if (curseUI != null)
            {
                curseUI.Setup(activeCurse.curseData, activeCurse.stackCount, activeCurse.remainingTime);
            }
        }
    }

    #region Public API

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
        var cursesToRemove = activeCurses.ToList();
        foreach (var curse in cursesToRemove)
        {
            RemoveCurse(curse.curseData);
        }

        if (debugMode) Debug.Log("[CurseManager] Removed all curses");
    }

    public int GetActiveCursesCount()
    {
        return activeCurses.Count;
    }

    #endregion

    #region Debug Methods

    [ContextMenu("Drop Random Curse")]
    public void DebugDropRandomCurse()
    {
        if (player != null)
        {
            TryDropCurse(player.transform.position + Vector3.up * 2f);
        }
        else
        {
            TryDropCurse(Vector3.zero);
        }
    }

    [ContextMenu("Remove All Curses")]
    public void DebugRemoveAllCurses()
    {
        RemoveAllCurses();
    }

    private void OnGUI()
    {
        if (!debugMode) return;

        GUILayout.BeginArea(new Rect(10, 10, 350, 400));

        GUILayout.Label("=== CURSE MANAGER DEBUG ===");
        GUILayout.Label($"User: ObjoradDdd | Time: {System.DateTime.Now:HH:mm:ss}");
        GUILayout.Label($"Active Curses: {activeCurses.Count}");
        GUILayout.Label($"Available Curses: {availableCurses?.Length ?? 0}");
        GUILayout.Label($"Drop Chance: {curseDropChance:P0}");
        GUILayout.Label($"Player Found: {player != null}");

        GUILayout.Space(5);

        if (GUILayout.Button("Drop Random Curse"))
        {
            DebugDropRandomCurse();
        }

        if (GUILayout.Button("Remove All Curses"))
        {
            DebugRemoveAllCurses();
        }

        GUILayout.Space(10);
        GUILayout.Label("=== ACTIVE CURSES ===");

        foreach (var curse in activeCurses)
        {
            string info = $"{curse.curseData.curseName}";
            if (curse.stackCount > 1) info += $" x{curse.stackCount}";
            if (curse.remainingTime > 0) info += $" ({curse.remainingTime:F1}s)";
            GUILayout.Label(info);
        }

        GUILayout.EndArea();
    }

    #endregion
}