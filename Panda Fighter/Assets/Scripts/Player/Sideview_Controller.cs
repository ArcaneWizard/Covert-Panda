
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Sideview_Controller : CentralController
{
    private int frames;
    private float time;


    [Range(0, 10)]
    public float gravityCounter;

    public override void Update()
    {
        base.Update();

        //use A and D keys for left or right movement
        dirX = 0;
        if (Input.GetKey(KeyCode.D))
            dirX++;
        if (Input.GetKey(KeyCode.A))
            dirX--;

        //use W and S keys for jumping up or thrusting downwards + allow double jump
        if (Input.GetKeyDown(KeyCode.W) && animator.GetBool("jumped") && !animator.GetBool("double jump"))
        {
            rig.velocity = new Vector2(rig.velocity.x, 0);
            rig.AddForce(new Vector2(0, doubleJumpForce));
            animController.startDoubleJumpAnimation(dirX, leftFoot.gameObject, rightFoot.gameObject);
        }

        //W to jump if you're grounded or literally are just going off the edge of a platform
        if (Input.GetKeyDown(KeyCode.W) && !animator.GetBool("double jump") && (isGrounded || iK_Foot.slipped))
        {
            rig.velocity = new Vector2(rig.velocity.x, 0);
            rig.gravityScale = maxGravity;
            rig.AddForce(new Vector2(0, jumpForce));
            animator.SetBool("jumped", true);
        }

        if (Input.GetKeyDown(KeyCode.S))
            rig.AddForce(new Vector2(0, -jumpForce));
    }

    private void FixedUpdate()
    {
        setPlayerVelocity();
    }

    private void setPlayerVelocity()
    {
        DebugGUI.debugText4 = $"jumped: {animator.GetBool("jumped")}, dirX: {dirX}, " +
            $"wallToTheRight: {wallToTheRight}, wallToTheLeft: {wallToTheLeft}, speed: {speed}";

        // when player goes idle and hasn't jumped, kill y velocity -> stop bounce glitch
        if (!animator.GetBool("jumped") && rig.velocity.y > 0 && dirX == 0)
        {
            rig.velocity = new Vector2(rig.velocity.x, 0);
            Debug.Log("1");
        }

        // when player is on the ground, their velocity is parallel to the slanted ground
        if (!animator.GetBool("jumped") && isGrounded && isTouchingMap && Mathf.Abs(groundAngle) < maxSlopeAngle)
        {
            //no x velocity when running into a wall -> can't "climb" sloped wall 
            if (dirX == 1 && wallToTheRight)
            {
                rig.velocity = new Vector2(0, 0);
                rig.gravityScale = 0;
                Debug.Log("2");
                return;
            }

            //no x velocity when running into a wall -> can't "climb" sloped wall 
            else if (dirX == -1 && wallToTheLeft)
            {
                rig.velocity = new Vector2(0, 0);
                rig.gravityScale = 0;
                Debug.Log("3");
                return;
            }

            //player velocity is parallel to the slanted ground
            else
            {
                rig.velocity = groundDir * speed * dirX;
                Debug.Log("4");
            }
        }

        //when player is not on the ground, their velocity is just left/right with gravity applied
        else
        {
            //lower x velocity when running into a wall mid-air to avoid severe clipping glitch
            if (dirX == 1 && wallToTheRight)
            {
                rig.velocity = new Vector2(3f, rig.velocity.y);
            }

            //lower x velocity when running into a wall mid-air to avoid severe clipping glitch
            else if (dirX == -1 && wallToTheLeft)
            {
                rig.velocity = new Vector2(-3f, rig.velocity.y);
            }

            //player velocity is just left or right (with gravity pulling the player down)
            else
                rig.velocity = new Vector2(speed * dirX, rig.velocity.y);
        }

        //don't apply gravity when you are both idle and directly touching the ground below you
        if (dirX == 0 && isTouchingMap && isGrounded && Mathf.Abs(groundAngle) < maxSlopeAngle)
            rig.gravityScale = 0;
        else if (!animator.GetBool("jumped") && isGrounded && isTouchingMap)
            rig.gravityScale = gravityCounter;
        else
            rig.gravityScale = 2.5f;
    }
}
