using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : CentralAnimationController
{
    protected override void setAnimationState()
    {
        base.setAnimationState();
        bool facingRight = Input.mousePosition.x >= controller.camera.WorldToScreenPoint(controller.shootingArm.parent.position).x;

        //if you're looking in the opposite direction as you're running, set walking speed to -1 (which auto triggers backwards walking animation)
        if (animator.GetInteger("Phase") == 1)
        {
            if ((controller.dirX == 1 && facingRight) || controller.dirX == -1 && !facingRight)
                animator.SetFloat("walking speed", 1);
            else if (controller.dirX != 0)
                animator.SetFloat("walking speed", -1);
        }
    }
}
