using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DoubleJumpAction2
{
   /* public DoubleJumpAction2(AIActionType action, AIActionInfo info) : base(action, info) { }

    private bool hasJumped;
    private TimedCode timedCode;

    float randomXPos;
    private ExecutionDelay waitTillCreatureIsAtJumpPos = ExecutionDelay.Wait;
    private ExecutionDelay timeB4Change = ExecutionDelay.Unknown;
    private ExecutionDelay timeB4SecondChnange = ExecutionDelay.Unknown;

    private void sudoAwake()
    {
        List<ExecutionDelay> delays = new List<ExecutionDelay>() { 
            ExecutionDelay.Instant, 
            waitTillCreatureIsAtJumpPos, 
            ExecutionDelay.Instant,
            timeB4Change, 
            timeB4SecondChnange 
        };

        List<Action> actions = new List<Action>() { 
            setSpeed, 
            waitTillReachedPos, 
            setJump, 
            setDoubleJump, 
            setFinalSpeed
        };

        timedCode = new TimedCode(delays, actions);

        void setSpeed()
        {
            randomXPos = UnityEngine.Random.Range(Info.Bounds.x, Info.Bounds.y);
            DirX = Math.Sign(randomXPos - creature.position.x);
            Speed = CentralController.MaxSpeed;
        }

        void waitTillReachedPos()
        {
            if (!((DirX == 1 && creature.position.x < randomXPos) || (DirX == -1 && creature.position.x > randomXPos)))
                waitTillCreatureIsAtJumpPos.StopWaiting();
            else
                timedCode.next = setJump;
        }

        void setJump()
        {
            DirX = Info.DirX;
            Speed = getRandomSpeed(Info.Speed);

            if (controller.isGrounded && !phaseTracker.Is(Phase.DoubleJumping))
                ExecuteNormalJumpNow = true;

            timedCode.next = setDoubleJump;
            timeB4Change.seconds = UnityEngine.Random.Range(Info.TimeB4Change.x, Info.TimeB4Change.y);
        }

        void setDoubleJump()
        {
            float randomSpeed = getRandomSpeed(Info.ChangedSpeed);
            DirX = Info.DirX * (int)Mathf.Sign(randomSpeed);
            Speed = Mathf.Abs(randomSpeed);

            if (phaseTracker.Is(Phase.Jumping) && !phaseTracker.Is(Phase.DoubleJumping))
                ExecuteDoubleJumpNow = true;


            timeB4SecondChnange.seconds = UnityEngine.Random.Range(Info.TimeB4SecondChange.x, Info.TimeB4SecondChange.y);
        }

        void setFinalSpeed()
        {
            float randomSpeed = getRandomSpeed(Info.SecondChangedSpeed);
            DirX = Info.DirX * (int)Mathf.Sign(randomSpeed);
            Speed = Mathf.Abs(randomSpeed);
        }
    }

    // starts execution (invoked once)
    public override void StartExecution(AI_Controller controller)
    {
        base.StartExecution(controller);
        hasJumped = false;
        timedCode.Start();
    }

    // during execution (invoked every frame)
    public override void Execute()
    {
        if (!controller.isGrounded && !hasJumped)
            hasJumped = true;

        if (controller.isGrounded && hasJumped)
            Finished = true;

        timedCode.Update();
    }*/
}
