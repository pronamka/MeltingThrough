using UnityEngine;

/// <summary>
/// Tracks the player's level progress based on vertical position relative to level start/end points.
/// Calculates progress as: (startY - currentPlayerY) / (startY - endY)
/// Returns a value between 0 (at start) and 1 (at end).
/// </summary>
public class LevelProgress : MonoBehaviour
{
    [SerializeField] private LevelGenerator levelGenerator;
    [SerializeField] private Transform player;

    private void Start()
    {
        // Find LevelGenerator if not assigned
        if (levelGenerator == null)
        {
            levelGenerator = FindFirstObjectByType<LevelGenerator>();
            if (levelGenerator == null)
            {
                Debug.LogWarning("LevelProgress: No LevelGenerator found in scene!");
            }
        }
        
        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogWarning("LevelProgress: No GameObject with 'Player' tag found!");
            }
        }
    }

    public float GetProgressPercentage()
    {
        if (levelGenerator == null || player == null)
        {
            return 0f;
        }

        float currentPlayerY = player.position.y;
        float platformStartY = levelGenerator.PlatformStartY;
        float platformEndY = levelGenerator.PlatformEndY;
        
        // Calculate progress: (startY - currentPlayerY) / (startY - endY)
        float totalDistance = platformStartY - platformEndY;
        
        if (totalDistance <= 0)
        {
            return 0f;
        }
        
        float playerProgress = platformStartY - currentPlayerY;
        
        // Clamp between 0 and 1
        float progressPercentage = Mathf.Clamp01(playerProgress / totalDistance);
        
        return progressPercentage;
    }

    // Public properties for debugging
    public float PlatformStartY => levelGenerator != null ? levelGenerator.PlatformStartY : 0f;
    public float PlatformEndY => levelGenerator != null ? levelGenerator.PlatformEndY : 0f;
    public float CurrentPlayerY => player != null ? player.position.y : 0f;
}