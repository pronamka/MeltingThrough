using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI component that displays the level progress as a progress bar.
/// Should be positioned on the left side of the screen.
/// Uses Unity's Image fillAmount to show progress similar to health/mana bars.
/// </summary>
public class LevelProgressBar : MonoBehaviour
{
    [SerializeField] private LevelProgress levelProgress;
    [SerializeField] private Image emptyProgress;
    [SerializeField] private Image fullProgress;

    private void Start()
    {
        // Find LevelProgress if not assigned
        if (levelProgress == null)
        {
            levelProgress = FindFirstObjectByType<LevelProgress>();
            if (levelProgress == null)
            {
                Debug.LogWarning("LevelProgressBar: No LevelProgress component found in scene!");
            }
        }
    }

    private void Update()
    {
        if (levelProgress != null && fullProgress != null)
        {
            fullProgress.fillAmount = levelProgress.GetProgressPercentage();
        }
    }
}