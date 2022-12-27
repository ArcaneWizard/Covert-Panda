using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// When the the creature looks around with their weapon, update the arm limbs/weapon
// to aim in the correct direction and update the head to look in the correct direction 

public abstract class CentralLookAround : MonoBehaviour
{
    public Vector2 directionToLook { get; protected set; }
    [SerializeField] protected Transform weaponPivot;
    
    //ideal aim coordinates when looking to the side, up or down 
    private Vector2 pointingRight, pointingUp, pointingDown, shoulderPos; 
    private float upVector, downVector, rightVector;
    private float up, right, down;

    [SerializeField] protected Transform head;
    protected Camera camera;
    protected Transform body;
    private Transform aimTarget;

    protected CentralPhaseManager phaseManager;
    protected CentralController controller;
    protected CentralShooting shooting;
    protected CentralWeaponSystem weaponSystem;
    protected Health health;
    protected Animator animator;

    protected abstract void figureOutDirectionToLookIn();
    protected abstract void updateDirectionCreatureFaces();
    
    protected virtual void Awake()
    {
        controller = transform.GetComponent<CentralController>();
        phaseManager = transform.GetComponent<CentralPhaseManager>();
        shooting = transform.GetComponent<CentralShooting>();
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        health = transform.GetComponent<Health>();
        body = transform.GetChild(0);
        animator = body.GetComponent<Animator>();
        camera = transform.parent.parent.parent.GetComponent<References>().Camera;
    }

    // updates the weapon's IK aim target 
    public void setAimTarget(Transform aimTarget) => this.aimTarget = aimTarget;

    public void calculateShoulderAngles(List<Vector2> aiming)
    {
        //get where the weapon's aiming target should be positioned when pointing the gun right, up or down
        pointingRight = aiming[0];
        pointingUp = aiming[1];
        pointingDown = aiming[2];
        shoulderPos = aiming[3];

        //get the aim target's angle relative to the shoulder when pointing the gun right, up or down
        right = Mathf.Atan2(pointingRight.y - shoulderPos.y, pointingRight.x - shoulderPos.x) * 180 / Mathf.PI;
        up = Mathf.Atan2(pointingUp.y - shoulderPos.y, pointingUp.x - shoulderPos.x) * 180 / Mathf.PI;
        down = Mathf.Atan2(pointingDown.y - shoulderPos.y, pointingDown.x - shoulderPos.x) * 180 / Mathf.PI;

        //get the aim target's distance from the shoulder when pointing the gun right, up or down
        rightVector = (pointingRight - shoulderPos).magnitude;
        upVector = (pointingUp - shoulderPos).magnitude;
        downVector = (pointingDown - shoulderPos).magnitude;
    }

    protected virtual void LateUpdate() 
    {
        if (health.isDead || phaseManager.DisableLimbsDuringSomersault)
            return;
        
        figureOutDirectionToLookIn();
        updateDirectionCreatureFaces();
        updateGunAndHeadRotation();
    }

    private void updateGunAndHeadRotation() 
    {
        //calculate the angle btwn mouse cursor and player's shooting arm
        float angleOfSight = Mathf.Atan2(directionToLook.y, Mathf.Abs(directionToLook.x)) * 180 / Mathf.PI;

        //adjust the angle of sight correspondingly when the player body is tilted on sloped ground/ramps:
        float zAngle = (transform.eulerAngles.z > 180f) ? 360 - transform.eulerAngles.z : transform.eulerAngles.z;
        zAngle *= (body.localEulerAngles.y / 90 - 1) * Mathf.Sign(transform.eulerAngles.z - 180);
        angleOfSight -= zAngle;

        rotateHead(angleOfSight);
        rotateWeapon(angleOfSight);
    }

    private void rotateHead(float angleOfSight) 
    {
        float headSlope, headRotation;
        if (directionToLook.y >= 0)
            headSlope = (135f - 92.4f) / 90f;
        else
            headSlope = (40f - 92.4f) / -90f;
        
        headRotation = headSlope * angleOfSight + 92.4f;
        headRotation += Mathf.Sign(directionToLook.x) * transform.localEulerAngles.z;
        head.eulerAngles = new Vector3(head.eulerAngles.x, head.eulerAngles.y, headRotation);
    }

    private void rotateWeapon(float angleOfSight)
    {
        float aimTargetDistanceFromShoulder, aimTargetAngle;
        if (directionToLook.y >= 0)
        {
            float slope = (up - right) / 90f;
            aimTargetAngle = angleOfSight * slope + right;

            float dirSlope = (upVector - rightVector) / 90f;
            aimTargetDistanceFromShoulder = angleOfSight * dirSlope + rightVector;
        }
        else 
        {
            float slope = (down - right) / -90f;
            aimTargetAngle = angleOfSight * slope + right;

            float dirSlope = (downVector - rightVector) / -90f;
            aimTargetDistanceFromShoulder = angleOfSight * dirSlope + rightVector;
        }

        if (aimTarget)
            aimTarget.transform.localPosition = aimTargetDistanceFromShoulder 
                * new Vector2(Mathf.Cos(aimTargetAngle * Mathf.PI / 180f), Mathf.Sin(aimTargetAngle * Mathf.PI / 180f)) + shoulderPos;
    }
}
