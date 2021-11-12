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

    [Header("Describe Jump")]
    public int movementDirX = 1;
    private float jumpForce = 1130f;
    private float doubleJumpForce = 1243f;
    public Vector2 speedRange = new Vector2(10f, 10f);
    public Vector2 timeB4Change = new Vector2(0f, 0f);
    public float changedSpeed;
    public Vector2 bonusTrait = new Vector2(0, 0);

    private float mass = 1f;
    private float defaultGravity = -32.5f;
    private float gravity;

    [Header("Other Settings")]
    public int considerationWeight = 1;
    public float lengthShown = 14;
    private float length;

    [Header("Connected Zone")]
    public Transform chainedDirectionZone;

    [HideInInspector]
    public Vector2 endPoint;

    private float yVelocity;
    private float x;
    private float y;
    private Vector2 lastP;
    private Vector2 lastP_b4DoubleJump;
    private Vector2 lastP_b4DirSwitch;


    private void Awake()
    {
        endPoint = transform.GetChild(0).position;
    }

    private void OnDrawGizmosSelected()
    {
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

        else if (!doubleJump)
        {
            transform.name = "Normal Jump";
            drawNormalJumpParabola(defaultGravity, lengthShown, jumpForce);
        }

        else if (doubleJump)
        {
            transform.name = "Double Jump";
            drawDoubleJumpParabola(timeB4Change.x);
            drawDoubleJumpParabola(timeB4Change.y);
        }

        //white sphere on end Point
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.GetChild(0).position, 0.25f);

        //yellow rec on connected decision zone
        if (chainedDirectionZone != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(chainedDirectionZone.position, 0.7f);
        }
    }

    Vector2 sampleParabola(Vector2 start, float time, float speed, float jumpForce)
    {
        x = start.x + speed * time * movementDirX;

        yVelocity = !headStraight ? (jumpForce * 0.02f / mass) : transform.parent.right.y / transform.parent.right.x * (float)movementDirX * speed;
        y = start.y + yVelocity * time + 0.5f * gravity * time * time;

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
        drawParabola(length, lastP_b4DirSwitch, changedSpeed, doubleJumpForce);
    }

    private void drawFallDownCurveParabola(float timeB4DirSwitch)
    {
        this.gravity = defaultGravity;
        this.length = lengthShown;

        lastP = transform.position;
        drawParabolaWithFasterFrames(timeB4DirSwitch * 5f + 0.01f, transform.position, speedRange.x, 0);

        lastP_b4DirSwitch = lastP;
        drawParabolaWithTimeOffset(length, lastP_b4DirSwitch, changedSpeed, 0, timeB4DirSwitch * 5f);
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

    //Converts trajectory info to a condensed, easy to read form (of type AI_ACTION)
    public AI_ACTION convertToAction()
    {
        AI_ACTION action;
            
            if (headStraight)
                action = defineAction("keepWalking");
            else if (fallDown)
                action = defineAction("fallDown");
            else if (fallDownCurve)
                action = defineAction("fallDownCurve");
            else if (doubleJump)
                action = defineAction("doubleJump");
            else
                action = defineAction("normalJump");
        
        return action;
    }
    
    //helper method for converting trajectory info to a condensed readable form
    private AI_ACTION defineAction(string actionName) => new AI_ACTION(actionName, movementDirX,
            speedRange, timeB4Change, changedSpeed, bonusTrait, transform.GetChild(0).position);

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
    public float changedSpeed { get; private set; }
    public string action { get; private set; }
    public Vector2 endLocation { get; private set; }
    public Vector2 bonusTrait { get; private set; }

    public AI_ACTION(string action, int direction, Vector2 speed, Vector2 timeB4Change, float changedSpeed, Vector2 bonusTrait, Vector2 endLocation)
    {
        this.action = action;
        this.dirX = direction;
        this.speed = speed;
        this.timeB4Change = timeB4Change;
        this.changedSpeed = changedSpeed;
        this.endLocation = endLocation;
        this.bonusTrait = bonusTrait;
    }
}

