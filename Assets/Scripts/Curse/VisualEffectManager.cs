using UnityEngine;

public class VisualEffectManager : MonoBehaviour
{
    private Camera targetCamera;
    private float defaultCameraSize;

    public static VisualEffectManager Instance { get; private set; }

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
        }
    }

    private void Start()
    {
        // Автоматический поиск основной камеры
        targetCamera = Camera.main;

        if (targetCamera != null && targetCamera.orthographic)
        {
            defaultCameraSize = targetCamera.orthographicSize;
            Debug.Log($"Target orthographic camera found: {targetCamera.name} with size: {defaultCameraSize}");
        }
        else
        {
            Debug.LogError("No orthographic main camera found in the scene. Please assign an orthographic camera manually.");
        }
    }

    public void ApplyCameraSize(float newSize)
    {
        if (targetCamera != null && targetCamera.orthographic)
        {
            targetCamera.orthographicSize = newSize;
            Debug.Log($"Camera size changed to: {newSize}");
        }
        else
        {
            Debug.LogError("Target camera is not set or is not orthographic. Cannot change camera size.");
        }
    }

    public void ResetCameraSize()
    {
        if (targetCamera != null && targetCamera.orthographic)
        {
            targetCamera.orthographicSize = defaultCameraSize;
            Debug.Log("Camera size reset to default.");
        }
        else
        {
            Debug.LogError("Target camera is not set or is not orthographic. Cannot reset camera size.");
        }
    }
}