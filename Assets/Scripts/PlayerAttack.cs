using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float primaryAttackCooldown;
    private float timeSincePrimaryAttack = 0;

    [SerializeField] private float bonusAttackCooldown;
    private float timeSinceBonusAttack = 0;
    [SerializeField]private FireballPool fireballPool;

    private Animator animator;

    private PlayerState playerState;

    private InputAction primaryAttackAction;
    private InputAction bonusAttackAction;

    private AnimationUtils animationUtils;

    private float spawnFireballAtTime = 0.7f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerState = GetComponent<PlayerState>();

        animationUtils = new AnimationUtils(animator);

        primaryAttackCooldown = animationUtils.GetAnimationDuration(AnimationNames.PrimaryAttack);
        bonusAttackCooldown = animationUtils.GetAnimationDuration(AnimationNames.BonusAttack);
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
        playerState.StartAttack(animationUtils.GetAnimationDuration(AnimationNames.PrimaryAttack));

        animator.SetTrigger(AnimationParameters.PrimaryAttack);
        timeSincePrimaryAttack = 0;
    }

    private void HandleBonusAttack()
    {
        if (bonusAttackAction.triggered && 
            timeSinceBonusAttack > bonusAttackCooldown &&
            playerState.CanBonusAttack()) 
            BonusAttack();

        timeSinceBonusAttack += Time.deltaTime;
    }

    private void BonusAttack()
    {
        playerState.StartAttack(animationUtils.GetAnimationDuration(AnimationNames.BonusAttack));
        animator.SetTrigger(AnimationParameters.BonusAttack);
        Invoke(nameof(SpawnFireball), spawnFireballAtTime);
        timeSinceBonusAttack = 0;
    }

    private void SpawnFireball()
    {
        Fireball fireball = fireballPool.TakeFireball();

        Vector2 direction = CalculateFireballDirection();
        Vector3 spawnPosition = CalculateFireballPosition(direction);

        fireball.transform.position = spawnPosition;
        fireball.SetDirection(direction);
    }

    private Vector3 CalculateFireballPosition(Vector2 direction)
    {
        float fireballOffset = 1.5f;
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
}
