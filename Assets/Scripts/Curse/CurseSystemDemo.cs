using UnityEngine;

/// <summary>
/// Демонстрационный скрипт для показа работы системы проклятий
/// </summary>
public class CurseSystemDemo : MonoBehaviour
{
    [Header("Demo Settings")]
    public bool showInstructions = true;
    public KeyCode[] testKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7 };
    
    [Header("Demo Curses - Assign ScriptableObjects")]
    public CurseData[] demoCurses;
    
    [Header("Demo Player Stats Display")]
    public bool showPlayerStats = true;
    
    private PlayerController player;
    private string instructions = "";
    
    private void Start()
    {
        // Найти игрока
        player = FindObjectOfType<PlayerController>();
        
        if (player == null)
        {
            Debug.LogWarning("[CurseSystemDemo] PlayerController not found! Creating demo player...");
            CreateDemoPlayer();
        }
        
        // Подготовить инструкции
        PrepareInstructions();
        
        Debug.Log("[CurseSystemDemo] Curse system demo initialized!");
        Debug.Log("[CurseSystemDemo] " + instructions);
    }
    
    private void CreateDemoPlayer()
    {
        GameObject playerObj = new GameObject("DemoPlayer");
        player = playerObj.AddComponent<PlayerController>();
        
        // Установить базовые значения
        player.maxHealth = 100f;
        player.currentHealth = 100f;
        player.moveSpeed = 5f;
        player.damage = 10f;
        player.defense = 5f;
        
        Debug.Log("[CurseSystemDemo] Created demo player with base stats");
    }
    
    private void PrepareInstructions()
    {
        instructions = "Curse System Demo Instructions:\n";
        
        for (int i = 0; i < testKeys.Length && i < demoCurses.Length; i++)
        {
            if (demoCurses[i] != null)
            {
                instructions += $"Press {testKeys[i]} - {demoCurses[i].curseName}\n";
            }
        }
        
        instructions += "Press R - Remove all curses\n";
        instructions += "Press I - Show/hide player stats";
    }
    
    private void Update()
    {
        // Обработка клавиш для тестирования проклятий
        for (int i = 0; i < testKeys.Length && i < demoCurses.Length; i++)
        {
            if (Input.GetKeyDown(testKeys[i]))
            {
                ApplyDemoCurse(i);
            }
        }
        
        // Удаление всех проклятий
        if (Input.GetKeyDown(KeyCode.R))
        {
            RemoveAllCurses();
        }
        
        // Переключение отображения статистики
        if (Input.GetKeyDown(KeyCode.I))
        {
            showPlayerStats = !showPlayerStats;
        }
    }
    
    private void ApplyDemoCurse(int index)
    {
        if (demoCurses == null || index >= demoCurses.Length || demoCurses[index] == null)
        {
            Debug.LogWarning($"[CurseSystemDemo] Demo curse at index {index} is null!");
            return;
        }
        
        CurseData curse = demoCurses[index];
        
        if (CurseManager.Instance != null)
        {
            CurseManager.Instance.ApplyCurse(curse);
            Debug.Log($"[CurseSystemDemo] Applied curse: {curse.curseName}");
        }
        else
        {
            Debug.LogError("[CurseSystemDemo] CurseManager instance not found!");
        }
    }
    
    private void RemoveAllCurses()
    {
        if (CurseManager.Instance != null)
        {
            CurseManager.Instance.RemoveAllCurses();
            Debug.Log("[CurseSystemDemo] Removed all curses");
        }
        else
        {
            Debug.LogError("[CurseSystemDemo] CurseManager instance not found!");
        }
    }
    
    private void OnGUI()
    {
        if (showInstructions)
        {
            // Инструкции
            GUI.Box(new Rect(10, 10, 350, 200), "");
            GUI.Label(new Rect(20, 20, 330, 180), instructions);
        }
        
        if (showPlayerStats && player != null)
        {
            // Статистика игрока
            float startY = showInstructions ? 220 : 10;
            GUI.Box(new Rect(10, startY, 300, 130), "");
            
            string statsText = "=== PLAYER STATS ===\n";
            statsText += $"Max Health: {player.maxHealth:F1}\n";
            statsText += $"Current Health: {player.currentHealth:F1}\n";
            statsText += $"Move Speed: {player.moveSpeed:F1}\n";
            statsText += $"Damage: {player.damage:F1}\n";
            statsText += $"Defense: {player.defense:F1}";
            
            GUI.Label(new Rect(20, startY + 10, 280, 110), statsText);
        }
        
        // Информация о активных проклятиях
        if (CurseManager.Instance != null)
        {
            float startY = showInstructions ? (showPlayerStats ? 360 : 220) : (showPlayerStats ? 150 : 10);
            var activeCurses = CurseManager.Instance.GetActiveCurses();
            
            if (activeCurses.Count > 0)
            {
                GUI.Box(new Rect(10, startY, 300, 20 + activeCurses.Count * 20), "");
                GUI.Label(new Rect(20, startY + 5, 280, 20), "=== ACTIVE CURSES ===");
                
                for (int i = 0; i < activeCurses.Count; i++)
                {
                    GUI.Label(new Rect(20, startY + 25 + i * 20, 280, 20), $"• {activeCurses[i].curseName}");
                }
            }
        }
    }
}