using UnityEngine;
using System.Collections;

public class CurseSystemTester : MonoBehaviour
{
    [Header("Test Settings")]
    public bool enableTesting = true;
    public KeyCode testKey = KeyCode.C;

    [Header("Test Curses")]
    public WeakenedCurse weakenedCurse;
    public SlowCurse slowCurse;
    public BluntedCurse bluntedCurse;
    public DefenseBreakCurse defenseBreakCurse;
    public RedVisionCurse redVisionCurse;
    public DarkVisionCurse darkVisionCurse;
    public NightmareCurse nightmareCurse;

    private int currentTestIndex = 0;
    private CurseData[] testCurses;

    private void Start()
    {
        if (!enableTesting) return;

        // Initialize test curses array
        testCurses = new CurseData[]
        {
            weakenedCurse,
            slowCurse,
            bluntedCurse,
            defenseBreakCurse,
            redVisionCurse,
            darkVisionCurse,
            nightmareCurse
        };

        Debug.Log("[CurseSystemTester] Initialized with " + testCurses.Length + " test curses");
        Debug.Log("[CurseSystemTester] Press " + testKey + " to cycle through curse tests");
    }

    private void Update()
    {
        if (!enableTesting) return;

        if (Input.GetKeyDown(testKey))
        {
            TestNextCurse();
        }
    }

    private void TestNextCurse()
    {
        if (testCurses == null || testCurses.Length == 0)
        {
            Debug.LogWarning("[CurseSystemTester] No test curses configured!");
            return;
        }

        CurseData curseToTest = testCurses[currentTestIndex];
        if (curseToTest == null)
        {
            Debug.LogWarning($"[CurseSystemTester] Test curse at index {currentTestIndex} is null!");
            currentTestIndex = (currentTestIndex + 1) % testCurses.Length;
            return;
        }

        Debug.Log($"[CurseSystemTester] Testing curse: {curseToTest.curseName}");

        // Apply curse through CurseManager
        if (CurseManager.Instance != null)
        {
            CurseManager.Instance.ApplyCurse(curseToTest);
            
            // Set up removal after 5 seconds
            StartCoroutine(RemoveCurseAfterDelay(curseToTest, 5f));
        }
        else
        {
            Debug.LogError("[CurseSystemTester] CurseManager instance not found!");
        }

        currentTestIndex = (currentTestIndex + 1) % testCurses.Length;
    }

    private IEnumerator RemoveCurseAfterDelay(CurseData curse, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (CurseManager.Instance != null)
        {
            CurseManager.Instance.RemoveCurse(curse);
            Debug.Log($"[CurseSystemTester] Removed curse: {curse.curseName}");
        }
    }

    private void OnGUI()
    {
        if (!enableTesting) return;

        GUILayout.BeginArea(new Rect(Screen.width - 300, 10, 290, 200));
        GUILayout.Label("=== CURSE SYSTEM TESTER ===");
        GUILayout.Label($"Press {testKey} to test curses");
        GUILayout.Label($"Current test: {currentTestIndex + 1}/{testCurses?.Length ?? 0}");
        
        if (testCurses != null && currentTestIndex < testCurses.Length)
        {
            CurseData nextCurse = testCurses[currentTestIndex];
            GUILayout.Label($"Next: {(nextCurse != null ? nextCurse.curseName : "NULL")}");
        }

        GUILayout.EndArea();
    }
}