
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : CentralController
{
    private int lastDirX;
    private bool needToWalkMinimumDistance;

    private bool standingOnJumpPad;
    private bool canThrustDown;

    void Update()
    {
        if (health.isDead) 
        {
            isTouchingMap = false;
            standingOnJumpPad = false;
            return;
        }

        //use A and D keys for left or right movement
        dirX = 0;
        if (Input.GetKey(KeyCode.D)) 
            dirX++;
        if (Input.GetKey(KeyCode.A)) 
            dirX--;
        
        //always move left/right for at least a full step instead of jittering after quick button taps
       // takeFullStep();

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
            rig.AddForce(new Vector2(0, -jumpForce));
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
            if (dirX == 1 && wallToTheRight)
                rig.velocity = new Vector2(0, 0);

            //no x velocity when running into a wall to avoid bounce/fall glitch
            else if (dirX == -1 && wallToTheLeft)
                rig.velocity = new Vector2(0, 0);

            //player velocity is parallel to the slanted ground
            else 
            {
                float speedMultiplier = phaseTracker.IsWalkingBackwards ? 0.87f : 1f;
                rig.velocity = groundSlope * speed * dirX * speedMultiplier;
            }

            //don't slip on steep slopes
            rig.gravityScale = (dirX == 0) ? 0f : maxGravity;

            //allow player to thrust themselves downwards the next time they jump
            canThrustDown = true;
        }

        //when player is not on the ground, player velocity is just left/right with gravity applied
        else
        {
            //no x velocity when running into a wall mid-air to avoid clipping glitch
            if (dirX == 1 && wallToTheRight)
                rig.velocity = new Vector2(0, rig.velocity.y);

            //no x velocity when running into a wall mid-air to avoid clipping glitch
            else if (dirX == -1 && wallToTheLeft)
                rig.velocity = new Vector2(0, rig.velocity.y);

            //player velocity is just left or right (with gravity pulling the player down)
            else
                rig.velocity = new Vector2(speed * dirX, rig.velocity.y);

            rig.gravityScale = maxGravity;
        }
    }

    // always move left/right for at least a full step instead of jittering after quick button taps
    private void takeFullStep() 
    {
        // when the player suddenly chooses to head left or right and this is different from their last input (idle or diff direction),
        // update that the player needs to walk some minimum distance and set the last input to be the current input
        if (dirX != 0 && lastDirX != dirX)  {
            StartCoroutine(waitForStepToComplete());
            lastDirX = dirX;
        }

        // as long as the player needs to walk some minimum distance, force the direction of movement to be to the last input they gave
        if (needToWalkMinimumDistance) 
            dirX = lastDirX;
        
        // otherwise reset the last input to be not moving
        else if (dirX == 0)
            lastDirX = 0;
    }

    // updates a bool to say that the player needs to walk some minimum distance, and after 0.24 seconds, updates the same bool
    // to convey the player no longer neesd to walk some minimum distance
    private IEnumerator waitForStepToComplete()
    {
        needToWalkMinimumDistance = true;
        yield return new WaitForSeconds(0.1f);
        needToWalkMinimumDistance = false;
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
