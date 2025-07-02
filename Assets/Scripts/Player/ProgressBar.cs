using UnityEngine;
using UnityEngine.UI;

public class LevelProgressBar : MonoBehaviour
{
    [SerializeField] private LevelProgress levelProgress;
    [SerializeField] private Image emptyProgressBar;
    [SerializeField] private Image fullProgressBar;

    private void Update()
    {
        if (levelProgress != null && fullProgressBar != null)
        {
            fullProgressBar.fillAmount = levelProgress.GetProgressPercentage();
        }
    }
}