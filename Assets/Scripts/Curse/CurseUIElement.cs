using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurseUIElement : MonoBehaviour
{
    [Header("UI Components")]
    public Image backgroundImage;
    public Image curseIcon;
    public TextMeshProUGUI curseNameText;
    public TextMeshProUGUI curseDescriptionText;
    public TextMeshProUGUI stackText;
    public TextMeshProUGUI timerText;
    public Button infoButton;

    private CurseData curseData;

    private void Start()
    {
        
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();

        if (curseIcon == null)
            curseIcon = transform.Find("Icon")?.GetComponent<Image>();

        if (curseNameText == null)
            curseNameText = transform.Find("Name")?.GetComponent<TextMeshProUGUI>();

        if (curseDescriptionText == null)
            curseDescriptionText = transform.Find("Description")?.GetComponent<TextMeshProUGUI>();

        if (stackText == null)
            stackText = transform.Find("Stack")?.GetComponent<TextMeshProUGUI>();

        if (timerText == null)
            timerText = transform.Find("Timer")?.GetComponent<TextMeshProUGUI>();

        if (infoButton == null)
            infoButton = transform.Find("InfoButton")?.GetComponent<Button>();
    }

    public void Setup(CurseData curse, int stackCount = 1, float remainingTime = -1f)
    {
        curseData = curse;

        
        if (curseIcon != null)
        {
            if (curse.curseSprite != null)
            {
                curseIcon.sprite = curse.curseSprite;
            }
            curseIcon.color = curse.uiColor;
        }

        
        if (curseNameText != null)
        {
            curseNameText.text = curse.curseName;
        }

        if (curseDescriptionText != null)
        {
            curseDescriptionText.text = curse.description;
        }

        
        if (stackText != null)
        {
            if (stackCount > 1)
            {
                stackText.text = $"x{stackCount}";
                stackText.gameObject.SetActive(true);
            }
            else
            {
                stackText.gameObject.SetActive(false);
            }
        }

        
        if (timerText != null)
        {
            if (remainingTime > 0)
            {
                timerText.text = $"{remainingTime:F1}s";
                timerText.gameObject.SetActive(true);
            }
            else
            {
                timerText.gameObject.SetActive(false);
            }
        }

        
        if (infoButton != null)
        {
            infoButton.onClick.RemoveAllListeners();
            infoButton.onClick.AddListener(ShowDetailedInfo);
        }

        
        if (backgroundImage != null)
        {
            backgroundImage.color = new Color(curse.uiColor.r, curse.uiColor.g, curse.uiColor.b, 0.3f);
        }
    }

    private void ShowDetailedInfo()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowCurseDetails(curseData);
        }
    }
}