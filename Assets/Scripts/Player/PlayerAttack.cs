using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] public float primaryAttackDamage;
    private float primaryAttackCooldown;
    private float timeSincePrimaryAttack = 0;

    private float bonusAttackCooldown;
    private float timeSinceBonusAttack = 0;
    [SerializeField] private float bonusAttackManaCost;
    [SerializeField] private FireballPool fireballPool;

    private Rigidbody2D body;
    private Animator animator;
    private PlayerState playerState;

    private InputAction primaryAttackAction;
    private InputAction bonusAttackAction;

    private AnimationUtils animationUtils;

    [SerializeField] private AudioClip bonusAttackSound;
    [SerializeField] private AudioClip primaryAttackSound;

    private float spawnFireballAtTime = 0.7f;

    private Dictionary<string, AttackAction> attackAnimationsAndSounds =
        new Dictionary<string, AttackAction>();

    private Dictionary<Collider2D, bool> enemies = new Dictionary<Collider2D, bool>();

    private Vector2 fireballDirection;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerState = GetComponent<PlayerState>();

        animationUtils = new AnimationUtils(animator);

        primaryAttackCooldown = animationUtils.GetAnimationDuration(AnimationNames.PlayerPrimaryAttack);
        bonusAttackCooldown = animationUtils.GetAnimationDuration(AnimationNames.PlayerBonusAttack);

        attackAnimationsAndSounds["primary"] = new AttackAction(
            AnimationNames.PlayerPrimaryAttack,
            animationUtils.GetAnimationDuration(AnimationNames.PlayerPrimaryAttack),
            AnimationParameters.PlayerPrimaryAttack,
            primaryAttackSound
        );

        attackAnimationsAndSounds["bonus"] = new AttackAction(
            AnimationNames.PlayerBonusAttack,
            animationUtils.GetAnimationDuration(AnimationNames.PlayerBonusAttack),
            AnimationParameters.PlayerBonusAttack,
            bonusAttackSound
        );
    }

    private void Start()
    {
        primaryAttackAction = InputSystem.actions.FindAction("PrimaryAttack");
        primaryAttackAction.Enable();
        bonusAttackAction = InputSystem.actions.FindAction("BonusAttack");
        bonusAttackAction.Enable();
    }

    void Update()
    {
        if (playerState.IsDead()) return;
        HandlePrimaryAttack();
        HandleBonusAttack();
    }

    private void HandlePrimaryAttack()
    {
        if (primaryAttackAction.triggered &&
            timeSincePrimaryAttack > primaryAttackCooldown &&
            playerState.CanPrimaryAttack()) PrimaryAttack();

        timeSincePrimaryAttack += Time.deltaTime;
    }

    private void PrimaryAttack()
    {
        ManageAnimationAndSound("primary");

        timeSincePrimaryAttack = 0;
    }

    private void DealDamage()
    {
        foreach (Collider2D enemy in enemies.Keys)
        {
            if (enemy.transform == null)
            {
                enemies.Remove(enemy);
                continue;
            }
            enemy.transform.GetComponent<EnemyState>().TakeDamage(primaryAttackDamage);
        }
    }

    private void HandleBonusAttack()
    {
        if (bonusAttackAction.triggered &&
            timeSinceBonusAttack > bonusAttackCooldown &&
            playerState.CanBonusAttack()&&playerState.mana.HasEnoughMana(bonusAttackManaCost))
            BonusAttack();
        timeSinceBonusAttack += Time.deltaTime;
    }

    private void BonusAttack()
    {
        playerState.mana.UseMana(bonusAttackManaCost);
        ManageAnimationAndSound("bonus");

        fireballDirection = CalculateFireballDirection();
        timeSinceBonusAttack = 0;
    }

    private void SpawnFireball()
    {
        Fireball fireball = fireballPool.TakeFireball();

        Vector3 spawnPosition = CalculateFireballPosition(fireballDirection);

        fireball.transform.position = spawnPosition;
        fireball.SetDirection(fireballDirection, body.linearVelocity);
    }

    private Vector3 CalculateFireballPosition(Vector2 direction)
    {
        float fireballOffset = 2f;
        Vector3 spawnPosition = transform.position + (Vector3)(direction * fireballOffset);
        return spawnPosition;
    }

    private Vector2 CalculateFireballDirection()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mousePosition.z = 0;
        Vector2 direction = (mousePosition - transform.position).normalized;

        return direction;
    }

    private void ManageAnimationAndSound(string attackName)
    {
        if (!attackAnimationsAndSounds.ContainsKey(attackName))
        {
            throw new KeyNotFoundException();
        }

        AttackAction attack = attackAnimationsAndSounds[attackName];
        playerState.StartAttack(attack.AnimationDuration);
        animator.SetTrigger(attack.AnimationTrigger);
        SoundManager.instance.PlaySound(attack.AnimationSound);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            enemies[other] = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (enemies.ContainsKey(other))
        {
            enemies.Remove(other);
        }
    }
}
