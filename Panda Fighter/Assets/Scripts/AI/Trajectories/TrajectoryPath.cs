using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEditor;
using System;

public class TrajectoryPath : MonoBehaviour
{
    [field: SerializeField] public AIActionType ActionType { get; private set; }

    [Header("Describe Jump")]
    public int movementDirX = 1;
    private float jumpForce;
    private float doubleJumpForce;
    private float launchPadForce;
    public Vector2 speedRange = new Vector2(10f, 10f);
    public Vector2 timeB4Change = new Vector2(0f, 0f);
    public Vector2 changedSpeed = new Vector2(0f, 0f);
    public Vector2 timeB4SecondChange = new Vector2(0f, 0f);
    public Vector2 secondChangedSpeed = new Vector2(0f, 0f);
    private float lastYVelocity;

    private float mass = 1f;
    private float defaultGravity = -32.5f;
    private float gravity;

    [Header("Other Settings")]
    public Vector2 jumpBounds = new Vector2(-1f, -1f);
    public int considerationWeight = 1;
    public float lengthShown = 14;

    [Header("Connected Zone")]
    public int chainedDecisionZone = -1;

    private float yVelocity;
    private float x;
    private float y;
    //private Vector2 lastPointPlotted;
    //private Vector2 lastP_b4DirSwitch;

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        jumpForce = CentralController.jumpForce;
        doubleJumpForce = CentralController.doubleJumpForce;
        launchPadForce = CentralController.jumpPadForce;
        defaultGravity = -65f;

        mass = 1f;

        if (ActionType == AIActionType.ChangeDirections)
        {
            transform.name = "Change Directions";
            drawStraightLine();
        }

        else if (ActionType == AIActionType.Falling)
        {
            transform.name = "Falling";
            drawFallDownArc(timeB4Change.x);
            drawFallDownArc(timeB4Change.y);
        }

        else if (ActionType == AIActionType.LaunchPad)
        {
            transform.name = "Launch Pad";
            drawJumpPadArc(defaultGravity, lengthShown, launchPadForce);
            showGizmoJumpBounds();
        }

        else if (ActionType == AIActionType.DoubleJump)
        {
            transform.name = "Double Jump";
            drawDoubleJump(timeB4Change.x);
            drawDoubleJump(timeB4Change.y);
            showGizmoJumpBounds();
        }

        else if (ActionType == AIActionType.NormalJump)
        {
            transform.name = "Normal Jump";
            drawNormalJump(defaultGravity, lengthShown, jumpForce, speedRange.x);
            drawNormalJump(defaultGravity, lengthShown, jumpForce, speedRange.y);
            showGizmoJumpBounds();
        }

        else
            Debug.LogError("The specified trajectory doesn't have any visuals");

