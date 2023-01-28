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
    public int dirX = 1;
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
    public Vector2 Bounds = new Vector2(-1f, -1f);
    public int considerationWeight = 1;
    public float lingerTime = 3f;

    [Header("Connected Zone")]
    public int chainedDecisionZone = -1;

    private const float epsilon = 0.001f;

    private Color[] trajectoryColors;
    private short colorIndex;

    // visually display AI trajectories in the scene view
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        defaultGravity = CentralController.Gravity * -13f;

        mass = 1f;

        if (ActionType == AIActionType.ChangeDirections)
        {
            transform.name = "Change Directions";
            drawStraightLine();
            showGizmoJumpBounds();
        }

        else if (ActionType == AIActionType.Falling)
        {
            transform.name = "Falling";
            drawFallDownArc(timeB4Change.x, speedRange.x);
            drawFallDownArc(timeB4Change.y, speedRange.x);
            drawFallDownArc(timeB4Change.x, speedRange.y);
            drawFallDownArc(timeB4Change.y, speedRange.y);
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
            colorIndex = 3;
            drawDoubleJump(speedRange.x, timeB4Change.x, changedSpeed.x);
            colorIndex = 3;
            drawDoubleJump(speedRange.x, timeB4Change.y, changedSpeed.x);
            colorIndex = 4;
            drawDoubleJump(speedRange.y, timeB4Change.x, changedSpeed.x);
            colorIndex = 4;
            drawDoubleJump(speedRange.y, timeB4Change.y, changedSpeed.x);
            colorIndex = 3;
            drawDoubleJump(speedRange.x, timeB4Change.x, changedSpeed.y);
            colorIndex = 3;
            drawDoubleJump(speedRange.x, timeB4Change.y, changedSpeed.y);
            colorIndex = 4;
            drawDoubleJump(speedRange.y, timeB4Change.x, changedSpeed.y);
            colorIndex = 4;
            drawDoubleJump(speedRange.y, timeB4Change.y, changedSpeed.y);
            showGizmoJumpBounds();
        }

        else if (ActionType == AIActionType.NormalJump)
        {
            transform.name = "Normal Jump";
            colorIndex = 0;
            drawNormalJump(speedRange.x, changedSpeed.x);
            colorIndex = 1;
            drawNormalJump(speedRange.y, changedSpeed.x);
            colorIndex = 0;
            drawNormalJump(speedRange.x, changedSpeed.y);
            colorIndex = 1;
            drawNormalJump(speedRange.y, changedSpeed.y);
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
    
    // visually display AI trajectory even if it's child object is selected (note: this could be removed)
    #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Selection.transforms.Length != 0 && Selection.transforms[0].parent == this.transform)
                OnDrawGizmosSelected();
        }
    #endif

    // gets the Transform of the specified chained zone (each zone is a node with its own trajectories)
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

    // Stores all of this trajectory's info in a form that the AI could execute
    private AIActionInfo actionInfo => new AIActionInfo(dirX,
            speedRange, timeB4Change, changedSpeed, timeB4SecondChange, secondChangedSpeed, jumpBounds, transform.position);

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
    
    // draws a straight line trajectory on the scene view
    private void drawStraightLine()
    {
        gravity = 0;

        float yVelocity = transform.parent.right.y / transform.parent.right.x * (float)dirX * speedRange.x;
        VerticalInfo v = new VerticalInfo(transform.position.y, 0, yVelocity, 0);

        drawTrajectory(5f, new Info(transform.position, 0.8f, speedRange.x, v));
    }

    // draws normal jump trajectories on the scene view
    private void drawNormalJump(float speedRange, float changedSpeed)
    {
        gravity = defaultGravity;

        VerticalInfo v = new VerticalInfo(transform.position.y, 0, 0, CentralController.JumpForce);
        Info midwayInfo = drawTrajectory(20f, new Info(transform.position, timeB4Change.x, speedRange, v));

        v.TimeElapsed = midwayInfo.Y.TimeElapsed;
        drawTrajectory(20f, new Info(midwayInfo.Pos, lingerTime, changedSpeed, v));
    }

    // draws the double jump trajectory on the scene view. first draws the normal jump up till right b4 
    // the double jump happens, then draws the double jump
    private void drawDoubleJump(float speedRange, float timeB4Change, float changedSpeed)
    {
        gravity = defaultGravity;
         gravity = defaultGravity;

        VerticalInfo v = new VerticalInfo(transform.position.y, 0, 0, CentralController.JumpForce);
        Info midwayInfo = drawTrajectory(20f, new Info(transform.position, timeB4Change, speedRange, v));

        v = new VerticalInfo(midwayInfo.Pos.y, 0, 0, CentralController.DoubleJumpForce);
        midwayInfo = drawTrajectory(20f, new Info(midwayInfo.Pos, timeB4SecondChange.x, changedSpeed, v));

        v.TimeElapsed = midwayInfo.Y.TimeElapsed;
        drawTrajectory(20f, new Info(midwayInfo.Pos, lingerTime, secondChangedSpeed.x, v));
    }

    // draws the falling down trajectory on the scene view. first draws the fall down path till right b4
    // the AI changes its x velocity, then it updates the fall path. Likewise when speed is changed a 2nd time
    private void drawFallDownArc(float timeB4Change,float speedRange)
    {
        this.gravity = defaultGravity;

        // initial y velocity depends on slope of platform which is indicated by this object's right vector
        float yVelocity = CentralController.MaxSpeed * transform.right.y * dirX;

        VerticalInfo v = new VerticalInfo(transform.position.y, 0, yVelocity, 0);
        Info midwayInfo = drawTrajectory(20f, new Info(transform.position, timeB4Change, speedRange, v));

        v.TimeElapsed = midwayInfo.Y.TimeElapsed;
        drawTrajectory(20f, new Info(midwayInfo.Pos, lingerTime, changedSpeed.x, v));
    }

    // draws the jump pad boost trajectory on the scene view. first draws the upwards path till right b4 the
    // AI changes its x velocity, then it draws this new path
    private void drawJumpPadArc()
    {
        gravity = defaultGravity;

        VerticalInfo v = new VerticalInfo(transform.position.y, 0, 0, CentralController.JumpPadForce);
        Info midwayInfo = drawTrajectory(20f, new Info(transform.position, timeB4Change.x, speedRange.x, v));

        v.TimeElapsed = midwayInfo.Y.TimeElapsed;
        drawTrajectory(20f, new Info(midwayInfo.Pos, timeB4SecondChange.x, changedSpeed.x, v));

        v.TimeElapsed = midwayInfo.Y.TimeElapsed;
        drawTrajectory(20f, new Info(midwayInfo.Pos, lingerTime, secondChangedSpeed.x, v));
    }

    //----------------------------------------------------------------------------------------------------------------
    //--------------------------- HELPER METHODS TO MATHEMATICALLY CALCULATE THE ARC  --------------------------------
    //----------------------------------------------------------------------------------------------------------------

    // returns the position the AI will be at the specified time elapsed assuming the AI started at the initial position,
    // horizontal and vertical speed, etc. all defined by the provided info
    private Vector2 calculatePosition(Info info, float timeElapsed)
    {
        float xPos = info.Pos.x;
        float speed = info.XVelocity;
        float time = timeElapsed;

        float x = xPos + dirX * (speed * time);

        float yPos = info.Y.InitPos;
        speed = info.Y.Velocity + info.Y.Force * 0.02f / mass;
        time = info.Y.TimeElapsed + timeElapsed;

        float y =  yPos + speed * time + 0.5f * gravity * time * time;

        return new Vector2(x, y);
    }

    // Draws trajectory on scene view quite accurately by using line segments. The trajectory requires info about 
    // the creature's initial x and y position, initial horizontal and vertical speed, initial vertical force, etc.
    // This function also requires how many line segments to draw per second of trajectory time
    // (more line segments used = better curves drawn). Returns info containing the position of the
    // last drawn point on the trajectory, and total duration of the trajectory so far
    private Info drawTrajectory(float linesPerSecond, Info info)
    {
        float increment = 1f / linesPerSecond;
        Vector2 prevPointPosition = info.Pos;

        float time = 0;
        for (;time <= info.Duration + epsilon; time += increment)
        {
            Gizmos.color = randomTrajectoryColor();

            Vector2 currPointPosition = calculatePosition(info, time);
            Gizmos.DrawLine(prevPointPosition, currPointPosition);
            prevPointPosition = currPointPosition;
        }

        VerticalInfo v = info.Y;
        v.TimeElapsed = info.Y.TimeElapsed + time - increment;
        return new Info(prevPointPosition, 0, 0, v);
    }

    private Color randomTrajectoryColor()
    {
        if (trajectoryColors == null || trajectoryColors.Length == 0)
        {
            trajectoryColors = new Color[6];
            trajectoryColors[0] = Color.white;
            trajectoryColors[1] = Color.green;
            trajectoryColors[2] = Color.red;
            trajectoryColors[3] = Color.magenta;
            trajectoryColors[4] = Color.yellow;
            trajectoryColors[5] = Color.blue;
        }

        return trajectoryColors[colorIndex % 6];
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