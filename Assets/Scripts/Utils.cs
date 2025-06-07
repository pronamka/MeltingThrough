using System.Collections.Generic;
using UnityEngine;

public static class AnimationParameters
{
    public static string PrimaryAttack = "swordAttack";
    public static string BonusAttack = "fireballAttack";

    public static string FireballExplode = "explode";
}

public static class AnimationNames
{
    public static string PrimaryAttack = "SwordAttack";
    public static string BonusAttack = "FireballAttack";

    public static string FireballExplode = "Explode";
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
