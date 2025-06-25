using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    [SerializeField] private float regenerationRate;
    [SerializeField] private float regenerationDelay;

    [SerializeField] private float invincibilityPeriod;

    private float timeSinceTakenDamage = 0;

    private Animator animator;
    private SpriteRenderer playerRenderer;

    private AnimationUtils utils;

    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;

    public bool isDead { get; private set; }

    private void Awake()
    {
        isDead = false;
        animator = GetComponent<Animator>();
        playerRenderer = GetComponent<SpriteRenderer>();

        utils = new AnimationUtils(animator);
    }

    private void Update()
    {
        if (isDead) return;


        timeSinceTakenDamage += Time.deltaTime;
        if (currentHealth == maxHealth) return;

        if (timeSinceTakenDamage > regenerationDelay)
        {
            currentHealth = Math.Min(maxHealth, currentHealth+regenerationRate * Time.deltaTime);
        }
    }

    public void TakeDamage(float damage)
    {
        if (IsInvincible()){}

        timeSinceTakenDamage = 0;
        float health = currentHealth - damage;
        currentHealth = Math.Max(0, health);
        if (health <= 0)
        {
            isDead = true;
            animator.SetTrigger(AnimationParameters.Death);
            SoundManager.instance.PlaySound(deathSound);

            Invoke(nameof(DisableEntity), utils.GetAnimationDuration(AnimationNames.Death));
            return;
        }

        animator.SetTrigger(AnimationParameters.Hurt);
        SoundManager.instance.PlaySound(hurtSound);
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
}
