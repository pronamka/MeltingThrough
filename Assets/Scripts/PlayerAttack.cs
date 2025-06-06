using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class AttackAnimationParameters
{
    public static string PrimaryAttack = "swordAttack";
    public static string BonusAttack = "fireballAttack";
}

public static class AttackAnimationNames
{
    public static string PrimaryAttack = "SwordAttack";
    public static string BonusAttack = "FireballAttack";
}

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float primaryAttackCooldown = 0;
    private float timeSincePrimaryAttack = 0;

    [SerializeField] private float bonusAttackCooldown = 0;
    private float timeSinceBonusAttack;

    private Animator animator;

    private PlayerMovement playerMovement;

    private InputAction primaryAttackAction;
    private InputAction bonusAttackAction;

    private AnimationUtils animationUtils;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        animationUtils = new AnimationUtils(animator);

        primaryAttackCooldown = animationUtils.GetAnimationDuration(AttackAnimationNames.PrimaryAttack);
        bonusAttackCooldown = animationUtils.GetAnimationDuration(AttackAnimationNames.BonusAttack);
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
        Debug.Log("Updating attack");
        HandlePrimaryAttack();
        HandleBonusAttack();
    }

    private void HandlePrimaryAttack()
    {
        //Debug.Log($"Checking for primary attack: {primaryAttackAction.triggered}, {timeSincePrimaryAttack} > {primaryAttackCooldown} = {timeSincePrimaryAttack > primaryAttackCooldown}, {playerMovement.canPrimaryAttack()}");
        if (primaryAttackAction.triggered && 
            timeSincePrimaryAttack > primaryAttackCooldown &&
            playerMovement.canPrimaryAttack()) PrimaryAttack();

        timeSincePrimaryAttack += Time.deltaTime;
    }

    private void PrimaryAttack()
    {
        //Debug.Log("Performing primary attack");
        animator.SetTrigger(AttackAnimationParameters.PrimaryAttack);
        timeSincePrimaryAttack = 0;
    }

    private void HandleBonusAttack()
    {
        if (bonusAttackAction.triggered && 
            timeSinceBonusAttack > bonusAttackCooldown &&
            playerMovement.canBonusAttack()) 
            BonusAttack();

        timeSinceBonusAttack += Time.deltaTime;
    }

    private void BonusAttack()
    {
        animator.SetTrigger(AttackAnimationParameters.BonusAttack);
        timeSinceBonusAttack = 0;
    }
}
