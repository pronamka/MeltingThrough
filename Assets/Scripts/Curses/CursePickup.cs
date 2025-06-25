using UnityEngine;
using UnityEngine.InputSystem;

public class CursePickup : MonoBehaviour
{
    [SerializeField] private CurseType curseType;
    [SerializeField] private float curseValue;
    [SerializeField] private float pickupRange = 2f;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatHeight = 0.3f;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private LayerMask groundLayer = -1;

    private Transform playerTransform;
    private Vector3 startPosition;
    private float timeAlive = 0f;
    private bool isPickedUp = false;
    private bool playerInRange = false;
    private bool hasLanded = false;

    
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D triggerCollider; 

    
    private ParticleSystem particles;

    
    private GameObject pickupHint;

    
    private InputAction interactAction;

    private void Awake()
    {
        SetupVisuals();
        SetupTrigger();
        SetupEffects();
        SetupPickupHint();
        SetupInput();
    }

    private void Start()
    {
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        
        StartFalling();

        
        Destroy(gameObject, 30f);
    }

    private void Update()
    {
        if (isPickedUp) return;

        timeAlive += Time.deltaTime;

        
        if (!hasLanded)
        {
            CheckLanding();
        }
        else
        {
        
            FloatingAnimation();
        }

        
        CheckPlayerDistance();

        
        if (playerInRange && interactAction != null && interactAction.triggered)
        {
            PickupCurse();
        }

        
        HandleBlinking();
    }

    private void StartFalling()
    {
       
        StartCoroutine(FallToGround());
    }

    private System.Collections.IEnumerator FallToGround()
    {
        Vector3 startPos = transform.position;

        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 50f, groundLayer);

