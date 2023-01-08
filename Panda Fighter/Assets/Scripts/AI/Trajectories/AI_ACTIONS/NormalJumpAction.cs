using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalJumpAction : AIAction
{
    public NormalJumpAction(AIActionType action, AIActionInfo info) : base(action, info) { }

    private bool hasJumped;

    public override void Begin(AI_Controller controller)
    {
        base.Begin(controller);
        controller.StartCoroutine(normalJump());

        hasJumped = false;
    }

    public override void Run()
    {
        if (!controller.isGrounded && !hasJumped)
            hasJumped = true;

        if (controller.isGrounded && hasJumped)
            Finished = true;
    }

    private IEnumerator normalJump()
    {
        float randomXPos = UnityEngine.Random.Range(Info.JumpBounds.x, Info.JumpBounds.y);
        int dirX = Math.Sign(randomXPos - creature.position.x);

        while ((dirX == 1 && creature.position.x < randomXPos) || (dirX == -1 && creature.position.x > randomXPos))
            yield return null;

        if (!controller.IsActionBeingExecuted(this))
            yield break;

        DirX = Info.DirX;
        Speed = getRandomSpeed(Info.Speed);

        if (controller.isGrounded && !phaseTracker.Is(Phase.DoubleJumping) && !phaseTracker.Is(Phase.Jumping))
            ExecuteNormalJumpNow = true;

        controller.StartCoroutine(changeVelocityAfterDelay(Info.TimeB4Change, Info.ChangedSpeed));
    }
}
