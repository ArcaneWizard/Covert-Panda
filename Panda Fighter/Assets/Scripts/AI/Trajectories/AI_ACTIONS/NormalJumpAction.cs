using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NormalJumpAction : AIAction
{
    public NormalJumpAction(AIActionType action, AIActionInfo info) : base(action, info) { }

    private bool hasJumped;

    public override void StartExecution(AI_Controller controller)
    {
        base.StartExecution(controller);

        executeCoroutine(normalJump());
        hasJumped = false;
    }

    public override void Execute()
    {
        if (!controller.isGrounded && !hasJumped)
            hasJumped = true;

        if (controller.isGrounded && hasJumped)
            Finished = true;
    }

    private IEnumerator normalJump()
    {
        float randomXPos = UnityEngine.Random.Range(Info.Bounds.x, Info.Bounds.y);
        DirX = Math.Sign(randomXPos - creature.position.x);
        Speed = CentralController.MaxSpeed;

        while ((DirX == 1 && creature.position.x < randomXPos) || (DirX == -1 && creature.position.x > randomXPos))
            yield return null;

        DirX = Info.DirX;
        Speed = getRandomSpeed(Info.Speed);

        if (controller.isGrounded && !phaseTracker.Is(Phase.DoubleJumping) && !phaseTracker.Is(Phase.Jumping))
            ExecuteNormalJumpNow = true;

        executeCoroutine(changeVelocityAfterDelay(Info.TimeB4Change, Info.ChangedSpeed));
    }
}
