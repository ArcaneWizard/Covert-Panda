using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_LookAround : CentralLookAround
{
    private Transform player;
    private AI_Shooting shootingAI;
    private Side side;

    public bool enemyInSight { get; private set; }
    public Vector2 lookAt { get; private set; }
    private float randomFloat;

    public override void Awake()
    {
        base.Awake();

        shootingAI = transform.GetComponent<AI_Shooting>();
        player = transform.parent.GetComponent<AI>().Player;
        side = transform.parent.GetComponent<Role>().side;

        randomFloat = UnityEngine.Random.Range(0f, 10f);
        StartCoroutine(IsEnemyInLightOfSight());
    }

    private void LateUpdate()
    {
        if (health.isDead)
            return;

        if (enemyInSight)
            lookAt = new Vector2(player.position.x - shootingArm.position.x, player.position.y - shootingArm.position.y);
        else if (controller.isGrounded && controller.isTouchingMap && animator.GetInteger("Phase") != 2) 
        {
            lookAt = new Vector2(
                1 + (Mathf.PerlinNoise(Time.time, randomFloat/2f) * 2f - 1f), 
                Mathf.Tan(transform.localEulerAngles.z * Mathf.PI / 180) + (Mathf.PerlinNoise(Time.time, randomFloat) * 2f -1f)
            );
        }
        
        lookAndAimInRightDirection();
        shootingAI.LateLateUpdate();
    }

    //handles creature orientation (left/right), gun rotation, gun position, head rotation
    private void lookAndAimInRightDirection()
    {
        //if creature isn't spinning in mid-air with a double jump
        if (!animController.disableLimbsDuringDoubleJump)
        {
            //creature faces left or right depending on where it's shooting or moving
            if (enemyInSight)
                body.localRotation = (lookAt.x > 0) ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
            else if (controller.dirX >= 0)
                body.localRotation = Quaternion.Euler(0, 0, 0);
            else
                body.localRotation = Quaternion.Euler(0, 180, 0);

            //calculate the angle btwn mouse cursor and creature's shooting arm
            Vector2 shootDirection = lookAt;
            float shootAngle = Mathf.Atan2(shootDirection.y, Mathf.Abs(shootDirection.x)) * 180 / Mathf.PI;

            //apply offset to the shoot angle when the creature is tilted on a ramp:
            float zAngle = ((180 - Mathf.Abs(180 - transform.eulerAngles.z))); // <- maps angles above 180 to their negative value instead (ex. 330 becomes -30)
            zAngle *= (body.localEulerAngles.y / 90 - 1) * Mathf.Sign(transform.eulerAngles.z - 180);
            shootAngle -= zAngle;

            base.rotateHeadAndWeapon(shootDirection, shootAngle);
        }
    }

    private IEnumerator IsEnemyInLightOfSight()
    {
        yield return new WaitForSeconds(0.3f);

        RaycastHit2D hit = Physics2D.Raycast(shootingArm.position, player.position - shootingArm.position,
            weaponSystem.weaponConfiguration.weaponRange, LayerMasks.mapOrTarget(side));
        enemyInSight = hit.collider != null && hit.collider.gameObject.layer == Layers.friendlyHitBox;

        StartCoroutine(IsEnemyInLightOfSight());
    }
}
