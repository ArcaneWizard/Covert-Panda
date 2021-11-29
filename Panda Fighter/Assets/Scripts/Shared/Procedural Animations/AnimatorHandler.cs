using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler
{
    private Animator animator;
    public string currentAnimation { private set; get; }

    public AnimatorHandler(Animator animator)
    {
        this.animator = animator;
    }

    public void SetAnimation(string animation)
    {
        currentAnimation = animation;
        animator.SetInteger("Phase", getAnimation(animation));
    }

    public bool IsPlaying(string animation)
    {
        return animator.GetInteger("Phase") == getAnimation(animation);
    }

    public void ResetJumping()
    {
        animator.SetBool("jumped", false);
        animator.SetBool("double jump", false);
    }

    private int getAnimation(string animation)
    {
        if (animation == null)
            return 0;
        else if (animation == Animation.jumping)
            return 2;

        Debug.LogError("mode not defined");
        return -1;
    }

}

public static class Animation
{
    public static string jumping = "jumping";
}