        Vector3 targetPos;
        if (hit.collider != null)
        {
        
            targetPos = new Vector3(startPos.x, hit.point.y + 0.5f, startPos.z);
        }
        else
        {
        
            targetPos = new Vector3(startPos.x, startPos.y - 5f, startPos.z);
        }

        
        float fallDuration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < fallDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fallDuration;

            
            t = 1f - (1f - t) * (1f - t); 

            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;
        startPosition = targetPos;
        hasLanded = true;

        
        CreateLandingEffect();
    }

    private void CheckLanding()
    {
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, groundLayer);
        if (hit.collider != null && !hasLanded)
        {
            startPosition = transform.position;
            hasLanded = true;
            CreateLandingEffect();
        }
    }

    private void CreateLandingEffect()
    {
        
        GameObject effect = new GameObject("LandingEffect");
        effect.transform.position = transform.position;

        ParticleSystem landingParticles = effect.AddComponent<ParticleSystem>();
        var main = landingParticles.main;
        main.startLifetime = 0.3f;
        main.startSpeed = 2f;
        main.startSize = 0.1f;
        main.startColor = GetCurseColor();
        main.maxParticles = 15;

        var emission = landingParticles.emission;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0f, 15)
        });

        var shape = landingParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.3f;

        Destroy(effect, 1f);
    }

    private void SetupInput()
    {
        
        interactAction = new InputAction("Interact", binding: "<Keyboard>/e");
        interactAction.Enable();
    }

    private void SetupTrigger()
    {
        
        triggerCollider = gameObject.AddComponent<CircleCollider2D>();
        triggerCollider.radius = pickupRange;
        triggerCollider.isTrigger = true;
    }

    private void SetupVisuals()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        spriteRenderer.sprite = CreateCurseSprite();
        spriteRenderer.sortingOrder = 10;
        transform.localScale = Vector3.one * 0.8f;
    }

    private void SetupEffects()
    {
        GameObject particleObj = new GameObject("CurseParticles");
        particleObj.transform.SetParent(transform);
        particleObj.transform.localPosition = Vector3.zero;

        particles = particleObj.AddComponent<ParticleSystem>();
        var main = particles.main;
        main.startLifetime = 1f;
        main.startSpeed = 0.5f;
        main.startSize = 0.05f;
        main.startColor = GetCurseColor();
        main.maxParticles = 10;

        var emission = particles.emission;
        emission.rateOverTime = 5f;

        var shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.3f;
    }

    private void SetupPickupHint()
    {
        pickupHint = new GameObject("PickupHint");
        pickupHint.transform.SetParent(transform);
        pickupHint.transform.localPosition = Vector3.up * 1.2f;

        TextMesh textMesh = pickupHint.AddComponent<TextMesh>();
        textMesh.text = "Нажмите E";
        textMesh.fontSize = 20;
        textMesh.color = Color.white;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.characterSize = 0.08f;

        MeshRenderer renderer = pickupHint.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = 100;
        }

        pickupHint.SetActive(false);
    }

    private Sprite CreateCurseSprite()
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        Color curseColor = GetCurseColor();

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distanceFromCenter = Vector2.Distance(new Vector2(x, y), new Vector2(size / 2, size / 2));

                if (distanceFromCenter < size / 2 - 2)
                {
                    float alpha = 1f - (distanceFromCenter / (size / 2)) * 0.5f;
                    texture.SetPixel(x, y, new Color(curseColor.r, curseColor.g, curseColor.b, alpha));
                }
                else if (distanceFromCenter < size / 2)
                {
                    texture.SetPixel(x, y, Color.black);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }

        DrawCurseSymbol(texture, size, curseColor);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    private void DrawCurseSymbol(Texture2D texture, int size, Color color)
    {
        int center = size / 2;
        int symbolSize = size / 4;

        switch (curseType)
        {
            case CurseType.SlowMovement:
                DrawArrowDown(texture, center, center, symbolSize, Color.white);
                break;
            case CurseType.WeakAttack:
                DrawBrokenSword(texture, center, center, symbolSize, Color.white);
                break;
            case CurseType.LowHealth:
                DrawCrackedHeart(texture, center, center, symbolSize, Color.white);
                break;
            case CurseType.SlowMana:
                DrawEmptyDrop(texture, center, center, symbolSize, Color.white);
                break;
            case CurseType.HeavyFall:
                DrawRock(texture, center, center, symbolSize, Color.white);
                break;
            case CurseType.ShortJump:
                DrawBrokenWings(texture, center, center, symbolSize, Color.white);
                break;
        }
    }

    private void DrawArrowDown(Texture2D texture, int centerX, int centerY, int size, Color color)
    {
        for (int i = -size / 2; i <= size / 2; i++)
        {
            texture.SetPixel(centerX, centerY + i, color);
        }
        for (int i = 1; i <= size / 3; i++)
        {
            texture.SetPixel(centerX - i, centerY - size / 2 + i, color);
            texture.SetPixel(centerX + i, centerY - size / 2 + i, color);
        }
    }

    private void DrawBrokenSword(Texture2D texture, int centerX, int centerY, int size, Color color)
    {
        for (int i = -size / 2; i <= 0; i++)
        {
            texture.SetPixel(centerX, centerY + i, color);
        }
        for (int i = size / 4; i <= size / 2; i++)
        {
            texture.SetPixel(centerX, centerY + i, color);
        }
    }

    private void DrawCrackedHeart(Texture2D texture, int centerX, int centerY, int size, Color color)
    {
        for (int i = -size / 3; i <= size / 3; i++)
        {
            for (int j = -size / 3; j <= size / 3; j++)
            {
                if (IsInHeart(i, j, size / 3))
                {
                    texture.SetPixel(centerX + i, centerY + j, color);
                }
            }
        }
    }

    private void DrawEmptyDrop(Texture2D texture, int centerX, int centerY, int size, Color color)
    {
        for (int i = -size / 3; i <= size / 3; i++)
        {
            for (int j = -size / 3; j <= size / 3; j++)
            {
                if (IsInDrop(i, j, size / 3))
                {
                    if (Mathf.Abs(i) < 2 && Mathf.Abs(j) < 2) continue;
                    texture.SetPixel(centerX + i, centerY + j, color);
                }
            }
        }
    }

    private void DrawRock(Texture2D texture, int centerX, int centerY, int size, Color color)
    {
        for (int i = -size / 3; i <= size / 3; i++)
        {
            for (int j = -size / 3; j <= size / 3; j++)
            {
                if (i * i + j * j <= (size / 3) * (size / 3))
                {
                    texture.SetPixel(centerX + i, centerY + j, color);
                }
            }
        }
    }

    private void DrawBrokenWings(Texture2D texture, int centerX, int centerY, int size, Color color)
    {
        for (int i = 1; i <= size / 3; i++)
        {
            for (int j = -i; j <= i; j++)
            {
                texture.SetPixel(centerX - size / 4 - i, centerY + j, color);
                texture.SetPixel(centerX + size / 4 + i, centerY + j, color);
            }
        }
    }

    private bool IsInHeart(int x, int y, int size)
    {
        return (x * x + (y + size / 2) * (y + size / 2) <= size * size / 4) ||
               ((x - size / 2) * (x - size / 2) + y * y <= size * size / 4) ||
               ((x + size / 2) * (x + size / 2) + y * y <= size * size / 4);
    }

    private bool IsInDrop(int x, int y, int size)
    {
        return x * x + (y - size / 3) * (y - size / 3) <= size * size / 4;
    }

    private Color GetCurseColor()
    {
        switch (curseType)
        {
            case CurseType.SlowMovement: return new Color(0.5f, 0.5f, 1f);
            case CurseType.WeakAttack: return new Color(1f, 0.5f, 0.5f);
            case CurseType.LowHealth: return new Color(1f, 0f, 0f);
            case CurseType.SlowMana: return new Color(0.5f, 0f, 1f);
            case CurseType.HeavyFall: return new Color(0.5f, 0.3f, 0f);
            case CurseType.ShortJump: return new Color(1f, 1f, 0.5f);
            default: return Color.red;
        }
    }

    private void FloatingAnimation()
    {
        if (!hasLanded) return;


        float newY = startPosition.y + Mathf.Sin(timeAlive * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void CheckPlayerDistance()
    {
        if (playerTransform == null) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (pickupHint != null)
        {
            pickupHint.SetActive(playerInRange);

            
            if (playerInRange && Camera.main != null)
            {
                pickupHint.transform.LookAt(Camera.main.transform);
                pickupHint.transform.Rotate(0, 180, 0);
            }
        }

        if (playerInRange)
        {
            StartGlowing();
        }
        else
        {
            StopGlowing();
        }
    }

    private void StartGlowing()
    {
        transform.localScale = Vector3.one * 1f;
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }
    }

    private void StopGlowing()
    {
        transform.localScale = Vector3.one * 0.8f;
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 0.8f;
            spriteRenderer.color = color;
        }
    }

    private void HandleBlinking()
    {
        if (timeAlive > 25f)
        {
            float blinkSpeed = 10f;
            float alpha = 0.5f + 0.5f * Mathf.Sin(Time.time * blinkSpeed);
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

   
    public void SetPlayerInRange(bool inRange)
    {
        playerInRange = inRange;
    }

    private void PickupCurse()
    {
        if (isPickedUp) return;

        isPickedUp = true;

        if (CurseManager.Instance != null)
        {
            CurseManager.Instance.AddCurse(curseType, curseValue);
        }

        if (pickupSound != null && SoundManager.instance != null)
        {
            SoundManager.instance.PlaySound(pickupSound);
        }

        if (CursePickupIndicator.Instance != null)
        {
            string curseName = GetCurseName();
            CursePickupIndicator.Instance.ShowCursePickup(transform.position, curseName, curseValue);
        }

        CreatePickupEffect();
        Destroy(gameObject);
    }

    private string GetCurseName()
    {
        switch (curseType)
        {
            case CurseType.SlowMovement: return "Замедление";
            case CurseType.WeakAttack: return "Слабость";
            case CurseType.LowHealth: return "Хрупкость";
            case CurseType.SlowMana: return "Истощение";
            case CurseType.HeavyFall: return "Тяжесть";
            case CurseType.ShortJump: return "Слабый прыжок";
            default: return "Проклятие";
        }
    }

    private void CreatePickupEffect()
    {
        GameObject effect = new GameObject("CursePickupEffect");
        effect.transform.position = transform.position;

        ParticleSystem pickupParticles = effect.AddComponent<ParticleSystem>();
        var main = pickupParticles.main;
        main.startLifetime = 0.5f;
        main.startSpeed = 5f;
        main.startSize = 0.2f;
        main.startColor = GetCurseColor();
        main.maxParticles = 30;

        var emission = pickupParticles.emission;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0f, 30)
        });

        var shape = pickupParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;

        Destroy(effect, 2f);
    }

    public void Initialize(CurseType type, float value)
    {
        curseType = type;
        curseValue = value;

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = CreateCurseSprite();
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