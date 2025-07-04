using UnityEngine;
using UnityEngine.InputSystem;

public class CursePickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private float pickupRange = 2f;
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float rotationSpeed = 45f;

    [Header("Visual Settings")]
    [SerializeField] private GameObject promptUI;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Text Settings")]
    [SerializeField] private string itemText = "Static Text";
    [SerializeField] private float textHeightOffset = 1f;
    [SerializeField] private Color textColor = Color.yellow;
    [SerializeField] private int textFontSize = 20;

    private TextMesh textMesh;

    private CurseData curseData;
    private bool playerInRange = false;
    private GameObject player;
    private Vector3 startPosition;
    private InputAction interactAction;

    private void Start()
    {
   
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        startPosition = transform.position;
        SetupColliders();
        SetupInput();

        player = GameObject.FindGameObjectWithTag("Player");

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0.5f;
        rb.linearDamping = 2f;

        
        rb.freezeRotation = true;

       
        CreateTextMesh();
    }

    private void SetupColliders()
    {
        CircleCollider2D triggerCollider = gameObject.AddComponent<CircleCollider2D>();
        triggerCollider.isTrigger = true;
        triggerCollider.radius = pickupRange;

        BoxCollider2D physicsCollider = gameObject.AddComponent<BoxCollider2D>();
        physicsCollider.isTrigger = false;
        physicsCollider.size = new Vector2(0.8f, 0.8f);
    }

    private void SetupInput()
    {
        interactAction = new InputAction();
        interactAction.AddBinding("<Keyboard>/e");
        interactAction.performed += OnInteract;
        interactAction.Enable();
    }

    private void CreateTextMesh()
    {
        GameObject textObject = new GameObject("PickupText");
        textObject.transform.SetParent(transform, false);
        textObject.transform.localPosition = new Vector3(0f, textHeightOffset, 0f);

        textMesh = textObject.AddComponent<TextMesh>();
        textMesh.text = itemText;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = textColor;
        textMesh.characterSize = 0.1f;
        textMesh.fontSize = textFontSize;
    }

    private void Update()
    {
        AnimateFloat();
    }

    private void AnimateFloat()
    {
        Vector3 newPosition = startPosition;
        newPosition.y += Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = newPosition;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (playerInRange)
        {
            PickupCurse();
        }
    }

    private void PickupCurse()
    {
        if (curseData == null)
        {
            Debug.LogWarning("Attempting to pickup curse but curseData is null!");
            return;
        }

        if (CurseManager.Instance != null)
        {
            CurseManager.Instance.ApplyCurse(curseData);
        }
        else
        {
            Debug.LogError("CurseManager.Instance is null. Cannot apply curse.");
        }

        CreatePickupEffect();

        if (curseData.pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(curseData.pickupSound, transform.position);
        }

        Debug.Log($"Picked up curse: {curseData.curseName}");

        Destroy(gameObject);
    }

    private void CreatePickupEffect()
    {
        GameObject effectObj = new GameObject("PickupEffect");
        effectObj.transform.position = transform.position;

        ParticleSystem particles = effectObj.AddComponent<ParticleSystem>();
        var main = particles.main;
        main.startLifetime = 1f;
        main.startSpeed = 3f;
        main.startSize = 0.3f;
        main.startColor = curseData != null ? curseData.uiColor : Color.red;
        main.maxParticles = 15;

        var emission = particles.emission;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0f, 15)
        });

        var shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.3f;

        Destroy(effectObj, 2f);
    }

    public void Initialize(CurseData curse)
    {
        curseData = curse;
        if (spriteRenderer != null && curse.curseSprite != null)
        {
            spriteRenderer.sprite = curse.curseSprite;
            Color color = curse.uiColor;
            if (color.a == 0) color.a = 1f;
            spriteRenderer.color = color;
        }

        startPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (promptUI != null)
            {
                promptUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (promptUI != null)
            {
                promptUI.SetActive(false);
            }
        }
    }

    private void OnDestroy()
    {
        if (interactAction != null)
        {
            interactAction.Disable();
            interactAction.Dispose();
        }
    }
}