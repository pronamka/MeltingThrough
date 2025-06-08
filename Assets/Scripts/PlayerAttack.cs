using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackAction
{
    public string AnimationName;
    public float AnimationDuration;
    public string AnimationTrigger;
    public AudioClip AnimationSound;


    public AttackAction(
        string animationName,
        float animationDuration,
        string animationTrigger,
        AudioClip animationSound
        )
    {
        AnimationName = animationName;
        AnimationDuration = animationDuration;
        AnimationTrigger = animationTrigger;
        AnimationSound = animationSound;
    }
}

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float primaryAttackCooldown;
    private float timeSincePrimaryAttack = 0;

    [SerializeField] private float bonusAttackCooldown;
    private float timeSinceBonusAttack = 0;
    [SerializeField] private FireballPool fireballPool;

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

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerState = GetComponent<PlayerState>();

        animationUtils = new AnimationUtils(animator);

        primaryAttackCooldown = animationUtils.GetAnimationDuration(AnimationNames.PrimaryAttack);
        bonusAttackCooldown = animationUtils.GetAnimationDuration(AnimationNames.BonusAttack);

        attackAnimationsAndSounds["primary"] = new AttackAction(
            AnimationNames.PrimaryAttack,
            animationUtils.GetAnimationDuration(AnimationNames.PrimaryAttack),
            AnimationParameters.PrimaryAttack,
            primaryAttackSound
        );

        attackAnimationsAndSounds["bonus"] = new AttackAction(
            AnimationNames.BonusAttack,
            animationUtils.GetAnimationDuration(AnimationNames.BonusAttack),
            AnimationParameters.BonusAttack,
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
        ManageAnimationAndSound("bonus");

        Vector2 fireballDirection = CalculateFireballDirection();
        StartCoroutine(SpawnFireball(fireballDirection));
        timeSinceBonusAttack = 0;
    }

    private IEnumerator SpawnFireball(Vector2 direction)
    {
        yield return new WaitForSeconds(spawnFireballAtTime);
        Fireball fireball = fireballPool.TakeFireball();

        Vector3 spawnPosition = CalculateFireballPosition(direction);

        fireball.transform.position = spawnPosition;
        fireball.SetDirection(direction);
    }

    private Vector3 CalculateFireballPosition(Vector2 direction)
    {
        float fireballOffset = 1.7f;
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
}
