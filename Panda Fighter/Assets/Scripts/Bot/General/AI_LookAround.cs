using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_LookAround : CentralLookAround
{
    private AI_Shooting shootingAI;
    private Side side;

    public GameObject targetInSight { get; private set; }
    public Vector2 lookAt { get; private set; }

    private float randomFloat;

    public override void Awake()
    {
        base.Awake();

        shootingAI = transform.GetComponent<AI_Shooting>();
        side = transform.parent.GetComponent<Role>().side;

        randomFloat = UnityEngine.Random.Range(0f, 10f);
        StartCoroutine(IsTargertInLightOfSight());
    }

    private void LateUpdate()
    {
        if (health.isDead)
            return;

        // look
        if (targetInSight)
            lookAt = new Vector2(targetInSight.transform.position.x - shootingArm.position.x, targetInSight.transform.position.y - shootingArm.position.y);

        else if (controller.isGrounded && controller.isTouchingMap && animator.GetInteger("Phase") != 2) 
        {
            lookAt = new Vector2(
                1 + (Mathf.PerlinNoise(Time.time, randomFloat/2f) * 2f - 1f), 
                Mathf.Tan(transform.localEulerAngles.z * Mathf.PI / 180) + (Mathf.PerlinNoise(Time.time, randomFloat) * 2f -1f)
            );
        }
        
        //handles creature orientation (left/right), gun rotation, gun position, head rotation
        lookAndAimInRightDirection();

        //for weapons that stay activated when holding down right click (ex. beam weapons), run their shooting logic AFTER lookAt is updated above
        shootingAI.LateLateUpdate();
    }

    //handles creature orientation (left/right), gun rotation, gun position, head rotation
    private void lookAndAimInRightDirection()
    {
        //if creature isn't spinning in mid-air with a double jump
        if (!animController.disableLimbsDuringDoubleJump)
        {
            //creature faces left or right depending on where it's shooting or moving
            if (targetInSight)
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

    private IEnumerator IsTargertInLightOfSight()
    {
        yield return new WaitForSeconds(0.3f);
        targetInSight = null;

        // scan for target creatures (on the opposite side) that are within a given radius from this creature. (scan radius is just the equipped weapon's range)
        foreach (Collider2D targetCollider in 
            Physics2D.OverlapCircleAll(shootingArm.position, weaponSystem.CurrentWeaponConfiguration.weaponRange, LayerMasks.target(side))) 
        {
            GameObject target = targetCollider.transform.parent.gameObject;
            Vector3 targetPosition = target.transform.position;
            
            // send a raycast to the target creature (check if there's a barrier in btwn this creature's weapon and that target creature)
            RaycastHit2D hit = Physics2D.Raycast(shootingArm.position, targetPosition - shootingArm.position,
                weaponSystem.CurrentWeaponConfiguration.weaponRange, LayerMasks.mapOrTarget(side));

            // if the target creature is in this creature's line of sight
            if (hit.collider != null && hit.collider.gameObject.layer == Layers.target(side)) 
            {
                // update that this target creature has been sighted, if it is the closest target so far that has been sighted
                if (targetInSight && sqrDistance(targetInSight.transform.position, shootingArm.position) > sqrDistance(targetPosition, shootingArm.position))
                    targetInSight = target;
                else if (!targetInSight)
                    targetInSight = target;
            }
        }

        StartCoroutine(IsTargertInLightOfSight());
    }

    private float sqrDistance(Vector2 a, Vector2 b) {
        return a.x * a.x + b.y * b.y;
    }
}
