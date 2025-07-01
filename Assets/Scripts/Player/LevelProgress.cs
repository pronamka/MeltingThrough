using UnityEngine;

public class LevelProgress : MonoBehaviour
{
    private LevelGenerator levelGenerator;
    private Transform playerTransform;
    private float levelStartY;
    private float levelEndY;

    private void Awake()
    {
        playerTransform = transform;
    }

    private void Start()
    {
        levelGenerator = FindObjectOfType<LevelGenerator>();
        if (levelGenerator != null)
        {
            levelStartY = levelGenerator.PlatformStartY;
            levelEndY = levelGenerator.PlatformEndY;
        }
        else
        {
            levelStartY = 200f;
            levelEndY = -300f;
            Debug.LogWarning("LevelGenerator not found! Using fallback values for progress calculation.");
        }
    }

    public float GetProgressPercentage()
    {
        if (Mathf.Approximately(levelStartY, levelEndY)) return 0f;

        float currentY = playerTransform.position.y;
        float progress = (levelStartY - currentY) / (levelStartY - levelEndY);

        return Mathf.Clamp01(progress);
    }

    public float GetCurrentDepth()
    {
        return levelStartY - playerTransform.position.y;
    }

    public float GetTotalLevelHeight()
    {
        return levelStartY - levelEndY;
    }
}