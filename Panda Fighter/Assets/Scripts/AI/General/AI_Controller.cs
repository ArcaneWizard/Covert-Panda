
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Controller : CentralController
{
    // the current action the AI is executing (null if no action is being executed right now) 
    public AIAction currAction { get; private set; }

    // the decision zone that provided the current action the AI should execute
    private Transform decisionZone;

    private bool hasStartedAction;
    private HashSet<Transform> decisionZonesNearby;

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

    // Start executing a specified action.
    public void StartAction(AIAction AI_action, Transform zone = null)
    {
        currAction = AI_action;
        decisionZone = zone;
        hasStartedAction = false;
    }

    // End the current action
    public void EndAction() => currAction = null;

    // Check if the specified action is being executed
    public bool IsActionBeingExecuted(AIAction action) => currAction.Equals(action);

    // Make AI head in a specified direction
    public void SetDirection(int dir) => StartAction(AIAction.ChangeDirection(dir));

    void Update()
    {
        // don't do anything if dead
        if (health.isDead)
        {
            isTouchingMap = false;
            currAction = null;
            decisionZone = null;
            decisionZonesNearby.Clear();
            return;
        }

        if (currAction.Finished)
            currAction = null;

        // AI moves at max speed when not executing an action
        if (currAction == null)
        {
            speed = maxSpeed;
            return;
        }

        handleExecutionOfCurrentAction();
    }

    private void handleExecutionOfCurrentAction()
    {
        // When the AI is grounded, start executing the action
        if (isGrounded && isTouchingMap)
        {
            // Invoke the start of the action once (assuming the creature hasn't ventured 
            // too far from that action's decision zone)
            if (!hasStartedAction)
            {
                if (decisionZonesNearby.Contains(decisionZone))
                {
                    speed = maxSpeed;
                    dirX = currAction.Info.DirX;
                    currAction.Begin(this);
                }
                else
                    EndAction();

                hasStartedAction = true;
            }
        }

        // run/execute an action until it's finished
        if (!currAction.Finished)
        {
            currAction.Run();
            dirX = currAction.DirX;
            speed = currAction.Speed;
        }
    }

    protected override void FixedUpdate()
    {
        if (currAction.ExecuteNormalJumpNow)
        {
            normalJump();
            currAction.ExecuteNormalJumpNow = false;
        }
        if (currAction.ExecuteDoubleJumpNow)
        {
            doubleJump();
            currAction.ExecuteDoubleJumpNow = false;
        }
        if (currAction.ExecuteJumpBoostNow)
        {
            jumpPadBoost();
            currAction.ExecuteJumpBoostNow = false;
        }
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
            if (currAction == null && wallToTheLeft)
                dirX = 1;
            else if (currAction == null && wallToTheRight)
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
                dirX = (int)-Mathf.Sign(dirX) * UnityEngine.Random.Range(0, 2);
                speed = 22;
                currAction = null;
            }

            // Set velocity to the left/right with specified speed and direction. Vertical motion is affected by gravity
            else
                rig.velocity = new Vector2(speed * dirX, rig.velocity.y);

            rig.gravityScale = maxGravity;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == Layer.DecisionZone)
            decisionZonesNearby.Add(col.transform);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.layer == Layer.DecisionZone && !decisionZonesNearby.Contains(col.transform))
            decisionZonesNearby.Add(col.transform);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == Layer.DecisionZone)
            decisionZonesNearby.Remove(col.transform);
    }
}