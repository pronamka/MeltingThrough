using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Health : MonoBehaviour
{

    [Header("Curse Drop Settings")]
    [SerializeField] protected bool canDropCurse = false; 
    [SerializeField] protected float curseDropChance = -1f;

    [SerializeField] public float maxHealth;
    [SerializeField] protected float currentHealth;

    [SerializeField] protected float regenerationRate;
    [SerializeField] protected float regenerationDelay;

    [SerializeField] protected float invincibilityPeriod;

    public float timeSinceTakenDamage = 0;

    protected Animator animator;
    protected SpriteRenderer playerRenderer;

    protected AnimationUtils utils;

    [SerializeField] protected AudioClip hurtSound;
    [SerializeField] protected AudioClip deathSound;

    protected float hurtAnimationDuration;

    public bool isDead { get; protected set; }

#if UNITY_EDITOR
    protected void OnValidate()
    {
        if (currentHealth <= 0)
            currentHealth = maxHealth;
    }
#endif

    protected void Awake()
    {
        isDead = false;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        playerRenderer = GetComponent<SpriteRenderer>();

        utils = new AnimationUtils(animator);
        hurtAnimationDuration = utils.GetAnimationDuration("Hurt");
    }

    protected void Update()
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

    public virtual void HandleDeath()
    {
        isDead = true;
        animator.SetTrigger(AnimationParameters.Death);
        SoundManager.instance.PlaySound(deathSound);

        Vector3 pos = transform.position;

        float deathAnimationDuration = utils.GetAnimationDuration(AnimationNames.Death);
        Invoke(nameof(DisableEntity), deathAnimationDuration);

        GameObject curseManager = GameObject.FindGameObjectWithTag("CurseManager");
        curseManager.GetComponent<CurseManager>().TryDropCurse(pos);

        //StartCoroutine(DeathSequence());

    }



    protected void DisableEntity()
    {
        playerRenderer.enabled = false;
        Destroy(this.gameObject);
    }

    protected bool IsInvincible()
    {
        return timeSinceTakenDamage < invincibilityPeriod;
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }


    public bool IsBeingHurt()
    {
        return timeSinceTakenDamage < hurtAnimationDuration;
    }
}