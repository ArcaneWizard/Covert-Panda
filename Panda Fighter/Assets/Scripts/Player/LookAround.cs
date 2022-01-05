using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAround : CentralLookAround
{
    private Shooting playerShooting;

    public override void Awake()
    {
        base.Awake();
        playerShooting = transform.GetComponent<Shooting>();
    }

    private void LateUpdate()
    {
        if (health.isDead)
            return;

        lookAndAimInRightDirection();
        playerShooting.LateLateUpdate();
    }

    //handles player orientation (left/right), gun rotation, gun position, head rotation
    private void lookAndAimInRightDirection()
    {
        //if player isn't spinning in mid-air with a double jump
        if (!animController.disableLimbsDuringDoubleJump)
        {
            //player faces left or right depending on mouse cursor
            if (Input.mousePosition.x >= camera.WorldToScreenPoint(transform.position).x)
                body.localRotation = Quaternion.Euler(0, 0, 0);
            else
                body.localRotation = Quaternion.Euler(0, 180, 0);

            //calculate the angle btwn mouse cursor and player's shooting arm
            Vector2 shootDirection = shooting.getAim();
            float shootAngle = Mathf.Atan2(shootDirection.y, Mathf.Abs(shootDirection.x)) * 180 / Mathf.PI;

            //apply offset to the shoot Angle when the player is tilted on a ramp:
            float zAngle = ((180 - Mathf.Abs(180 - transform.eulerAngles.z))); // <- maps angles above 180 to their negative value instead (ex. 330 becomes -30)
            zAngle *= (body.localEulerAngles.y / 90 - 1) * Mathf.Sign(transform.eulerAngles.z - 180);
            shootAngle -= zAngle;

            base.rotateHeadAndWeapon(shootDirection, shootAngle);
        }
    }
}
