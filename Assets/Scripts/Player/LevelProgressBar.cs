using UnityEngine;
using UnityEngine.UI;

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