using System;
using System.Collections;

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
        if (!controller.IsGrounded && !hasJumped)
            hasJumped = true;

        if (controller.IsGrounded && hasJumped)
            Finished = true;
    }

    private IEnumerator normalJump()
    {
        float randomXPos = UnityEngine.Random.Range(Info.Bounds.x, Info.Bounds.y);
        DirX = Math.Sign(randomXPos - creature.position.x);
        Speed = CentralController.MAX_SPEED;

        while ((DirX == 1 && creature.position.x < randomXPos) || (DirX == -1 && creature.position.x > randomXPos))
            yield return null;

        DirX = Info.DirX;
        Speed = getRandomSpeed(Info.Speed);

        if (controller.IsGrounded && !phaseTracker.Is(Phase.DoubleJumping) && !phaseTracker.Is(Phase.Jumping))
            ExecuteNormalJumpNow = true;

        executeCoroutine(changeVelocityAfterDelay(Info.TimeB4Change, Info.ChangedSpeed));
    }
}
