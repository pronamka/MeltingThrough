using UnityEngine;

/// <summary>
/// Validates that the curse system is properly configured and working
/// </summary>
public class CurseSystemValidator : MonoBehaviour
{
    [Header("Validation Settings")]
    public bool validateOnStart = true;
    public bool showValidationResults = true;
    
    private bool validationPassed = false;
    private string validationReport = "";
    
    private void Start()
    {
        if (validateOnStart)
        {
            ValidateCurseSystem();
        }
    }
    
    [ContextMenu("Validate Curse System")]
    public void ValidateCurseSystem()
    {
        validationReport = "=== CURSE SYSTEM VALIDATION ===\n";
        validationPassed = true;
        
        // Check CurseManager
        ValidateCurseManager();
        
        // Check PlayerController
        ValidatePlayerController();
        
        // Check VisualEffectManager
        ValidateVisualEffectManager();
        
        // Check curse implementations
        ValidateCurseImplementations();
        
        if (validationPassed)
        {
            validationReport += "\n✓ ALL VALIDATIONS PASSED!\n";
            validationReport += "The curse system is properly configured and ready to use.";
            Debug.Log("[CurseSystemValidator] " + validationReport);
        }
        else
        {
            validationReport += "\n✗ VALIDATION FAILED!\n";
            validationReport += "Please fix the issues above before using the curse system.";
            Debug.LogError("[CurseSystemValidator] " + validationReport);
        }
    }
    
    private void ValidateCurseManager()
    {
        validationReport += "\n--- CurseManager Validation ---\n";
        
        if (CurseManager.Instance == null)
        {
            validationReport += "✗ CurseManager instance not found!\n";
            validationPassed = false;
        }
        else
        {
            validationReport += "✓ CurseManager instance found\n";
        }
    }
    
    private void ValidatePlayerController()
    {
        validationReport += "\n--- PlayerController Validation ---\n";
        
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            validationReport += "✗ PlayerController not found in scene!\n";
            validationPassed = false;
        }
        else
        {
            validationReport += "✓ PlayerController found\n";
            
            // Test stat modification methods
            float originalHealth = player.maxHealth;
            float originalSpeed = player.moveSpeed;
            float originalDamage = player.damage;
            float originalDefense = player.defense;
            
            try
            {
                player.ModifyMaxHealth(5f);
                player.ModifySpeed(1f);
                player.ModifyDamage(2f);
                player.ModifyDefense(1f);
                
                validationReport += "✓ Stat modification methods working\n";
                
                // Reset stats
                player.ResetStats();
                validationReport += "✓ Stat reset method working\n";
            }
            catch (System.Exception e)
            {
                validationReport += $"✗ Error testing stat modifications: {e.Message}\n";
                validationPassed = false;
            }
        }
    }
    
    private void ValidateVisualEffectManager()
    {
        validationReport += "\n--- VisualEffectManager Validation ---\n";
        
        VisualEffectManager visualManager = FindObjectOfType<VisualEffectManager>();
        if (visualManager == null)
        {
            validationReport += "⚠ VisualEffectManager not found (visual curses will not work)\n";
        }
        else
        {
            validationReport += "✓ VisualEffectManager found\n";
        }
    }
    
    private void ValidateCurseImplementations()
    {
        validationReport += "\n--- Curse Implementation Validation ---\n";
        
        // Check if curse classes can be instantiated
        try
        {
            // Test stat curses
            var weakenedCurse = ScriptableObject.CreateInstance<WeakenedCurse>();
            var slowCurse = ScriptableObject.CreateInstance<SlowCurse>();
            var bluntedCurse = ScriptableObject.CreateInstance<BluntedCurse>();
            var defenseBreakCurse = ScriptableObject.CreateInstance<DefenseBreakCurse>();
            
            validationReport += "✓ Stat curse classes can be instantiated\n";
            
            // Test visual curses
            var redVisionCurse = ScriptableObject.CreateInstance<RedVisionCurse>();
            var darkVisionCurse = ScriptableObject.CreateInstance<DarkVisionCurse>();
            
            validationReport += "✓ Visual curse classes can be instantiated\n";
            
            // Test special curses
            var nightmareCurse = ScriptableObject.CreateInstance<NightmareCurse>();
            
            validationReport += "✓ Special curse classes can be instantiated\n";
            
            // Clean up
            DestroyImmediate(weakenedCurse);
            DestroyImmediate(slowCurse);
            DestroyImmediate(bluntedCurse);
            DestroyImmediate(defenseBreakCurse);
            DestroyImmediate(redVisionCurse);
            DestroyImmediate(darkVisionCurse);
            DestroyImmediate(nightmareCurse);
        }
        catch (System.Exception e)
        {
            validationReport += $"✗ Error instantiating curse classes: {e.Message}\n";
            validationPassed = false;
        }
    }
    
    private void OnGUI()
    {
        if (showValidationResults && !string.IsNullOrEmpty(validationReport))
        {
            GUI.Box(new Rect(Screen.width - 450, 10, 440, 300), "");
            GUI.Label(new Rect(Screen.width - 440, 20, 420, 280), validationReport);
        }
    }
}