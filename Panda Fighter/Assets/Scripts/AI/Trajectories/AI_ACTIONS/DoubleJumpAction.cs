using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpAction : AIAction
{
    public DoubleJumpAction(AIActionType action, AIActionInfo info) : base(action, info) { }

    private bool hasJumped;

    public override void Begin(AI_Controller controller)
    {
        base.Begin(controller);
        controller.StartCoroutine(doubleJump());

        hasJumped = false;
    }

    public override void Run()
    {
        if (!controller.isGrounded && !hasJumped)
            hasJumped = true;

        if (controller.isGrounded && hasJumped)
            Finished = true;
    }

    private IEnumerator doubleJump()
    {
        float randomXPos = UnityEngine.Random.Range(Info.JumpBounds.x, Info.JumpBounds.y);
        int dirX = Math.Sign(randomXPos - creature.position.x);

        while ((dirX == 1 && creature.position.x < randomXPos) || (dirX == -1 && creature.position.x > randomXPos))
            yield return null;

        if (!controller.IsActionBeingExecuted(this))
            yield break;

        DirX = Info.DirX;
        Speed = getRandomSpeed(Info.Speed);

        if (controller.isGrounded && !phaseTracker.Is(Phase.DoubleJumping))
            ExecuteNormalJumpNow = true;

        yield return new WaitForSeconds(UnityEngine.Random.Range(Info.TimeB4Change.x, Info.TimeB4Change.y));
        if (!controller.IsActionBeingExecuted(this))
            yield break;

        float randomSpeed = getRandomSpeed(Info.ChangedSpeed);
        DirX = Info.DirX * (int)Mathf.Sign(randomSpeed);
        Speed = Mathf.Abs(randomSpeed);

        if (phaseTracker.Is(Phase.Jumping) && !phaseTracker.Is(Phase.DoubleJumping))
            ExecuteDoubleJumpNow = true;

        controller.StartCoroutine(changeVelocityAfterDelay(Info.TimeB4SecondChange, Info.SecondChangedSpeed));
    }
}
