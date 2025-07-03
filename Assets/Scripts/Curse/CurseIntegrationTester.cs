using UnityEngine;
using System.Collections;

[System.Serializable]
public class CurseTestData
{
    public string testName;
    public CurseData curseData;
    public float testDuration = 3f;
    public bool testVisuals = true;
    public bool testStats = true;
}

public class CurseIntegrationTester : MonoBehaviour
{
    [Header("Integration Test Settings")]
    public bool autoRunTests = false;
    public float testInterval = 5f;
    public KeyCode manualTestKey = KeyCode.T;

    [Header("Test Cases")]
    public CurseTestData[] testCases;

    private int currentTestIndex = 0;
    private bool isRunningTests = false;
    private PlayerController testPlayer;
    private VisualEffectManager testVisualManager;

    private void Start()
    {
        InitializeTestEnvironment();
        
        if (autoRunTests)
        {
            StartCoroutine(RunAllTests());
        }
    }

    private void InitializeTestEnvironment()
    {
        // Find required components
        testPlayer = FindObjectOfType<PlayerController>();
        testVisualManager = FindObjectOfType<VisualEffectManager>();

        if (testPlayer == null)
        {
            Debug.LogWarning("[CurseIntegrationTester] PlayerController not found! Creating test player...");
            CreateTestPlayer();
        }

        if (testVisualManager == null)
        {
            Debug.LogWarning("[CurseIntegrationTester] VisualEffectManager not found! Some visual tests may fail.");
        }

        Debug.Log($"[CurseIntegrationTester] Initialized with {testCases?.Length ?? 0} test cases");
    }

    private void CreateTestPlayer()
    {
        GameObject playerObj = new GameObject("TestPlayer");
        testPlayer = playerObj.AddComponent<PlayerController>();
        
        // Set default values
        testPlayer.maxHealth = 100f;
        testPlayer.currentHealth = 100f;
        testPlayer.moveSpeed = 5f;
        testPlayer.damage = 10f;
        testPlayer.defense = 0f;
        
        Debug.Log("[CurseIntegrationTester] Created test player with default stats");
    }

    private void Update()
    {
        if (Input.GetKeyDown(manualTestKey) && !isRunningTests)
        {
            StartCoroutine(RunSingleTest());
        }
    }

    private IEnumerator RunAllTests()
    {
        if (testCases == null || testCases.Length == 0)
        {
            Debug.LogWarning("[CurseIntegrationTester] No test cases configured!");
            yield break;
        }

        isRunningTests = true;
        Debug.Log("[CurseIntegrationTester] Starting automated curse integration tests...");

        for (int i = 0; i < testCases.Length; i++)
        {
            currentTestIndex = i;
            yield return StartCoroutine(RunTestCase(testCases[i]));
            yield return new WaitForSeconds(testInterval);
        }

        isRunningTests = false;
        Debug.Log("[CurseIntegrationTester] All tests completed!");
    }

    private IEnumerator RunSingleTest()
    {
        if (testCases == null || testCases.Length == 0)
        {
            Debug.LogWarning("[CurseIntegrationTester] No test cases configured!");
            yield break;
        }

        isRunningTests = true;
        yield return StartCoroutine(RunTestCase(testCases[currentTestIndex]));
        
        currentTestIndex = (currentTestIndex + 1) % testCases.Length;
        isRunningTests = false;
    }

    private IEnumerator RunTestCase(CurseTestData testCase)
    {
        if (testCase.curseData == null)
        {
            Debug.LogError($"[CurseIntegrationTester] Test case '{testCase.testName}' has null curse data!");
            yield break;
        }

        Debug.Log($"[CurseIntegrationTester] Running test: {testCase.testName}");

        // Record initial stats
        var initialStats = RecordPlayerStats();

        // Apply curse through CurseManager
        if (CurseManager.Instance != null)
        {
            CurseManager.Instance.ApplyCurse(testCase.curseData);
            Debug.Log($"[CurseIntegrationTester] Applied curse: {testCase.curseData.curseName}");
        }
        else
        {
            Debug.LogError("[CurseIntegrationTester] CurseManager instance not found!");
            yield break;
        }

        // Wait for test duration
        yield return new WaitForSeconds(testCase.testDuration);

        // Verify effects
        if (testCase.testStats)
        {
            var currentStats = RecordPlayerStats();
            VerifyStatChanges(initialStats, currentStats, testCase);
        }

        // Remove curse
        CurseManager.Instance.RemoveCurse(testCase.curseData);
        Debug.Log($"[CurseIntegrationTester] Removed curse: {testCase.curseData.curseName}");

        // Wait a bit more to verify removal
        yield return new WaitForSeconds(1f);

        // Verify stats are restored
        if (testCase.testStats)
        {
            var restoredStats = RecordPlayerStats();
            VerifyStatRestoration(initialStats, restoredStats, testCase);
        }
    }

