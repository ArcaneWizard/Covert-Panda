
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Controller : CentralController
{
    // the current action the AI is executing (null if no action is being executed right now) 
    [field: SerializeField] public AIAction currAction { get; private set; }

    // the decision zone that provided the current action the AI should execute
    private Transform decisionZone;

    private bool hasBegunAction;
    private HashSet<Transform> decisionZonesNearby;

    protected override void Start()
    {
        base.Start();

        dirX = UnityEngine.Random.Range(0, 2) * 2 - 1;
        speed = MaxSpeed;
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
        hasBegunAction = false;
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
        if (health.IsDead)
        {
            isTouchingMap = false;
            currAction.Exit();
            currAction = null;
            decisionZone = null;
            decisionZonesNearby.Clear();
            return;
        }
        // AI moves at max speed when not executing an action
        if (currAction == null)
        {
            speed = MaxSpeed;
            return;
        }

        handleExecutionOfCurrentAction();

        if (hasBegunAction && currAction.Finished)
        {
            currAction.Exit();
            currAction = null;
        }
    }

    protected override void FixedUpdate()
    {
        if (currAction == null || currAction.Finished)
            return;

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

    private void handleExecutionOfCurrentAction()
    {
        if (currAction == null || currAction.Finished)
            return;

        // When the AI is grounded, begin executing it's current action.
        if (isGrounded && isTouchingMap && !hasBegunAction)
        {
            // Do ignore the action if the creature is too far from its decision zone
            if (!decisionZonesNearby.Contains(decisionZone))
            {
                EndAction();
                return;
            }
            
            currAction.Begin(this);
            hasBegunAction = true;
        }

        // run/execute an action until it's finished
        if (hasBegunAction)
        {
            currAction.Run();
            dirX = currAction.DirX;
            speed = currAction.Speed;
        }
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
            rig.gravityScale = (dirX == 0) ? 0f : Gravity;
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

            rig.gravityScale = Gravity;
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