using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Health : MonoBehaviour
{
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [SerializeField] private float regenerationRate = 5f;
    [SerializeField] private float regenerationDelay = 3f;

    [SerializeField] private float invincibilityPeriod = 1f;

    private float timeSinceTakenDamage = 0;

    private Animator animator;
    private SpriteRenderer playerRenderer;

    private AnimationUtils utils;

    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;

    [Header("Death UI Settings")]
    [SerializeField] private GameObject deathCanvas;
    [SerializeField] private float showCanvasDelay = 2f; 
    [SerializeField] private float canvasDisplayTime = 60f; 
    [SerializeField] private string menuSceneName = "MenuScene"; 

    public bool isDead { get; private set; }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (currentHealth <= 0)
            currentHealth = maxHealth;
    }
#endif

    private void Awake()
    {
        isDead = false;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        playerRenderer = GetComponent<SpriteRenderer>();

        utils = new AnimationUtils(animator);

        if (deathCanvas != null)
            deathCanvas.SetActive(false);
    }

    private void Start()
    {
        if (deathCanvas == null)
        {
            GameObject foundCanvas = GameObject.FindGameObjectWithTag("DeathUI");
            if (foundCanvas == null)
                foundCanvas = GameObject.Find("DeathCanvas");

            if (foundCanvas != null)
            {
                deathCanvas = foundCanvas;
                deathCanvas.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (isDead) return;

        timeSinceTakenDamage += Time.deltaTime;
        if (currentHealth >= maxHealth) return;

        if (timeSinceTakenDamage > regenerationDelay)
        {
            currentHealth = Math.Min(maxHealth, currentHealth + regenerationRate * Time.deltaTime);
        }
    }

    public void TakeDamage(float damage)
    {
        if (IsInvincible()) return;

        timeSinceTakenDamage = 0;
        float health = currentHealth - damage;
        currentHealth = Math.Max(0, health);

        if (health <= 0)
        {
            HandleDeath();
            return;
        }

        animator.SetTrigger(AnimationParameters.Hurt);
        SoundManager.instance.PlaySound(hurtSound);
    }

    private void HandleDeath()
    {
        isDead = true;
        animator.SetTrigger(AnimationParameters.Death);
        SoundManager.instance.PlaySound(deathSound);

    
        StartCoroutine(DeathSequence());

        float deathAnimationDuration = utils.GetAnimationDuration(AnimationNames.Death);
        Invoke(nameof(DisableEntity), deathAnimationDuration);
    }

    private IEnumerator DeathSequence()
    {
    
        yield return new WaitForSeconds(showCanvasDelay);

    
        if (deathCanvas != null)
        {
            deathCanvas.SetActive(true);
    
        }

    
        yield return new WaitForSeconds(canvasDisplayTime);

    
    
        SceneManager.LoadScene(menuSceneName);
    }

    private void DisableEntity()
    {
        playerRenderer.enabled = false;

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this && script != null)
            {
                script.enabled = false;
            }
        }
    }

    private bool IsInvincible()
    {
        return timeSinceTakenDamage < invincibilityPeriod;
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    
    public void GoToMenuNow()
    {
        StopAllCoroutines(); 
        SceneManager.LoadScene(menuSceneName);
    }

   
}