    private PlayerStats RecordPlayerStats()
    {
        if (testPlayer == null) return new PlayerStats();
        
        return new PlayerStats
        {
            maxHealth = testPlayer.maxHealth,
            currentHealth = testPlayer.currentHealth,
            moveSpeed = testPlayer.moveSpeed,
            damage = testPlayer.damage,
            defense = testPlayer.defense
        };
    }

    private void VerifyStatChanges(PlayerStats initial, PlayerStats current, CurseTestData testCase)
    {
        bool hasChanges = false;

        if (current.maxHealth != initial.maxHealth)
        {
            Debug.Log($"[CurseIntegrationTester] Max Health: {initial.maxHealth} -> {current.maxHealth}");
            hasChanges = true;
        }

        if (current.moveSpeed != initial.moveSpeed)
        {
            Debug.Log($"[CurseIntegrationTester] Move Speed: {initial.moveSpeed} -> {current.moveSpeed}");
            hasChanges = true;
        }

        if (current.damage != initial.damage)
        {
            Debug.Log($"[CurseIntegrationTester] Damage: {initial.damage} -> {current.damage}");
            hasChanges = true;
        }

        if (current.defense != initial.defense)
        {
            Debug.Log($"[CurseIntegrationTester] Defense: {initial.defense} -> {current.defense}");
            hasChanges = true;
        }

        if (hasChanges)
        {
            Debug.Log($"[CurseIntegrationTester] ✓ {testCase.testName} - Stat changes detected");
        }
        else
        {
            Debug.LogWarning($"[CurseIntegrationTester] ⚠ {testCase.testName} - No stat changes detected!");
        }
    }

    private void VerifyStatRestoration(PlayerStats initial, PlayerStats restored, CurseTestData testCase)
    {
        bool isRestored = true;

        if (Mathf.Abs(restored.maxHealth - initial.maxHealth) > 0.01f)
        {
            Debug.LogWarning($"[CurseIntegrationTester] Max Health not restored: {initial.maxHealth} vs {restored.maxHealth}");
            isRestored = false;
        }

        if (Mathf.Abs(restored.moveSpeed - initial.moveSpeed) > 0.01f)
        {
            Debug.LogWarning($"[CurseIntegrationTester] Move Speed not restored: {initial.moveSpeed} vs {restored.moveSpeed}");
            isRestored = false;
        }

        if (Mathf.Abs(restored.damage - initial.damage) > 0.01f)
        {
            Debug.LogWarning($"[CurseIntegrationTester] Damage not restored: {initial.damage} vs {restored.damage}");
            isRestored = false;
        }

        if (Mathf.Abs(restored.defense - initial.defense) > 0.01f)
        {
            Debug.LogWarning($"[CurseIntegrationTester] Defense not restored: {initial.defense} vs {restored.defense}");
            isRestored = false;
        }

        if (isRestored)
        {
            Debug.Log($"[CurseIntegrationTester] ✓ {testCase.testName} - Stats successfully restored");
        }
        else
        {
            Debug.LogError($"[CurseIntegrationTester] ✗ {testCase.testName} - Stats not properly restored!");
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, Screen.height - 150, 400, 140));
        
        GUILayout.Label("=== CURSE INTEGRATION TESTER ===");
        GUILayout.Label($"Test Cases: {testCases?.Length ?? 0}");
        GUILayout.Label($"Running Tests: {isRunningTests}");
        GUILayout.Label($"Current Test: {currentTestIndex + 1}/{testCases?.Length ?? 0}");
        
        if (testCases != null && currentTestIndex < testCases.Length)
        {
            GUILayout.Label($"Next: {testCases[currentTestIndex].testName}");
        }
        
        GUILayout.Label($"Manual Test Key: {manualTestKey}");
        
        GUILayout.EndArea();
    }

    [System.Serializable]
    public class PlayerStats
    {
        public float maxHealth;
        public float currentHealth;
        public float moveSpeed;
        public float damage;
        public float defense;
    }
}