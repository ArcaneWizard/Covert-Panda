using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_AnimationController : CentralAnimationController
{
    protected override void updateAnimationState()
    {
        base.updateAnimationState();
        bool facingRight = body.localEulerAngles.y == 0;

        // if you're looking in the same direction as you're running, play the 
        // the normal running animation. Otherwise play the backwards walking animation. 
        if (animator.GetInteger("Phase") == 1)
        {
            if ((controller.dirX == 1 && facingRight) || controller.dirX == -1 && !facingRight)
                animator.SetFloat("walking speed", 1);
            else if (controller.dirX != 0)
                animator.SetFloat("walking speed", -1);
        }
       
    }

}