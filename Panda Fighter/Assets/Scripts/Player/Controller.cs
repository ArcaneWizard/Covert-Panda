
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : CentralController
{
    private bool standingOnJumpPad;
    private bool canThrustDown;

    protected override void Update()
    {
        base.Update();

        if (health.IsDead) 
        {
            isTouchingMap = false;
            standingOnJumpPad = false;
            return;
        }

        //use A and D keys for left or right movement
        DirX = 0;
        if (Input.GetKey(KeyCode.D)) 
            DirX++;
        if (Input.GetKey(KeyCode.A)) 
            DirX--;

        //use W and S keys for jumping up or thrusting downwards + allow double jump
        if (Input.GetKeyDown(KeyCode.W)) 
        {
            if (isGrounded && !phaseTracker.Is(Phase.Jumping) && !standingOnJumpPad)
                normalJump();

            else if (phaseTracker.Is(Phase.Jumping)) 
                doubleJump();

            else if (isGrounded && !phaseTracker.Is(Phase.Jumping) && standingOnJumpPad)
                jumpPadBoost();
        }

        if (Input.GetKeyDown(KeyCode.S) && canThrustDown)
        {
            rig.velocity = new Vector2(rig.velocity.x, 0);
            rig.AddForce(new Vector2(0, -JumpForce));
            canThrustDown = false;
        }

        setPlayerVelocity();
    }

    private void setPlayerVelocity()
    {
        //nullify the slight bounce on a slope glitch when changing slopes
        if ((!phaseTracker.IsMidAir || phaseTracker.Is(Phase.Falling)) && rig.velocity.y > 0)
            rig.velocity = new Vector2(0, 0);

        //when player is on the ground, player velocity is parallel to the slanted ground 
        if (!phaseTracker.IsMidAir && isGrounded && isTouchingMap)
        {
            //no x velocity when running into a wall to avoid bounce/fall glitch
            if (DirX == 1 && wallToTheRight)
                rig.velocity = new Vector2(0, 0);

            //no x velocity when running into a wall to avoid bounce/fall glitch
            else if (DirX == -1 && wallToTheLeft)
                rig.velocity = new Vector2(0, 0);

            //player velocity is parallel to the slanted ground
            else 
            {
                float speedMultiplier = phaseTracker.IsWalkingBackwards ? 0.87f : 1f;
                rig.velocity = groundSlope * speed * DirX * speedMultiplier;
            }

            //don't slip on steep slopes
            rig.gravityScale = (DirX == 0) ? 0f : Gravity;

            //allow player to thrust themselves downwards the next time they jump
            canThrustDown = true;
        }

        //when player is not on the ground, player velocity is just left/right with gravity applied
        else
        {
            //no x velocity when running into a wall mid-air to avoid clipping glitch
            if (DirX == 1 && wallToTheRight)
                rig.velocity = new Vector2(0, rig.velocity.y);

            //no x velocity when running into a wall mid-air to avoid clipping glitch
            else if (DirX == -1 && wallToTheLeft)
                rig.velocity = new Vector2(0, rig.velocity.y);

            //player velocity is just left or right (with gravity pulling the player down)
            else
                rig.velocity = new Vector2(speed * DirX, rig.velocity.y);

            rig.gravityScale = Gravity;
        }
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