        // put a green dot on neighboring decision zones
        if (chainedDecisionZone != -1)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(getChainedZone().position, 0.5f);
        }
    }
    #endif

    public Transform getChainedZone() => transform.parent.parent.GetChild(chainedDecisionZone);

    // converts the trajectory into a performable AI Action
    public AIAction ConvertToAction()
    {
        AIAction action = null;

        if (ActionType == AIActionType.ChangeDirections)
            action = new ChangeDirectionsAction(ActionType, actionInfo);

        else if (ActionType == AIActionType.NormalJump)
            action = new NormalJumpAction(ActionType, actionInfo);

        else if (ActionType == AIActionType.DoubleJump)
            action = new DoubleJumpAction(ActionType, actionInfo);

       else if (ActionType == AIActionType.Falling)
            action = new FallingAction(ActionType, actionInfo);

       else if (ActionType == AIActionType.LaunchPad)
            action = new LaunchPadAction(ActionType, actionInfo);

        return action;
    }

    // Stores all of this trajectory's info into an Action Info object
    private AIActionInfo actionInfo => new AIActionInfo(movementDirX,
            speedRange, timeB4Change, changedSpeed, timeB4SecondChange, secondChangedSpeed, jumpBounds, transform.position);


    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Selection.transforms.Length != 0 && Selection.transforms[0].parent == this.transform)
            OnDrawGizmosSelected();
    }
    #endif

    // draw spheres on jump bounds in the scene editor
    private void showGizmoJumpBounds()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + new Vector3(jumpBounds.x,
            transform.right.y / transform.right.x * jumpBounds.x, 0), 0.3f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position + new Vector3(jumpBounds.y,
            transform.right.y / transform.right.x * jumpBounds.y, 0), 0.3f);
    }


    //----------------------------------------------------------------------------------------------------------------
    //---------------------------------- DRAW DIFF TYPES OF TRAJECTORIES ---------------------------------------------
    //----------------------------------------------------------------------------------------------------------------
    
    // draws a straight line journey on the scene view
    private void drawStraightLine()
    {
        this.gravity = 0;
        
        Vector2 pointRightB4VelocityChange = plotJourney(4, transform.position, speedRange.x, 0);
    }

    // draws a normal jump journey on the scene view
    private void drawNormalJump(float gravity, float lengthShown, float jumpForce, float speed)
    {
        this.gravity = gravity;

        Vector2 pointRightB4VelocityChange = plotJourneyVeryAccurately(timeB4Change.x * 5f, transform.position, speed, jumpForce);
        plotJourneyAfterChangingSpeedOnce(lengthShown, pointRightB4VelocityChange, transform.position, speed, timeB4Change.x, changedSpeed.x, jumpForce);
    }

    // draws the double jump journey on the scene view. first draws the normal jump up till right b4 
    // the double jump happens, then draws the double jump
    private void drawDoubleJump(float timeB4DoubleJump)
    {
        this.gravity = defaultGravity;

        Vector2 pointRightB4DoubleJumping = plotJourneyVeryAccurately(timeB4DoubleJump * 5f, transform.position, speedRange.x, jumpForce);
        Vector2 pointRightB4VelocityChange = plotJourneyVeryAccurately(timeB4SecondChange.x * 5f, pointRightB4DoubleJumping, changedSpeed.x, doubleJumpForce);
        plotJourneyAfterChangingSpeedOnce(lengthShown, pointRightB4VelocityChange, pointRightB4DoubleJumping, changedSpeed.x, timeB4SecondChange.x, secondChangedSpeed.x, doubleJumpForce);
    }
    
    // draws the fall down arc journey on the scene view. first draws the fall down path till right b4
    // the AI changes its x velocity, then it draws the new fall down path
    private void drawFallDownArc(float timeB4DirSwitch)
    {
        this.gravity = defaultGravity;

        Vector2 pointRightB4ChangingSpeedHorizontally = plotJourneyVeryAccurately(timeB4DirSwitch * 5f + 0.01f, transform.position, speedRange.x, 0);
        plotJourneyWithTimeOffset(lengthShown, pointRightB4ChangingSpeedHorizontally, changedSpeed.x, 0, timeB4DirSwitch * 5f);
    }

    // draws the jump pad boost journey on the scene view. first draws the upwards path till right b4 the
    // AI changes its x velocity, then it draws this new path
    private void drawJumpPadArc(float gravity, float lengthShown, float jumpForce)
    {
        this.gravity = gravity;

        Vector2 pointB4ChangingSpeedOnce = plotJourneyVeryAccurately(timeB4Change.x * 5, transform.position, speedRange.x, jumpForce);
        Vector2 pointB4ChangingSpeedTwice = plotJourneyAfterChangingSpeedOnce(timeB4SecondChange.x * 5, pointB4ChangingSpeedOnce, transform.position, speedRange.x,
            timeB4Change.x, changedSpeed.x, jumpForce);
        plotJourneyAfterChangingSpeedTwice(lengthShown, pointB4ChangingSpeedTwice, transform.position, speedRange.x, timeB4Change.x, changedSpeed.x, timeB4SecondChange.x,
            secondChangedSpeed.x, jumpForce);
    }

    //----------------------------------------------------------------------------------------------------------------
    //--------------------------- HELPER METHODS TO MATHEMATICALLY CALCULATE THE ARC  --------------------------------
    //----------------------------------------------------------------------------------------------------------------
    
    // returns the position the AI assuming the AI had the given horizontal speed and jump force at
     // the given start position, and traveled at that speed for some time 
    private Vector2 pointAlongShortJourney(float time, Vector2 start, float speed, float jumpForce)
    {
        x = start.x + speed * time * movementDirX;

        yVelocity = !(ActionType == AIActionType.ChangeDirections) 
            ? (jumpForce * 0.02f / mass) 
            : transform.parent.right.y / transform.parent.right.x * (float)movementDirX * speed;
        y = start.y + yVelocity * time + 0.5f * gravity * time * time;
        return new Vector2(x, y);
    }

    // returns the position the AI will be assuming the AI had the given horizontal speed and jump force at
    // the given start position, traveled at that speed for some time (speed / time), changed it's speed midair and
    // continued at that for a few seconds (speed 2 / time 2)
    private Vector2 pointAlongMediumJourney(float time, Vector2 start, float speed, float time2, float speed2, float jumpForce)
    {
        x = start.x + movementDirX * (speed * time + speed2 * time2);

        yVelocity = !(ActionType == AIActionType.ChangeDirections) 
            ? (jumpForce * 0.02f / mass) 
            : transform.parent.right.y / transform.parent.right.x * (float)movementDirX * speed;
        y = start.y + yVelocity * (time + time2) + 0.5f * gravity * (time + time2) * (time + time2);
        return new Vector2(x, y);
    }

     // returns the position the AI will be at after a given time, assuming the AI had the given horizontal speed and jump force at
     // the given start position,  traveled at that speed for some time (speed / time), changed it's speed midair and continued
     // at that for an additional few seconds (speed 2 / time 2), and later changed it's speed for a second time + continued at that 
     // for a few seconds (speed 3 / time 3)
    private Vector2 pointAlongLongJourney(float time, Vector2 start, float speed, float time2, float speed2, float time3, float speed3, float jumpForce)
    {
        x = start.x + movementDirX * (speed * time + speed2 * time2 + speed3 * time3);

        yVelocity = !(ActionType == AIActionType.ChangeDirections) 
            ? (jumpForce * 0.02f / mass) 
            : transform.parent.right.y / transform.parent.right.x * (float)movementDirX * speed;
        y = start.y + yVelocity * (time + time2 + time3) + 0.5f * gravity * (time + time2 + time3) * (time + time2 + time3);
        return new Vector2(x, y);
    }

    //----------------------------------------------------------------------------------------------------------------
    //----------------------------- HELPER METHOS TO PLOT THE ARC ON THE SCENE VIEW  ---------------------------------
    //----------------------------------------------------------------------------------------------------------------

    // draws journey on scene view using medium length line segments. Takes in a start position, initial velocity, initial jump force, and how
    // many segments to draw b4 stopping. The segements will be blue/green if the journey goes to the right, and red/magenta if it goes to the left
    private Vector2 plotJourney(float linesDrawn, Vector2 start, float speed, float jumpForce)
    {
        Vector2 lastPointPlotted = start;
        for (float i = 0; i <= linesDrawn; i++)
        {
            if (movementDirX == 1)
                Gizmos.color = i % 2 == 0 ? Color.blue : Color.green;
            else
                Gizmos.color = i % 2 == 0 ? Color.red : Color.magenta;

            Vector2 nextPointPlotted = pointAlongShortJourney(i / 5f, start, speed, jumpForce);
            Gizmos.DrawLine(lastPointPlotted, nextPointPlotted);
            lastPointPlotted = nextPointPlotted;
        }

        return lastPointPlotted;
    }

    // draws journey on scene view quite accurately by using tiny line segments. Takes in a start position, initial velocity, initial jump 
    // force, and how many segments to draw b4 stopping. The segements will be blue/green if the journey goes to the right, and red/magenta 
    // if it goes to the left
    private Vector2 plotJourneyVeryAccurately(float linesDrawn, Vector2 start, float speed, float jumpForce)
    {
        Vector2 lastPointPlotted = start;
        for (float i = 0; i <= linesDrawn; i += 0.25f)
        {
            if (movementDirX == 1)
                Gizmos.color = i % 0.5f == 0 ? Color.blue : Color.green;
            else
                Gizmos.color = i % 0.5f == 0 ? Color.red : Color.magenta;

            Vector2 nextPointPlotted = pointAlongShortJourney(i / 5f, start, speed, jumpForce);
            Gizmos.DrawLine(lastPointPlotted, nextPointPlotted);
            lastPointPlotted = nextPointPlotted;
        }

        return lastPointPlotted;
    }

    // draws journey on scene view, given a start position, initial velocity, initial jump force, and
    // how many segments to draw b4 stopping. Takes into account the time already fallen to figure out
    // the AI's vertical velocity at the start. Segments will be blue/green if the journey heads right 
    // or red/magenta if the journey heads Returns the point it stopped plotting at
    private Vector2 plotJourneyWithTimeOffset(float linesDrawn, Vector2 start, float speed, float jumpForce, float timeAlreadyFallen)
    {
        Vector2 lastPointPlotted = start;
        for (float i = 0; i <= linesDrawn; i += 0.25f)
        {
            if (movementDirX == 1)
                Gizmos.color = i % 2 == 0 ? Color.blue : Color.green;
            else
                Gizmos.color = i % 2 == 0 ? Color.red : Color.magenta;

            Vector2 nextPointPlotted = pointAlongShortJourney(timeAlreadyFallen + i / 5, start, speed, jumpForce);
            Gizmos.DrawLine(lastPointPlotted, nextPointPlotted);
            lastPointPlotted = nextPointPlotted;
        }

        return lastPointPlotted;
    }

    // draws journey on scene view using medium length line segments. Takes in a how many segments to draw b4 stopping,
    // the point to continue plotting from and parameters of the pointAlongMediumJourney() method. Segements will be blue/green 
    // if the journey heads right or red/magenta if the journey heads left. Returns the point it stopped plotting at
    private Vector2 plotJourneyAfterChangingSpeedOnce(float linesDrawn, Vector2 lastPointPosition, Vector2 start, float speed, float time, float speed2,
        float jumpForce)
    {
        Vector2 lastPointPlotted = lastPointPosition;
        for (float i = 0; i <= linesDrawn; i++)
        {
            if (movementDirX == 1)
                Gizmos.color = i % 2 == 0 ? Color.blue : Color.green;
            else
                Gizmos.color = i % 2 == 0 ? Color.red : Color.magenta;

            Vector2 newPointPlotted = pointAlongMediumJourney(time, start, speed, i/5f, speed2, jumpForce);
            Gizmos.DrawLine(lastPointPlotted, newPointPlotted);
            lastPointPlotted = newPointPlotted;
        }

        return lastPointPlotted;
    }

    // draws journey on scene view using medium length line segments. Takes in a how many segments to draw b4 stopping,
    // the point to continue plotting from and parameters of the pointAlongLongJourney() method. Segements will be blue/green 
    // if the journey heads right or red/magenta if the journey heads left. Returns the point it stopped plotting at
    private Vector2 plotJourneyAfterChangingSpeedTwice(float linesDrawn, Vector2 lastPointPosition, Vector2 start, float speed, float time, float speed2, 
        float time2, float speed3, float jumpForce)
    {
        Vector2 lastPointPlotted = lastPointPosition;
        for (float i = 0; i <= linesDrawn; i++)
        {
            if (movementDirX == 1)
                Gizmos.color = i % 2 == 0 ? Color.blue : Color.green;
            else
                Gizmos.color = i % 2 == 0 ? Color.red : Color.magenta;

            Vector2 newPointPlotted = pointAlongLongJourney(time, start, speed, time2, speed2, i/5f, speed3, jumpForce);
            Gizmos.DrawLine(lastPointPlotted, newPointPlotted);
            lastPointPlotted = newPointPlotted;
        }

        return lastPointPlotted;
    }
}