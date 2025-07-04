using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Health : MonoBehaviour


{

    [Header("Curse Drop Settings")]
    [SerializeField] private bool canDropCurse = false; 
    [SerializeField] private float curseDropChance = -1f;

    [SerializeField] public float maxHealth;
    [SerializeField] private float currentHealth;

    [SerializeField] private float regenerationRate;
    [SerializeField] private float regenerationDelay;

    [SerializeField] private float invincibilityPeriod;

    public float timeSinceTakenDamage = 0;

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

    private float hurtAnimationDuration;

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
        hurtAnimationDuration = utils.GetAnimationDuration("Hurt");
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

    public void TakeDamage(float damage, bool playHurtAnimation = true)
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
        
        GameObject curseManager = GameObject.FindGameObjectWithTag("CurseManager");
        curseManager.GetComponent<CurseManager>().TryDropCurse(transform.position);

        float deathAnimationDuration = utils.GetAnimationDuration(AnimationNames.Death);
        Invoke(nameof(DisableEntity), deathAnimationDuration);
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {

        yield return new WaitForSeconds(showCanvasDelay);


        if (deathCanvas != null)
        {
            deathCanvas.SetActive(true);

        }

        animator.SetTrigger(AnimationParameters.Hurt);

        SoundManager.instance.PlaySound(hurtSound);

        yield return new WaitForSeconds(canvasDisplayTime);



        SceneManager.LoadScene(menuSceneName);
    }

    private void DisableEntity()
    {
        playerRenderer.enabled = false;
        Destroy(this.gameObject);
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


    public bool IsBeingHurt()
    {
        return timeSinceTakenDamage < hurtAnimationDuration;
    }
}