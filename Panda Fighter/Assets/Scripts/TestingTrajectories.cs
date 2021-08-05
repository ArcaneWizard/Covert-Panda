using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class TestingTrajectories : MonoBehaviour
{
    public int movementDirX = 1;

    private float jumpForce = 1130f;
    private float doubleJumpForce = 1260f;
    public Vector2 speedRange = new Vector2(10f, 10f);
    public Vector2 timeB4DoubleJump = new Vector2(0f, 0f);
    public float doubleJumpSpeed;
    private float mass = 1f;

    private float defaultGravity = -24.7f;
    private float gravity;

    public float lengthShown = 14;
    private float length;

    public bool headStraight = false;
    public bool doubleJump = false;

    [HideInInspector]
    public Vector2 endPoint;

    private float yVelocity;
    private float x;
    private float y;
    private Vector2 lastP;
    private Vector2 lastP_b4DoubleJump;

    private void Awake()
    {
        endPoint = transform.GetChild(0).position;
    }

    private void OnDrawGizmosSelected()
    {
        if (headStraight)
            drawNormalJumpParabola(0, 5);

        else if (!doubleJump)
            drawNormalJumpParabola(defaultGravity, lengthShown);

        else if (doubleJump)
        {
            drawDoubleJumpParabola(timeB4DoubleJump.x);
            drawDoubleJumpParabola(timeB4DoubleJump.y);
        }

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.GetChild(0).position, 0.25f);
    }

    Vector2 sampleParabola(Vector2 start, float time, float speed, float jumpForce)
    {
        x = start.x + speed * time * movementDirX;

        yVelocity = !headStraight ? (jumpForce * 0.02f / mass) : transform.parent.right.y / transform.parent.right.x * (float)movementDirX * speed;
        y = start.y + yVelocity * time + 0.5f * gravity * time * time;

        return new Vector2(x, y);
    }

    private void drawNormalJumpParabola(float gravity, float lengthShown)
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
        drawParabola(timeB4DoubleJump * 5 + 0.01f, transform.position, speedRange.x, jumpForce);

        lastP_b4DoubleJump = lastP;
        drawParabola(length, lastP_b4DoubleJump, doubleJumpSpeed, doubleJumpForce);
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
}
