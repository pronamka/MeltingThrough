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

        // Добавляем Rigidbody2D для физики
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0.5f; // Легкая гравитация
        rb.linearDamping = 2f; // Сопротивление для плавной остановки
    }

    private void SetupColliders()
    {
        // Триггер для определения близости игрока
        CircleCollider2D triggerCollider = gameObject.AddComponent<CircleCollider2D>();
        triggerCollider.isTrigger = true;
        triggerCollider.radius = pickupRange;

        // Физический коллайдер для взаимодействия с землей
        BoxCollider2D physicsCollider = gameObject.AddComponent<BoxCollider2D>();
        physicsCollider.isTrigger = false;
        physicsCollider.size = new Vector2(0.8f, 0.8f);
    }

    private void SetupInput()
    {
        interactAction = new InputAction();
        interactAction.AddBinding("<Keyboard>/e");
        interactAction.AddBinding("<Keyboard>/f"); // Альтернативная клавиша
        interactAction.performed += OnInteract;
        interactAction.Enable();
    }

   
    private void Update()
    {
        // Анимация плавания
        AnimateFloat();

        // Вращение
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Проверяем дистанцию до игрока для автоматического подбора
        if (player != null && playerInRange)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= 1f) // Очень близко - автоматический подбор
            {
                PickupCurse();
            }
        }
    }

    private void AnimateFloat()
    {
        // Плавное движение вверх-вниз
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

        // Добавляем проклятие игроку через CurseManager
        if (CurseManager.Instance != null)
        {
            CurseManager.Instance.ApplyCurse(curseData);
        }
        else
        {
            Debug.LogError("CurseManager.Instance is null. Cannot apply curse.");
        }

        // Создаем эффект подбора
        CreatePickupEffect();

        // Звук подбора (если есть SoundManager)
        if (curseData.pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(curseData.pickupSound, transform.position);
        }

        Debug.Log($"Picked up curse: {curseData.curseName}");

        // Уничтожаем объект
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

        // Уничтожаем эффект
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

        // Обновляем стартовую позицию после инициализации
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