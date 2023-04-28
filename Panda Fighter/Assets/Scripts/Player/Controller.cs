
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : CentralController
{
    private bool standingOnJumpPad;
    private bool canThrustDown;
    private float fastestYVelocityRecorded;

    public float a = 0.4f;
    public float b = 100f;
    public float c = 12f;

    [SerializeField] private CameraMovement cameraMovement;

    protected override void Start()
    {
        base.Start();
        canThrustDown = true;
    }

    protected override void Update()
    {
        base.Update();

        if (health.IsDead) 
        {
            isTouchingMap = false;
            standingOnJumpPad = false;
            return;
        }

        //use W and S keys for jumping up or thrusting downwards + allow double jump
        if (Input.GetKeyDown(KeyCode.W)) 
        {
            if (isGrounded && !phaseTracker.Is(Phase.Jumping) && !standingOnJumpPad)
                StartCoroutine(normalJump());

            else if (phaseTracker.Is(Phase.Jumping)) 
                StartCoroutine(doubleJump());

            else if (isGrounded && !phaseTracker.Is(Phase.Jumping) && standingOnJumpPad)
                StartCoroutine(jumpPadBoost());
        }

        if (Input.GetKeyDown(KeyCode.S) && canThrustDown && phaseTracker.IsMidAir)
        {
            rig.velocity = new Vector2(rig.velocity.x, 0);
            rig.AddForce(new Vector2(0, DOWNWARDS_THRUST_FORCE));
            canThrustDown = false;
            fastestYVelocityRecorded = 0;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        //use A and D keys for left or right movement
        DirX = 0;
        if (Input.GetKey(KeyCode.D))
            DirX++;
        if (Input.GetKey(KeyCode.A))
            DirX--;

        setPlayerVelocity();

        if (rig.velocity.y < fastestYVelocityRecorded)
            fastestYVelocityRecorded = rig.velocity.y;
    }

    private void setPlayerVelocity()
    {
        //nullify the slight bounce on a slope glitch when changing slopes
        //if ((!phaseTracker.IsMidAir || phaseTracker.Is(Phase.Falling)) && rig.velocity.y > 0)
          //  rig.velocity = new Vector2(0, 0);

        //when player is on the ground, player velocity is parallel to the slanted ground 
        if (!phaseTracker.IsMidAir && isGrounded && isTouchingMap)
        {
            // if running forwards into a wall, kill x velocity
            if (wallInFront && ((DirX == 1 && lookAround.IsFacingRight) || (DirX == -1 && !lookAround.IsFacingRight)))
            {
                rig.velocity = new Vector2(0, rig.velocity.y);
            }

            // if walking backwards into a wall, kill x velocity
            else if (wallBehind && ((DirX == 1 && !lookAround.IsFacingRight) || (DirX == -1 && lookAround.IsFacingRight)))
            {
                rig.velocity = new Vector2(0, rig.velocity.y);
            }

            //else player velocity is parallel to the slanted ground
            else if (!recentlyJumpedOffGround && !recentlyDoubleJumpedOffGround)
            {
                float speedMultiplier = phaseTracker.IsWalkingBackwards ? 0.87f : 1f;
                addForce(groundSlope * speed * DirX * speedMultiplier);
                // rig.velocity = groundSlope * speed * DirX * speedMultiplier;
                rig.gravityScale = (DirX == 0) ? 0f : GRAVITY;
            }

            else
            {
                float speedMultiplier = phaseTracker.IsWalkingBackwards ? 0.87f : 1f;
                addForce(new Vector2(speed * DirX * speedMultiplier, rig.velocity.y));
                rig.gravityScale = GRAVITY;
            }

            //camera shakes if landing from a downwards thrust
            if (!canThrustDown && fastestYVelocityRecorded < -20f)
            {
                float shakeMultiplier = a + (-fastestYVelocityRecorded - 40f) / b;
                cameraMovement.ExecuteCameraShake(shakeMultiplier);
            }

            //allow player to thrust themselves downwards the next time they jump
            canThrustDown = true;
        }

        //when player is not on the ground, player velocity is just left/right with gravity applied
        else
        {
            // if running forwards into a wall, do nothing
            if (wallInFront && ((DirX == 1 && lookAround.IsFacingRight) || (DirX == -1 && !lookAround.IsFacingRight)))
            {
                rig.velocity = new Vector2(0, rig.velocity.y);
            }

            // if walking backwards into a wall, do nothing
            else if (wallBehind && ((DirX == 1 && !lookAround.IsFacingRight) || (DirX == -1 && lookAround.IsFacingRight)))
            {
                rig.velocity = new Vector2(0, rig.velocity.y);
            }
            else
            {
                 addForce(new Vector2(speed * DirX, rig.velocity.y));
            }

            rig.gravityScale = GRAVITY;
        }
    }

    private void addForce(Vector2 velocity)
    {
        rig.AddForce((velocity * rig.mass - rig.velocity * rig.mass) / 0.02f);
    }

    private void OnTriggerEnter2D(Collider2D col) 
    {
        if (col.gameObject.layer == Layer.JumpPad)
            standingOnJumpPad = true;
    }

    private void OnTriggerExit2D(Collider2D col) 
    {
        if (col.gameObject.layer == Layer.JumpPad)
            standingOnJumpPad = false;
    }
}