
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Sideview_Controller : CentralController
{
    private int frames;
    private float time;

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
            rig.AddForce(new Vector2(0, doublejumpForce));
            controller.startDoubleJumpAnimation(dirX, leftFoot.gameObject, rightFoot.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.W) && isGrounded && !animator.GetBool("double jump"))
        {
            rig.velocity = new Vector2(rig.velocity.x, 0);
            rig.gravityScale = maxGravity;
            rig.AddForce(new Vector2(0, jumpForce));
            animator.SetBool("jumped", true);
        }

        if (Input.GetKeyDown(KeyCode.S))
            rig.AddForce(new Vector2(0, -jumpForce));

        setPlayerVelocity();
    }

    private void setPlayerVelocity()
    {
        //nullify the slight bounce on a slope glitch when changing slopes
        if (!animator.GetBool("jumped") && rig.velocity.y > 0)
            rig.velocity = new Vector2(0, 0);

        //when player is on the ground, player velocity is parallel to the slanted ground 
        if (!animator.GetBool("jumped") && isGrounded && isTouchingMap)
        {
            //no x velocity when running into a wall to avoid bounce/fall glitch
            if (dirX == 1 && wallToTheRight)
                rig.velocity = new Vector2(0, 0);

            //no x velocity when running into a wall to avoid bounce/fall glitch
            else if (dirX == -1 && wallToTheLeft)
                rig.velocity = new Vector2(0, 0);

            //player velocity is parallel to the slanted ground
            else
                rig.velocity = groundDir * speed * dirX;

            //don't slip on steep slopes
            rig.gravityScale = (dirX == 0) ? 0f : maxGravity;
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

    private void debugFrameRate()
    {
        frames++;
        time += Time.deltaTime;

        if (time >= 1.4f)
        {
            Debug.Log(frames / time);
            time = 0;
            frames = 0;
        }
    }

    /*//Player is on a levitation boost platform and clicks W -> give them a jump boost 
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Levitation" && Input.GetKeyDown(KeyCode.W) && isGrounded)
            rig.AddForce(Constants.levitationBoost);
    }*/
}
