using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class TestingTrajectories : MonoBehaviour
{
    [Header("Jump Type")]
    public bool headStraight = false;
    public bool doubleJump = false;
    public bool fallDown = false;
    public bool fallDownCurve = false;
    public bool launchPad = false;

    [Header("Describe Jump")]
    public int movementDirX = 1;
    private float jumpForce = 1300f;
    private float doubleJumpForce = 1200f;
    private float launchPadForce = 2500f;
    public Vector2 speedRange = new Vector2(10f, 10f);
    public Vector2 timeB4Change = new Vector2(0f, 0f);
    public Vector2 changedSpeed = new Vector2(0f, 0f);

    private float mass = 1f;
    private float defaultGravity = -32.5f;
    private float gravity;

    [Header("Other Settings")]
    public Vector2 jumpBounds = new Vector2(-1f, -1f);
    public int considerationWeight = 1;
    public float lengthShown = 14;
    private float length;

    [Header("Connected Zone")]
    public int chainedDecisionZone = -1;

    private float yVelocity;
    private float x;
    private float y;
    private Vector2 lastP;
    private Vector2 lastP_b4DoubleJump;
    private Vector2 lastP_b4DirSwitch;

    private void OnDrawGizmosSelected()
    {
        jumpForce = CentralController.jumpForce;
        doubleJumpForce = CentralController.doubleJumpForce;
        launchPadForce = 2500f;
        defaultGravity = -32.5f;

        mass = 1f;

        if (headStraight)
        {
            transform.name = "Head Straight";
            drawNormalJumpParabola(0, 5, jumpForce);
        }

        else if (fallDown)
        {
            transform.name = "Fall Down";
            drawNormalJumpParabola(defaultGravity, lengthShown, 0);
        }

        else if (fallDownCurve)
        {
            transform.name = "Fall Down Curve";
            drawFallDownCurveParabola(timeB4Change.x);
            drawFallDownCurveParabola(timeB4Change.y);
        }

        else if (launchPad)
        {
            transform.name = "Launch Pad";
            drawJumpPadParabola(defaultGravity, lengthShown, launchPadForce);
            showGizmoJumpBounds();
        }

        else if (!doubleJump)
        {
            transform.name = "Normal Jump";
            drawNormalJumpParabola(defaultGravity, lengthShown, jumpForce);
            showGizmoJumpBounds();
        }

        else if (doubleJump)
        {
            transform.name = "Double Jump";
            drawDoubleJumpParabola(timeB4Change.x);
            drawDoubleJumpParabola(timeB4Change.y);
            showGizmoJumpBounds();
        }

        //yellow rec on connected decision zone
        if (chainedDecisionZone != -1)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(getChainedZone().position, 0.5f);
        }
    }

    private Vector2 sampleParabola(Vector2 start, float time, float speed, float jumpForce)
    {
        x = start.x + speed * time * movementDirX;

        yVelocity = !headStraight ? (jumpForce * 0.02f / mass) : transform.parent.right.y / transform.parent.right.x * (float)movementDirX * speed;
        y = start.y + yVelocity * time + 0.5f * gravity * time * time;
        return new Vector2(x, y);
    }

    private Vector2 alteredParabola(Vector2 start, float time, float speed, float newTime, float newXSpeed, float jumpForce)
    {
        x = start.x + speed * time * movementDirX + newXSpeed * newTime * movementDirX;

        yVelocity = !headStraight ? (jumpForce * 0.02f / mass) : transform.parent.right.y / transform.parent.right.x * (float)movementDirX * speed;
        y = start.y + yVelocity * (time + newTime) + 0.5f * gravity * (time + newTime) * (time + newTime);
        return new Vector2(x, y);
    }

    private void drawNormalJumpParabola(float gravity, float lengthShown, float jumpForce)
    {
        this.gravity = gravity;
        this.length = lengthShown;

        lastP = transform.position;
        drawParabola(length, transform.position, speedRange.x, jumpForce);

        lastP = transform.position;
        drawParabola(length, transform.position, speedRange.y, jumpForce);
    }

    private void drawDoubleJumpParabola(float timeB4DoubleJump)
    {
        this.gravity = defaultGravity;
        this.length = lengthShown;

        lastP = transform.position;
        drawParabolaWithFasterFrames(timeB4DoubleJump * 5f + 0.01f, transform.position, speedRange.x, jumpForce);

        lastP_b4DirSwitch = lastP;
        drawParabola(length, lastP_b4DirSwitch, changedSpeed.x, doubleJumpForce);
    }

    private void drawFallDownCurveParabola(float timeB4DirSwitch)
    {
        this.gravity = defaultGravity;
        this.length = lengthShown;

        lastP = transform.position;
        drawParabolaWithFasterFrames(timeB4DirSwitch * 5f + 0.01f, transform.position, speedRange.x, 0);

        lastP_b4DirSwitch = lastP;
        drawParabolaWithTimeOffset(length, lastP_b4DirSwitch, changedSpeed.x, 0, timeB4DirSwitch * 5f);
    }

    private void drawJumpPadParabola(float gravity, float lengthShown, float jumpForce)
    {
        this.gravity = gravity;
        this.length = lengthShown;

        lastP = transform.position;
        for (float i = 0; i <= timeB4Change.x * 5; i++)
        {
            Vector2 p = sampleParabola(transform.position, i / 5f, speedRange.x, jumpForce);

            if (movementDirX == 1)
                Gizmos.color = i % 2 == 0 ? Color.blue : Color.green;
            else
                Gizmos.color = i % 2 == 0 ? Color.red : Color.magenta;

            Gizmos.DrawLine(lastP, p);
            lastP = p;
        }

        for (float i = 0; i <= length; i++)
        {
            Vector2 p = alteredParabola(transform.position, timeB4Change.x, speedRange.x, i / 5f, changedSpeed.x, jumpForce);

            if (movementDirX == 1)
                Gizmos.color = i % 2 == 0 ? Color.blue : Color.green;
            else
                Gizmos.color = i % 2 == 0 ? Color.red : Color.magenta;

            Gizmos.DrawLine(lastP, p);
            lastP = p;
        }
    }

    private void drawParabola(float linesDrawn, Vector2 start, float speed, float jumpForce)
    {
        for (float i = 0; i <= linesDrawn; i++)
        {
            Vector2 p = sampleParabola(start, i / 5, speed, jumpForce);

            if (movementDirX == 1)
                Gizmos.color = i % 2 == 0 ? Color.blue : Color.green;
            else
                Gizmos.color = i % 2 == 0 ? Color.red : Color.magenta;

            Gizmos.DrawLine(lastP, p);
            lastP = p;
        }
    }

    private void drawParabolaWithFasterFrames(float linesDrawn, Vector2 start, float speed, float jumpForce)
    {
        for (float i = 0; i <= linesDrawn; i += 0.25f)
        {
            Vector2 p = sampleParabola(start, i / 5, speed, jumpForce);

            if (movementDirX == 1)
                Gizmos.color = i % 2 == 0 ? Color.blue : Color.green;
            else
                Gizmos.color = i % 2 == 0 ? Color.red : Color.magenta;

            Gizmos.DrawLine(lastP, p);
            lastP = p;
        }
    }

    private void drawParabolaWithTimeOffset(float linesDrawn, Vector2 start, float speed, float jumpForce, float timeAlreadyFallen)
    {
        for (float i = 0; i <= linesDrawn; i += 0.25f)
        {
            Vector2 p = sampleParabola(start, timeAlreadyFallen + i / 5, speed, jumpForce);

            if (movementDirX == 1)
                Gizmos.color = i % 2 == 0 ? Color.blue : Color.green;
            else
                Gizmos.color = i % 2 == 0 ? Color.red : Color.magenta;

            Gizmos.DrawLine(lastP, p);
            lastP = p;
        }
    }

    public Transform getChainedZone() => transform.parent.parent.GetChild(chainedDecisionZone);

    //draw spheres on jump bounds in scene editor
    private void showGizmoJumpBounds()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + new Vector3(jumpBounds.x,
            transform.right.y / transform.right.x * jumpBounds.x, 0), 0.3f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position + new Vector3(jumpBounds.y,
            transform.right.y / transform.right.x * jumpBounds.y, 0), 0.3f);
    }

    //Converts trajectory info to a condensed, easy to read form (of type AI_ACTION)
    public AI_ACTION convertToAction()
    {
        AI_ACTION action;

        if (headStraight)
            action = defineAction("headStraight");
        else if (fallDown)
            action = defineAction("fallDown");
        else if (fallDownCurve)
            action = defineAction("fallDownCurve");
        else if (doubleJump)
            action = defineAction("doubleJump");
        else if (launchPad)
            action = defineAction("launchPad");
        else
            action = defineAction("normalJump");

        return action;
    }

    //helper method for converting trajectory info to a condensed readable form
    private AI_ACTION defineAction(string actionName) => new AI_ACTION(actionName, movementDirX,
            speedRange, timeB4Change, changedSpeed, jumpBounds, transform.position);

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Selection.transforms.Length != 0 && Selection.transforms[0].parent == this.transform)
            OnDrawGizmosSelected();
    }
#endif
}

[System.Serializable]
public struct AI_ACTION
{
    public int dirX { get; private set; }
    public Vector2 speed { get; private set; }
    public Vector2 timeB4Change { get; private set; }
    public Vector2 changedSpeed { get; private set; }
    public string action { get; private set; }
    public Vector2 jumpBounds { get; private set; }

    public AI_ACTION(string action, int direction, Vector2 speed, Vector2 timeB4Change, Vector2 changedSpeed, Vector2 jumpBounds, Vector3 trajectoryPos)
    {
        this.action = action;
        this.dirX = direction;
        this.speed = speed;
        this.timeB4Change = timeB4Change;
        this.changedSpeed = changedSpeed;
        this.jumpBounds = new Vector2(trajectoryPos.x + jumpBounds.x, trajectoryPos.x + jumpBounds.y);
    }

    public override string ToString()
    {
        return $"action: {action}, dirX: {dirX}, speed: {speed}, timeB4CHange: {timeB4Change}" +
        $"changedSpeed: {changedSpeed} + jumpBounds: {jumpBounds}";
    }
}

