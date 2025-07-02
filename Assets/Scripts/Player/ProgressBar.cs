using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressBar : MonoBehaviour
{
    private Transform player;
    [SerializeField] private RectTransform progressBarEmpty;
    [SerializeField] private Image progressBarFull;
    [SerializeField] private RectTransform progressDot;

    [SerializeField] private float levelStartY = 0f;
    [SerializeField] private float levelEndY = -1000f;

    private float barHeight;

    private float originalY;
    private float maxY;


    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalY = progressDot.position.y;
        barHeight = progressBarEmpty.rect.height * progressBarEmpty.lossyScale.y;
        maxY = originalY - barHeight * 0.8f;
    }

    void Update()
    {
        float playerY = Mathf.Clamp(player.position.y, levelEndY, levelStartY);

        float progress = (playerY - levelStartY) / (levelEndY - levelStartY);

        float newY = Mathf.Clamp(originalY - progress * barHeight, maxY, originalY);
        progressDot.position = new Vector3(progressDot.position.x, newY,
            progressDot.position.z);
        
        progressBarFull.fillAmount = progress+0.1f;
        Debug.Log($"{barHeight}; {progress}; {originalY}; {originalY - progress * barHeight}");
    }
}
