using UnityEngine;

public class CursePickup : MonoBehaviour
{
    private CurseData curseData;
    private SpriteRenderer spriteRenderer;

    [Header("Pickup Settings")]
    public float bobSpeed = 2f;
    public float bobHeight = 0.5f;

    private Vector3 startPosition;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        startPosition = transform.position;

        if (GetComponent<Collider2D>() == null)
        {
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }
    }

    private void Update()
    {
        if (startPosition == Vector3.zero)
            startPosition = transform.position;

        // ������ ����������� �����-����, ��� ��������
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    public void Initialize(CurseData curse)
    {
        curseData = curse;
        if (spriteRenderer != null && curse.curseSprite != null)
        {
            spriteRenderer.sprite = curse.curseSprite;

            // ��������, ��� ������������ ���������� ��������
            EnsureSpriteMaterial();

            // ��������, ��� �����-����� �� ����� 0
            Color color = curse.uiColor;
            if (color.a == 0) color.a = 1f;
            spriteRenderer.color = color;
        }
        else if (curse.curseSprite == null)
        {
            // ������� ��������� ������ ���� �������� �����������
            CreateFallbackSprite(curse);
        }
    }

    private void EnsureSpriteMaterial()
    {
        // ���������, ����� �� ������������� ��������
        if (spriteRenderer.material == null ||
            spriteRenderer.material.name.Contains("Default-Material") ||
            spriteRenderer.material.shader.name.Contains("Hidden"))
        {
            // �������� ����� ���������� ������
            Shader spriteShader = FindBestSpriteShader();

            if (spriteShader != null)
            {
                Material spriteMaterial = new Material(spriteShader);
                spriteRenderer.material = spriteMaterial;
                Debug.Log($"Applied shader: {spriteShader.name}");
            }
            else
            {
                // ���� �� ����� ������, ��������� �������� ��� null
                // Unity ��� ������� ����������
                spriteRenderer.material = null;
                Debug.Log("Using Unity default material");
            }
        }
    }

    private Shader FindBestSpriteShader()
    {
        // ������ �������� � ������� ����������
        string[] shaderNames = {
            "Sprites/Default",
            "UI/Default",
            "Universal Render Pipeline/2D/Sprite-Lit-Default",
            "Legacy Shaders/Transparent/Diffuse",
            "Unlit/Transparent"
        };

        foreach (string shaderName in shaderNames)
        {
            Shader shader = Shader.Find(shaderName);
            if (shader != null)
            {
                return shader;
            }
        }

        return null;
    }

    private void CreateFallbackSprite(CurseData curse)
    {
        Texture2D texture = new Texture2D(32, 32);
        Color[] colors = new Color[32 * 32];

        // ��������, ��� ���� ����� ���������� �����-�����
        Color fillColor = curse.uiColor;
        if (fillColor.a == 0) fillColor.a = 1f;

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = fillColor;
        }

        texture.SetPixels(colors);
        texture.Apply();

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = Color.white; // ��� fallback ������� ���������� ����� ����

        // ��������, ��� �������� ���������� ���������
        EnsureSpriteMaterial();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (CurseManager.Instance != null)
            {
                CurseManager.Instance.ApplyCurse(curseData);
                Destroy(gameObject);
            }
        }
    }
}