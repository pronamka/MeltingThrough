using UnityEngine;
using UnityEngine.InputSystem;

public class CursePickup : MonoBehaviour
{
    private CurseData curseData;
    private SpriteRenderer spriteRenderer;

    [Header("Pickup Settings")]
    public float bobSpeed = 2f;
    public float bobHeight = 0.5f;
    public float pickupRange = 2f;

    [Header("UI Settings")]
    public GameObject pickupPrompt;
    public string promptText = "Нажмите E";

    private Vector3 startPosition;
    private bool playerInRange = false;
    private GameObject player;
    private InputAction interactAction;
    private GameObject promptUI;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        startPosition = transform.position;

        // Настраиваем коллайдеры
        SetupColliders();

        // Настраиваем ввод
        SetupInput();

        // Создаем UI подсказку
        CreatePickupPrompt();

        // Находим игрока
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void SetupColliders()
    {
        // Триггер для определения близости игрока
        CircleCollider2D triggerCollider = gameObject.AddComponent<CircleCollider2D>();
        triggerCollider.isTrigger = true;
        triggerCollider.radius = pickupRange;
    }

    private void SetupInput()
    {
        // Создаем действие для взаимодействия
        interactAction = new InputAction("Interact", binding: "<Keyboard>/e");
        interactAction.Enable();
    }

    private void CreatePickupPrompt()
    {
        // Создаем 3D текст над объектом
        promptUI = new GameObject("PickupPrompt");
        promptUI.transform.SetParent(transform);
        promptUI.transform.localPosition = Vector3.up * 1.5f;

        TextMesh textMesh = promptUI.AddComponent<TextMesh>();
        textMesh.text = promptText;
        textMesh.fontSize = 20;
        textMesh.color = Color.white;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.characterSize = 0.1f;

        // Добавляем MeshRenderer для правильного отображения
        MeshRenderer meshRenderer = promptUI.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = 100;
        }

        // Изначально скрываем подсказку
        promptUI.SetActive(false);
    }

    private void Update()
    {
        if (startPosition == Vector3.zero)
            startPosition = transform.position;

        // Анимация плавающего движения
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Поворачиваем подсказку к камере
        if (promptUI != null && promptUI.activeInHierarchy && Camera.main != null)
        {
            promptUI.transform.LookAt(Camera.main.transform);
            promptUI.transform.Rotate(0, 180, 0); // Поворачиваем текст правильно
        }

        // Проверяем нажатие E когда игрок рядом
        if (playerInRange && interactAction.triggered)
        {
            PickupCurse();
        }

        // Эффект свечения когда игрок рядом
        UpdateGlowEffect();
    }

    private void UpdateGlowEffect()
    {
        if (spriteRenderer != null)
        {
            if (playerInRange)
            {
                // Эффект пульсации
                float pulse = Mathf.Sin(Time.time * 8f) * 0.2f + 0.8f;
                Color color = spriteRenderer.color;
                color.a = pulse;
                spriteRenderer.color = color;

                // Увеличиваем размер
                transform.localScale = Vector3.one * (1f + Mathf.Sin(Time.time * 4f) * 0.1f);
            }
            else
            {
                // Обычное состояние
                Color color = spriteRenderer.color;
                color.a = 0.8f;
                spriteRenderer.color = color;
                transform.localScale = Vector3.one;
            }
        }
    }

    private void PickupCurse()
    {
        if (CurseManager.Instance != null && curseData != null)
        {
            CurseManager.Instance.ApplyCurse(curseData);

            // Эффект подбора
            CreatePickupEffect();

            // Звук подбора (если есть)
            PlayPickupSound();

            Destroy(gameObject);
        }
    }

    private void CreatePickupEffect()
    {
        // Создаем эффект частиц при подборе
        GameObject effectObj = new GameObject("PickupEffect");
        effectObj.transform.position = transform.position;

        ParticleSystem particles = effectObj.AddComponent<ParticleSystem>();
        var main = particles.main;
        main.startLifetime = 0.5f;
        main.startSpeed = 5f;
        main.startSize = 0.2f;
        main.startColor = curseData != null ? curseData.uiColor : Color.white;
        main.maxParticles = 20;

        var emission = particles.emission;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0f, 20)
        });

        var shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.5f;

        // Уничтожаем эффект через 2 секунды
        Destroy(effectObj, 2f);
    }

    private void PlayPickupSound()
    {
        // Если у вас есть AudioSource или SoundManager
        if (SoundManager.instance != null)
        {
            // SoundManager.instance.PlaySound("curse_pickup");
        }
    }

    public void Initialize(CurseData curse)
    {
        curseData = curse;
        if (spriteRenderer != null && curse.curseSprite != null)
        {
            spriteRenderer.sprite = curse.curseSprite;

            // Обеспечиваем правильное отображение спрайта
            EnsureSpriteMaterial();

            // Проверяем, что альфа-канал не равен 0
            Color color = curse.uiColor;
            if (color.a == 0) color.a = 0.8f; // Немного прозрачности для красоты
            spriteRenderer.color = color;
        }
        else if (curse.curseSprite == null)
        {
            // Создаем запасной спрайт если основной отсутствует
            CreateFallbackSprite(curse);
        }
    }

    private void EnsureSpriteMaterial()
    {
        // Проверяем, нужен ли материал для спрайта
        if (spriteRenderer.material == null ||
            spriteRenderer.material.name.Contains("Default-Material") ||
            spriteRenderer.material.shader.name.Contains("Hidden"))
        {
            // Находим подходящий шейдер для спрайта
            Shader spriteShader = FindBestSpriteShader();

            if (spriteShader != null)
            {
                Material spriteMaterial = new Material(spriteShader);
                spriteRenderer.material = spriteMaterial;
                Debug.Log($"Applied shader: {spriteShader.name}");
            }
            else
            {
                // Если не найден шейдер, оставляем материал как null
                // Unity сам подберет подходящий
                spriteRenderer.material = null;
                Debug.Log("Using Unity default material");
            }
        }
    }

    private Shader FindBestSpriteShader()
    {
        // Список шейдеров в порядке приоритета
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

        // Проверяем, что цвет имеет достаточную альфа-канал
        Color fillColor = curse.uiColor;
        if (fillColor.a == 0) fillColor.a = 1f;

        // Создаем круглую форму
        Vector2 center = new Vector2(16, 16);
        float radius = 14f;

        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 32; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance <= radius)
                {
                    float alpha = 1f - (distance / radius) * 0.3f;
                    colors[y * 32 + x] = new Color(fillColor.r, fillColor.g, fillColor.b, alpha);
                }
                else
                {
                    colors[y * 32 + x] = Color.clear;
                }
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = Color.white; // Для fallback спрайта устанавливаем белый цвет

        // Проверяем, что материал правильно настроен
        EnsureSpriteMaterial();
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
        // Очищаем ресурсы
        if (interactAction != null)
        {
            interactAction.Disable();
            interactAction.Dispose();
        }
    }
}