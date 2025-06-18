using System.Collections.Generic;
using UnityEngine;

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

public static class AnimationParameters
{
    public static string PlayerPrimaryAttack = "swordAttack";
    public static string PlayerBonusAttack = "fireballAttack";

    public static string FireballExplode = "explode";
    public static string Hurt = "hurt";
    public static string Death = "death";

    public static string EnemyPrimaryAttack = "attack";
}

public static class AnimationNames
{
    public static string PlayerPrimaryAttack = "SwordAttack";
    public static string PlayerBonusAttack = "FireballAttack";

    public static string FireballExplode = "Explode";

    public static string Hurt = "Hurt";
    public static string Death = "Death";

    public static string EnemyPrimaryAttack = "Attack";
}

public class AnimationUtils
{
    private Dictionary<string, float> AnimationDurations = new Dictionary<string, float>();
    public AnimationUtils(Animator animator)
    {
        RuntimeAnimatorController controller = animator.runtimeAnimatorController;
        foreach (AnimationClip clip in controller.animationClips)
        {
            AnimationDurations[clip.name] = clip.length;
        }
    }

    public float GetAnimationDuration(string animationName)
    {
        return AnimationDurations[animationName];
    }
}
