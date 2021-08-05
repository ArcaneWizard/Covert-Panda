
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Sideview_Controller : CentralController
{
    private int frames;
    private float time;

    private void Update()
    {
        //use A and D keys for left or right movement
        movementDirX = 0;
        if (Input.GetKey(KeyCode.D))
            movementDirX++;
        if (Input.GetKey(KeyCode.A))
            movementDirX--;

        //use W and S keys for jumping up or thrusting downwards + allow double jump
        if (Input.GetKeyDown(KeyCode.W) && animator.GetBool("jumped") && !animator.GetBool("double jump"))
        {
            rig.velocity = new Vector2(rig.velocity.x, 0);
            rig.AddForce(new Vector2(0, jumpForce * 1.1f));
            controller.startDoubleJumpAnimation(movementDirX, leftFoot.gameObject, rightFoot.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.W) && grounded && !animator.GetBool("double jump"))
        {
            rig.velocity = new Vector2(rig.velocity.x, 0);
            rig.AddForce(new Vector2(0, jumpForce));
            animator.SetBool("jumped", true);
        }

        if (Input.GetKeyDown(KeyCode.S))
            rig.AddForce(new Vector2(0, -jumpForce));

        setPlayerVelocity();
        tilt();
    }

    private void LateUpdate()
    {
        lookAndAimInRightDirection();
    }

    private void setPlayerVelocity()
    {
        //nullify the slight bounce on a slope glitch when changing slopes
        if (!animator.GetBool("jumped") && rig.velocity.y > 0)
            rig.velocity = new Vector2(0, 0);

        //when player is on the ground, player velocity is parallel to the slanted ground 
        if (!animator.GetBool("jumped") && grounded && touchingMap)
        {
            //no x velocity when running into a wall to avoid bounce/fall glitch
            if (movementDirX == 1 && wallToTheRight)
                rig.velocity = new Vector2(0, 0);

            //no x velocity when running into a wall to avoid bounce/fall glitch
            else if (movementDirX == -1 && wallToTheLeft)
                rig.velocity = new Vector2(0, 0);

            //player velocity is parallel to the slanted ground
            else
                rig.velocity = groundDir * speed * movementDirX;
        }

        //when player is not on the ground, player velocity is just left/right with gravity applied
        else
        {
            //no x velocity when running into a wall mid-air to avoid clipping glitch
            if (movementDirX == 1 && wallToTheRight)
                rig.velocity = new Vector2(0, rig.velocity.y);

            //no x velocity when running into a wall mid-air to avoid clipping glitch
            else if (movementDirX == -1 && wallToTheLeft)
                rig.velocity = new Vector2(0, rig.velocity.y);

            //player velocity is just left or right (with gravity pulling the player down)
            else
                rig.velocity = new Vector2(speed * movementDirX, rig.velocity.y);
        }
    }

    //handles player orientation (left/right), gun rotation, gun position, head rotation
    private void lookAndAimInRightDirection()
    {
        //if player isn't spinning in mid-air with a double jump
        if (!controller.disableLimbsDuringDoubleJump)
        {
            //player faces left or right depending on mouse cursor
            if (Input.mousePosition.x >= camera.WorldToScreenPoint(shootingArm.parent.position).x)
                body.localRotation = Quaternion.Euler(0, 0, 0);
            else
                body.localRotation = Quaternion.Euler(0, 180, 0);

            //calculate the angle btwn mouse cursor and player's shooting arm
            Vector2 shootDirection = (Input.mousePosition - camera.WorldToScreenPoint(shootingArm.position)).normalized;
            float shootAngle = Mathf.Atan2(shootDirection.y, Mathf.Abs(shootDirection.x)) * 180 / Mathf.PI;

            //apply offset to the shoot Angle when the player is tilted on a ramp:
            float zAngle = ((180 - Mathf.Abs(180 - transform.eulerAngles.z))); // <- maps angles above 180 to their negative value instead (ex. 330 becomes -30)
            zAngle *= (body.localEulerAngles.y / 90 - 1) * Mathf.Sign(transform.eulerAngles.z - 180);
            shootAngle -= zAngle;

            rotateHeadAndWeapon(shootDirection, shootAngle, weaponAttacks.disableAiming);
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

    //Player is on a levitation boost platform and clicks W -> give them a jump boost 
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Levitation" && Input.GetKeyDown(KeyCode.W) && grounded)
            rig.AddForce(Constants.levitationBoost);
    }
}
