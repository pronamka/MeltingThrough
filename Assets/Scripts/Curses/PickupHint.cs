using UnityEngine;
using UnityEngine.UI;

public class SimplePickupHint : MonoBehaviour
{
    [SerializeField] private Text hintText;
    [SerializeField] private Image background;

    private void Start()
    {
        
        if (hintText == null)
        {
            CreateHintUI();
        }

        
        gameObject.SetActive(false);
    }

    private void CreateHintUI()
    {
        
        if (background == null)
        {
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(transform);
            background = bgObj.AddComponent<Image>();
            background.color = new Color(0f, 0f, 0f, 0.7f);

            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            bgRect.anchoredPosition = Vector2.zero;
        }

        
        if (hintText == null)
        {
            GameObject textObj = new GameObject("HintText");
            textObj.transform.SetParent(transform);
            hintText = textObj.AddComponent<Text>();
            hintText.text = "ֽאזלטעו E";
            hintText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            hintText.fontSize = 100;
            hintText.color = Color.white;
            hintText.alignment = TextAnchor.MiddleCenter;

            RectTransform textRect = hintText.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
        }
    }

    public void ShowHint(bool show)
    {
        gameObject.SetActive(show);
    }

    public void SetHintText(string text)
    {
        if (hintText != null)
        {
            hintText.text = text;
        }
    }
}