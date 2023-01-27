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
    public Vector2 speedRange = new Vector2(25f, 25f);
    public Vector2 timeB4Change = new Vector2(1f, 1f);
    public Vector2 changedSpeed = new Vector2(25f, 25f);
    public Vector2 timeB4SecondChange = new Vector2(1f, 1f);
    public Vector2 secondChangedSpeed = new Vector2(25f, 25f);

    private float mass = 1f;
    private float defaultGravity = -32.5f;
    private float gravity;

    [Header("Other Settings")]
    public Vector2 jumpBounds = new Vector2(-1f, -1f);
    public int considerationWeight = 1;
    public float timeShown = 3f;

    [Header("Connected Zone")]
    public int chainedDecisionZone = -1;

    private float yVelocity;
    private float x;
    private float y;

    private const float epsilon = 0.001f;
    //private Vector2 lastPointPlotted;
    //private Vector2 lastP_b4DirSwitch;

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        defaultGravity = CentralController.Gravity * -13f;

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
            drawJumpPadArc();
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
            drawNormalJump();
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
        gravity = 0;

        float yVelocity = transform.parent.right.y / transform.parent.right.x * (float)movementDirX * speedRange.x;
        VerticalInfo v = new VerticalInfo(transform.position.y, 0, yVelocity, 0);

        drawTrajectory(5f, new Info(transform.position, 0.8f, speedRange.x, v));
    }

    // draws two normal jump trajectories on the scene view, one for each end of the speedRange
    private void drawNormalJump()
    {
        gravity = defaultGravity;

        VerticalInfo v = new VerticalInfo(transform.position.y, 0, 0, CentralController.JumpForce);
        Info midwayInfo = drawTrajectory(20f, new Info(transform.position, timeB4Change.x, speedRange.x, v));

        v.TimeElapsed = midwayInfo.Y.TimeElapsed;
        drawTrajectory(20f, new Info(midwayInfo.Pos, timeShown, changedSpeed.x, v));
    }

    // draws the double jump journey on the scene view. first draws the normal jump up till right b4 
    // the double jump happens, then draws the double jump
    private void drawDoubleJump(float timeB4Change)
    {
        gravity = defaultGravity;

       // Vector2 pointRightB4DoubleJumping = drawTrajectory(timeB4DoubleJump, 20f, transform.position, speedRange.x, jumpForce);
        //Vector2 pointRightB4VelocityChange = drawTrajectory(timeB4SecondChange.x, 20f, pointRightB4DoubleJumping, changedSpeed.x, doubleJumpForce);
       // plotJourneyAfterChangingSpeedOnce(timeShown, pointRightB4VelocityChange, pointRightB4DoubleJumping, changedSpeed.x, timeB4SecondChange.x, secondChangedSpeed.x, doubleJumpForce);
    }
    
    // draws the fall down arc journey on the scene view. first draws the fall down path till right b4
    // the AI changes its x velocity, then it draws the new fall down path
    private void drawFallDownArc(float timeB4Change)
    {
        this.gravity = defaultGravity;

        VerticalInfo v = new VerticalInfo(transform.position.y, 0, 0, 0);
        Info midwayInfo = drawTrajectory(20f, new Info(transform.position, timeB4Change, speedRange.x, v));

        v.TimeElapsed = midwayInfo.Y.TimeElapsed;
        drawTrajectory(20f, new Info(midwayInfo.Pos, timeShown, changedSpeed.x, v));
    }

    // draws the jump pad boost journey on the scene view. first draws the upwards path till right b4 the
    // AI changes its x velocity, then it draws this new path
    private void drawJumpPadArc()
    {
        gravity = defaultGravity;

        VerticalInfo v = new VerticalInfo(transform.position.y, 0, 0, CentralController.JumpPadForce);
        Info midwayInfo = drawTrajectory(20f, new Info(transform.position, timeB4Change.x, speedRange.x, v));

        v.TimeElapsed = midwayInfo.Y.TimeElapsed;
        drawTrajectory(20f, new Info(midwayInfo.Pos, timeShown, changedSpeed.x, v));
    }

    //----------------------------------------------------------------------------------------------------------------
    //--------------------------- HELPER METHODS TO MATHEMATICALLY CALCULATE THE ARC  --------------------------------
    //----------------------------------------------------------------------------------------------------------------

    // returns the position the AI will be at the given time assuming the AI started at the initial position
    // with the given horizontal speed, vertical speed and initial jump force
    private Vector2 calculatePosition(Info info, float timeElapsed)
    {
        float xPos = info.Pos.x;
        float speed = info.XVelocity;
        float time = timeElapsed;

        x = xPos + movementDirX * (speed * time);

        float yPos = info.Y.InitPos;
        speed = info.Y.Velocity + info.Y.Force * 0.02f / mass;
        time = info.Y.TimeElapsed + timeElapsed;

        y =  yPos + speed * time + 0.5f * gravity * time * time;

        return new Vector2(x, y);
    }

    // Draws trajectory on scene view quite accurately by using line segments. The trajectory requires info including 
    // the creature's initial x and y position, initial horizontal and vertical speed, initial vertical force, etc.
    // This function also requires how many line segments to draw per second of trajectory time
    // (more line segments used = better curves drawn). Returns information about the end position and
    // total time elapsed 
    private Info drawTrajectory(float linesPerSecond, Info info)
    {
        float increment = 1f / linesPerSecond;
        Vector2 prevPointPosition = info.Pos;

        float time = 0;
        for (;time <= info.Duration + epsilon; time += increment)
        {
            if (movementDirX == 1)
                Gizmos.color = time % (2 * increment) < epsilon ? Color.blue : Color.green;
            else
                Gizmos.color = time % (2 * increment) < epsilon ? Color.red : Color.magenta;

            Vector2 currPointPosition = calculatePosition(info, time);
            Gizmos.DrawLine(prevPointPosition, currPointPosition);
            prevPointPosition = currPointPosition;
        }

        VerticalInfo v = info.Y;
        v.TimeElapsed = info.Y.TimeElapsed + time - increment;
        return new Info(prevPointPosition, 0, 0, v);
    }

    private struct Info
    {
        public VerticalInfo Y;
        public Vector2 Pos;
        public float Duration;
        public float XVelocity;

        public Info(Vector2 pos, float duration, float xVelocity, VerticalInfo y)
        {
            Y = y;
            Duration = duration;
            Pos = pos;
            XVelocity = xVelocity;
        }
    }

    private struct VerticalInfo
    {
        public float InitPos;
        public float TimeElapsed;
        public float Velocity;
        public float Force;

        public VerticalInfo(float initPos, float timeElapsed, float yVelocity, float yForce)
        {
            InitPos = initPos;
            TimeElapsed = timeElapsed;
            Velocity = yVelocity;
            Force = yForce;
        }
    }

    private struct SpeedTime
    {
        public float Time;
        public float Speed;
    }
}