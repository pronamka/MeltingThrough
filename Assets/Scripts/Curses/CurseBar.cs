using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurseBar : MonoBehaviour
{
    [SerializeField] private Image curseBarFill;
    [SerializeField] private TextMeshProUGUI curseValueText;
    [SerializeField] private TextMeshProUGUI relicsToReceiveText;
    [SerializeField] private float maxDisplayValue = 10f;

    private void Start()
    {
        
        if (curseBarFill == null)
        {
            curseBarFill = GetComponentInChildren<Image>();
        }

        if (curseValueText == null)
        {
            TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length > 0) curseValueText = texts[0];
            if (texts.Length > 1) relicsToReceiveText = texts[1];
        }

        
        if (curseBarFill == null || curseValueText == null || relicsToReceiveText == null)
        {
            CreateCurseBarUI();
        }
    }

    private void Update()
    {
        UpdateCurseBar();
    }

    private void CreateCurseBarUI()
    {
        
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
        }

        
        if (curseBarFill == null)
        {
            GameObject barBg = new GameObject("CurseBarBackground");
            barBg.transform.SetParent(transform);

            Image bgImage = barBg.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            RectTransform bgRect = barBg.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0.02f, 0.85f);
            bgRect.anchorMax = new Vector2(0.3f, 0.9f);
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

        
            GameObject barFill = new GameObject("CurseBarFill");
            barFill.transform.SetParent(barBg.transform);

            curseBarFill = barFill.AddComponent<Image>();
            curseBarFill.color = Color.red;
            curseBarFill.type = Image.Type.Filled;
            curseBarFill.fillMethod = Image.FillMethod.Horizontal;

            RectTransform fillRect = barFill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
        }

        
        if (curseValueText == null)
        {
            GameObject textObj = new GameObject("CurseValueText");
            textObj.transform.SetParent(transform);

            curseValueText = textObj.AddComponent<TextMeshProUGUI>();
            curseValueText.text = "Проклятия: 0.0";
            curseValueText.fontSize = 16;
            curseValueText.color = Color.white;

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.02f, 0.9f);
            textRect.anchorMax = new Vector2(0.3f, 0.95f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }

        
        if (relicsToReceiveText == null)
        {
            GameObject relicsObj = new GameObject("RelicsText");
            relicsObj.transform.SetParent(transform);

            relicsToReceiveText = relicsObj.AddComponent<TextMeshProUGUI>();
            relicsToReceiveText.text = "Реликвий: 0";
            relicsToReceiveText.fontSize = 16;
            relicsToReceiveText.color = Color.yellow;

            RectTransform relicsRect = relicsObj.GetComponent<RectTransform>();
            relicsRect.anchorMin = new Vector2(0.02f, 0.8f);
            relicsRect.anchorMax = new Vector2(0.3f, 0.85f);
            relicsRect.offsetMin = Vector2.zero;
            relicsRect.offsetMax = Vector2.zero;
        }
    }

    private void UpdateCurseBar()
    {
        if (CurseManager.Instance == null) return;

        float currentValue = CurseManager.Instance.GetTotalCurseValue();
        int relicsToReceive = CurseManager.Instance.GetRelicsToReceive();

        
        if (curseBarFill != null)
        {
            curseBarFill.fillAmount = currentValue / maxDisplayValue;

        
            Color barColor = Color.Lerp(Color.yellow, Color.red, currentValue / maxDisplayValue);
            curseBarFill.color = barColor;
        }

        
        if (curseValueText != null)
        {
            curseValueText.text = $"Проклятия: {currentValue:F1}";
        }

        if (relicsToReceiveText != null)
        {
            relicsToReceiveText.text = $"Реликвий: {relicsToReceive} (нажми M)";
        }
    }
}