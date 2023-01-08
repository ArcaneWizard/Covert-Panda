
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Controller : CentralController
{
    // the current action the AI is executing
    public AIAction currAction { get; private set; }

    // the decision zone that provided the current action the AI should execute
    private Transform decisionZone;

    // the status of the current action the AI is executing
    public Status ActionProgress { get; private set; }

    private bool hasStartedAction;

    private HashSet<Transform> decisionZonesNearby;

    private bool hasNotFallenYet;
    private bool leftPlatform;
    private float randomXPos;
    private float randomSpeed;

    protected override void Start()
    {
        base.Start();

        dirX = UnityEngine.Random.Range(0, 2) * 2 - 1;
        currAction = null;
        decisionZone = null;
        decisionZonesNearby= new HashSet<Transform>();
    }

    // Return the coordinates of a point in space in front of the AI's upper body
    public Vector3 InFrontOfAI() => shootingArm.position + new Vector3(dirX, 0, 0);

    // Begin executing a specified action.
    public void StartAction(AIAction AI_action, Transform zone = null)
    {
        currAction = AI_action;
        decisionZone = zone;
        hasStartedAction = false;
    }

    // End the current action
    public void EndAction() => currAction = null;

    // Check if the following action is being executed
    public bool IsActionBeingExecuted(AIAction action) => currAction.Equals(action) && ActionProgress == Status.InProgress;

    // Change the x direction of the creature's movement.
    public void SetDirection(int dir) => dirX = dir;

    // Change the speed of the creature's movement.
    public void SetSpeed(float speed) => this.speed = speed;


    void Update()
    {
        // don't do anything if dead
        if (health.isDead)
        {
            isTouchingMap = false;
            EndAction();
            return;
        }

        // AI moves at max speed when not executing an action
        if (currAction == null)
        {
            speed = maxSpeed;
            return;
        }

        // When the AI is grounded, start executing the action
        if (isGrounded && isTouchingMap)
            executeCurrentAction();
    }

    // executes the current action 
    private void executeCurrentAction()
    {
        if (!hasStartedAction)
        {
            if (decisionZonesNearby.Contains(decisionZone))
            {
                speed = maxSpeed;
                dirX = currAction.Info.DirX;
                currAction.StartExecution();
            }
            else
                EndAction();

            hasStartedAction = true;
        }

        if (!currAction.FinishedExecuting)
            currAction.Execute();
        else
            speed = maxSpeed;
    }

    //------------------------------------------------------------------
    //----------Handle Falling at the right time------------------------
    //------------------------------------------------------------------

    private IEnumerator executeFallingDownCurveMotion(AIAction action)
    {
        speed = getRandomReasonableSpeed(currAction.Speed);
        
        yield return new WaitForSeconds(UnityEngine.Random.Range(currAction.TimeB4Change.x, currAction.TimeB4Change.y));
        if (!isCreatureStillExecutingThisAction(action))
            yield break;

        randomSpeed = getRandomReasonableSpeed(currAction.ChangedSpeed);
        dirX = currAction.DirX * (int)Mathf.Sign(randomSpeed);
        speed = Mathf.Abs(randomSpeed);

        yield return new WaitForSeconds(UnityEngine.Random.Range(currAction.TimeB4SecondChange.x, currAction.TimeB4SecondChange.y));
        if (!isCreatureStillExecutingThisAction(action))
            yield break;

        randomSpeed = getRandomReasonableSpeed(currAction.SecondChangedSpeed);
        dirX = currAction.DirX * (int)Mathf.Sign(randomSpeed);
        speed = Mathf.Abs(randomSpeed);
    }

    //------------------------------------------------------------------
    //---------Handle Jumping at the right time------------------------
    //------------------------------------------------------------------

    private IEnumerator executeVerticalMotionAction(AIAction action)
    {
        randomXPos = UnityEngine.Random.Range(currAction.JumpBounds.x, currAction.JumpBounds.y);
        dirX = Math.Sign(randomXPos - transform.position.x);

        while ((dirX == 1 && transform.position.x < randomXPos) || (dirX == -1 && transform.position.x > randomXPos)) 
            yield return null;
        if (!isCreatureStillExecutingThisAction(action))
            yield break;

        dirX = currAction.DirX;
        speed = getRandomReasonableSpeed(currAction.Speed);

        if (currAction.actionName == "launchPad")
            StartCoroutine(executeJumpPadLaunchAtRightMoment(currAction));

        else if (currAction.actionName == "normalJump") 
        {
            if (isGrounded && !phaseTracker.Is(Phase.DoubleJumping) && !phaseTracker.Is(Phase.Jumping))
                normalJump();

            StartCoroutine(changeVelocityAfterDelay(currAction.TimeB4Change, currAction.ChangedSpeed, currAction));
        }

        else if (currAction.actionName == "doubleJump")
            StartCoroutine(executeDoubleJumpAtRightMoment(currAction));
    }

    private IEnumerator executeDoubleJumpAtRightMoment(AIAction action)
    {
        if (isGrounded && !phaseTracker.Is(Phase.DoubleJumping))
            normalJump();

        yield return new WaitForSeconds(UnityEngine.Random.Range(currAction.TimeB4Change.x, currAction.TimeB4Change.y));
        if (!isCreatureStillExecutingThisAction(action))
            yield break;

        randomSpeed = getRandomReasonableSpeed(currAction.ChangedSpeed);
        dirX = currAction.DirX * (int)Mathf.Sign(randomSpeed);
        speed = Mathf.Abs(randomSpeed);

        if (phaseTracker.Is(Phase.Jumping) && !phaseTracker.Is(Phase.DoubleJumping))
            doubleJump();

        StartCoroutine(changeVelocityAfterDelay(currAction.TimeB4SecondChange, currAction.SecondChangedSpeed, currAction));
    }

    //apply a huge jump pad boost force and alter the alien's horizontal speed midway in its arc
    private IEnumerator executeJumpPadLaunchAtRightMoment(AIAction action)
    {
        jumpPadBoost();

        yield return new WaitForSeconds(UnityEngine.Random.Range(action.TimeB4Change.x, action.TimeB4Change.y));
        if (!isCreatureStillExecutingThisAction(action))
            yield break;

        randomSpeed = getRandomReasonableSpeed(action.ChangedSpeed);
        dirX = action.DirX * (int)Mathf.Sign(randomSpeed);
        speed = Mathf.Abs(randomSpeed);
    }

    protected override void LateUpdate() 
    {
        base.LateUpdate();
        setAlienVelocity();
    }

    // handles setting the alien velocity on slopes, while falling, etc.
    private void setAlienVelocity()
    {
        // nullify the slight bounce on a slope glitch when changing slopes
        if ((!phaseTracker.IsMidAir || phaseTracker.Is(Phase.Falling)) && rig.velocity.y > 0)
            rig.velocity = new Vector2(0, 0);

        // when alien is on the ground, alien velocity is parallel to the slanted ground 
        if (!phaseTracker.IsMidAir && isGrounded && isTouchingMap)
        {
            if (ActionProgress == Status.Ended && wallToTheLeft) 
                dirX = 1;
            else if (ActionProgress == Status.Ended && wallToTheRight)
                dirX = -1;

            rig.velocity = groundSlope * speed * dirX;
            rig.gravityScale = (dirX == 0) ? 0f : maxGravity;
        }
        
        // when alien is not on the ground (falling or midair after a jump)
        else 
        {
            // Stop moving horizontally if the AI is about to crash into a wall after a jump.
            if (rig.velocity.y > 0 && ((dirX == 1 && wallToTheRight) || (dirX == -1 && wallToTheLeft))) 
                rig.velocity = new Vector2(0, rig.velocity.y);

            // Change directions if the AI is falling left or right and about to crash into a wall.
            else if ((dirX == 1 && wallToTheRight) || (dirX == -1 && wallToTheLeft))
            {
                rig.velocity = new Vector2(0, rig.velocity.y);
                dirX = (int) -Mathf.Sign(dirX) * UnityEngine.Random.Range(0,2);
                speed = 22;
                ActionProgress = Status.Ended;
            }

            // Set velocity to the left/right with specified speed and direction. Vertical motion is affected by gravity
            else
                rig.velocity = new Vector2(speed * dirX, rig.velocity.y);

            rig.gravityScale = maxGravity;
        }
    }

    // changes velocity after a given delay if you're still on the same action
    private IEnumerator changeVelocityAfterDelay(Vector2 delay, Vector2 velocity, AIAction action)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(delay.x, delay.y));
        if (!isCreatureStillExecutingThisAction(action))
            yield break;

        float randomSpeed = getRandomReasonableSpeed(velocity);
        dirX = currAction.DirX * (int)Mathf.Sign(randomSpeed);
        speed = Mathf.Abs(randomSpeed);
    }



    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == Layer.DecisionZone)
            decisionZonesCreatureIsTouching.Add(col.transform);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == Layer.DecisionZone)
            decisionZonesCreatureIsTouching.Remove(col.transform);
    }
}

public enum Status
{
    PendingStart,
    Started,
    InProgress,
    Ended
}
