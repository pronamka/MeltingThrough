using UnityEngine;

public class CursePickup : MonoBehaviour
{
    private CurseData curseData;
    private SpriteRenderer spriteRenderer;

    [Header("Pickup Settings")]
    public float bobSpeed = 2f;
    public float bobHeight = 0.5f;
    public float rotationSpeed = 90f;

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

        
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    public void Initialize(CurseData curse)
    {
        curseData = curse;
        if (spriteRenderer != null && curse.curseSprite != null)
        {
            spriteRenderer.sprite = curse.curseSprite;
            spriteRenderer.color = curse.uiColor;
        }

       
        if (curse.curseSprite == null)
        {
       
            Texture2D texture = new Texture2D(32, 32);
            Color[] colors = new Color[32 * 32];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = curse.uiColor;
            }
            texture.SetPixels(colors);
            texture.Apply();

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
            spriteRenderer.sprite = sprite;
        }
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