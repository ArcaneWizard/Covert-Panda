
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AI_Controller : CentralController
{

    private void Update()
    {
        //use A and D keys for left or right movement
        movementDirX = 0;

        setAlienVelocity();
        tilt();
    }

    private void LateUpdate()
    {
        lookAndAimInRightDirection();
    }

    private void setAlienVelocity()
    {
        //nullify the slight bounce on a slope glitch when changing slopes
        if (!animator.GetBool("jumped") && rig.velocity.y > 0)
            rig.velocity = new Vector2(0, 0);

        //when alien is on the ground, player velocity is parallel to the slanted ground 
        if (!animator.GetBool("jumped") && grounded && touchingMap)
            rig.velocity = groundDir * speed * movementDirX;

        //when alien is not on the ground, player velocity is just left/right with gravity applied
        else
            rig.velocity = new Vector2(speed * movementDirX, rig.velocity.y);
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

            //calculate the angle btwn mouse cursor and creature's shooting arm
            Vector2 shootDirection = (Input.mousePosition - camera.WorldToScreenPoint(shootingArm.position)).normalized;
            float shootAngle = Mathf.Atan2(shootDirection.y, Mathf.Abs(shootDirection.x)) * 180 / Mathf.PI;

            //apply offset to the shoot Angle when the creature is tilted on a ramp:
            float zAngle = ((180 - Mathf.Abs(180 - transform.eulerAngles.z))); // <- maps angles above 180 to their negative value instead (ex. 330 becomes -30)
            zAngle *= (body.localEulerAngles.y / 90 - 1) * Mathf.Sign(transform.eulerAngles.z - 180);
            shootAngle -= zAngle;

            rotateHeadAndWeapon(shootDirection, shootAngle, weaponAttacks.disableAiming);
        }
    }
}
