using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_LookAround : CentralLookAround
{
    private Transform player;
    private AI_Shooting ai_shooting;
    private Side side;

    public bool playerIsInSight { get; private set; }
    public Vector3 lookAt { get; private set; }

    public override void Awake()
    {
        base.Awake();

        ai_shooting = transform.GetComponent<AI_Shooting>();
        player = transform.parent.GetComponent<AI>().Player;
        side = transform.parent.GetComponent<Role>().side;

        StartCoroutine(updatePlayerInLineOfSight());
    }

    private void LateUpdate()
    {
        if (health.isDead)
            return;

        lookAt = player.position - shootingArm.position;
        lookAndAimInRightDirection();
        ai_shooting.LateLateUpdate();
    }

    //handles player orientation (left/right), gun rotation, gun position, head rotation
    private void lookAndAimInRightDirection()
    {
        //if player isn't spinning in mid-air with a double jump
        if (!animController.disableLimbsDuringDoubleJump)
        {
            //player faces left or right depending on where it's shooting or moving
            if (playerIsInSight)
                body.localRotation = (lookAt.x > 0) ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
            else if (controller.dirX >= 0)
                body.localRotation = Quaternion.Euler(0, 0, 0);
            else
                body.localRotation = Quaternion.Euler(0, 180, 0);

            //calculate the angle btwn mouse cursor and player's shooting arm
            Vector2 shootDirection = lookAt;
            float shootAngle = Mathf.Atan2(shootDirection.y, Mathf.Abs(shootDirection.x)) * 180 / Mathf.PI;

            //apply offset to the shoot Angle when the player is tilted on a ramp:
            float zAngle = ((180 - Mathf.Abs(180 - transform.eulerAngles.z))); // <- maps angles above 180 to their negative value instead (ex. 330 becomes -30)
            zAngle *= (body.localEulerAngles.y / 90 - 1) * Mathf.Sign(transform.eulerAngles.z - 180);
            shootAngle -= zAngle;

            base.rotateHeadAndWeapon(shootDirection, shootAngle);
        }
    }

    private IEnumerator updatePlayerInLineOfSight()
    {
        yield return new WaitForSeconds(0.3f);

        RaycastHit2D hit = Physics2D.Raycast(shootingArm.position, player.position - shootingArm.position,
            weaponSystem.weaponConfiguration.weaponRange, LayerMasks.mapOrTarget(side));
        playerIsInSight = hit.collider != null && hit.collider.gameObject.layer == Layers.friendlyHitBox;

        StartCoroutine(updatePlayerInLineOfSight());
    }
}
