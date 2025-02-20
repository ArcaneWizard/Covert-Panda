using System;
using System.Collections;

using UnityEngine;

[System.Serializable]
public class DoubleJumpAction : AIAction
{
    public DoubleJumpAction(AIActionType action, AIActionInfo info) : base(action, info) { }

    private bool hasJumped;

    public override void StartExecution(AI_Controller controller)
    {
        base.StartExecution(controller);
        executeCoroutine(doubleJump());

        hasJumped = false;
    }

    public override void Execute()
    {
        if (!controller.IsGrounded && !hasJumped)
            hasJumped = true;

        if (controller.IsGrounded && hasJumped)
            Finished = true;
    }

    private IEnumerator doubleJump()
    {
        float randomXPos = UnityEngine.Random.Range(Info.Bounds.x, Info.Bounds.y);
        DirX = Math.Sign(randomXPos - creature.position.x);
        Speed = CentralController.MAX_SPEED;

        while ((DirX == 1 && creature.position.x < randomXPos) || (DirX == -1 && creature.position.x > randomXPos))
            yield return null;

        DirX = Info.DirX;
        Speed = getRandomSpeed(Info.Speed);

        if (controller.IsGrounded && !phaseTracker.Is(Phase.DoubleJumping))
            ExecuteNormalJumpNow = true;

        yield return new WaitForSeconds(UnityEngine.Random.Range(Info.TimeB4Change.x, Info.TimeB4Change.y));

        float randomSpeed = getRandomSpeed(Info.ChangedSpeed);
        DirX = Info.DirX * (int)Mathf.Sign(randomSpeed);
        Speed = Mathf.Abs(randomSpeed);

        if (phaseTracker.Is(Phase.Jumping) && !phaseTracker.Is(Phase.DoubleJumping))
            ExecuteDoubleJumpNow = true;

        executeCoroutine(changeVelocityAfterDelay(Info.TimeB4SecondChange, Info.SecondChangedSpeed));
    }
}
