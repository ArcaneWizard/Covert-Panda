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
        Debug.Log(currentAnimation + ", " + getAnimation(animation));
    }

    public bool IsPlaying(string animation)
    {
        return animator.GetInteger("Phase") == getAnimation(animation);
    }

    private int getAnimation(string animation)
    {
        switch (animation)
        {
            case null:
                return 0;
            case Animation.jumping:
                return 1;
            case Animation.doubleJumping:
                return 2;
        }

        Debug.LogError("animation not defined");
        return -1;
    }

}

public static class Animation
{
    public const string jumping = "jumping";
    public const string doubleJumping = "double jumping";
}
