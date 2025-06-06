using System.Collections.Generic;
using UnityEngine;

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
