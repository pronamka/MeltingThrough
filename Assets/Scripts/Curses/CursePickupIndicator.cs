using UnityEngine;
using TMPro;

public class CursePickupIndicator : MonoBehaviour
{
    [SerializeField] private GameObject textPrefab;

    public static CursePickupIndicator Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowCursePickup(Vector3 position, string curseName, float curseValue)
    {
    
        GameObject textObj = new GameObject("CursePickupText");
        textObj.transform.position = position;

    
        TextMeshPro text = textObj.AddComponent<TextMeshPro>();
        text.text = $"{curseName}\n+{curseValue:F1}";
        text.fontSize = 3f;
        text.color = Color.red;
        text.alignment = TextAlignmentOptions.Center;
        text.sortingOrder = 100;

  
        CurseTextAnimation animation = textObj.AddComponent<CurseTextAnimation>();
        animation.Initialize();

  
        Destroy(textObj, 2f);
    }
}

public class CurseTextAnimation : MonoBehaviour
{
    private float speed = 2f;
    private float fadeSpeed = 1f;
    private TextMeshPro text;
    private float elapsed = 0f;
    private float lifeTime = 2f;

    public void Initialize()
    {
        text = GetComponent<TextMeshPro>();
      
    }

    private void Update()
    {
      
        transform.position += Vector3.up * speed * Time.deltaTime;

      
        elapsed += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, elapsed / lifeTime);
        if (text != null)
        {
            Color color = text.color;
            color.a = alpha;
            text.color = color;
        }

        if (elapsed >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}