using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using UnityEngine;

// When the the creature looks around with their weapon, update the arm limbs/weapon
// to aim in the correct direction and update the head to look in the correct direction.
// Note: the player will look in the direction of their mouse cursor, while the
// AI's sight follows an algorithm

public abstract class CentralLookAround : MonoBehaviour
{
    public Vector2 directionToLook { get; protected set; }
    public bool IsLookingRight => directionToLook.x >= 0;

    // update the direction to look in
    protected abstract void figureOutDirectionToLookIn();

    // update the direction the creature's body faces
    protected abstract void updateDirectionBodyFaces();

    // IK coordinates for main arm when looking up, down or to the side
    private float upVector, downVector, rightVector;
    private float up, right, down;

    // IK coordinates for other arm when looking up, down or to the side (optional)
    private float upVector2, downVector2, rightVector2;
    private float up2, right2, down2;

    [SerializeField] protected Transform head;
    [SerializeField] protected Transform weaponPivot;

    protected Camera camera;
    protected Transform body;
    private Transform mainArmIKTarget;
    private Transform otherArmIKTarget;

    protected CentralPhaseTracker phaseTracker;
    protected CentralController controller;
    protected CentralShooting shooting;
    protected CentralWeaponSystem weaponSystem;
    protected CentralDeathSequence deathSequence;
    protected Health health;
    protected Animator animator;

    // Get the angle the creature's body tilt on sloped ground. Returns a value between -180 and 180
    public float GetAngleOfBodyTilt() => MathX.StandardizeAngle(transform.eulerAngles.z);

    // Get the angle the creature is looking in relative to the positive x-axis in world space.
    // Returns a value between -180 and 180
    public float GetAngleOfSight()
    {
        float angleOfSight = Mathf.Atan2(directionToLook.y, Mathf.Abs(directionToLook.x)) * 180 / Mathf.PI;
        return MathX.StandardizeAngle(angleOfSight);
    }

    // Update the main arm and back arm's Inverse Kinematic (IK) settings so they aim the current
    // equipped weapon correctly. Afterall, different weapon types (long barrel, pistol grip, etc.) have
    // different IK configurations
    public void UpdateArmInverseKinematics()
    {
        WeaponConfiguration configuration = weaponSystem.CurrentWeaponConfiguration;
        mainArmIKTarget = configuration.MainArmIKTracker;
        otherArmIKTarget = configuration.OtherArmIKTracker;    

        if (mainArmIKTarget != null)
        {
            // Store this arm's IK target coordinates (when pointing the gun right, up or down)
            Vector2 pointingRight = configuration.MainArmIKCoordinates[0];
            Vector2 pointingUp = configuration.MainArmIKCoordinates[1];
            Vector2 pointingDown = configuration.MainArmIKCoordinates[2];
            Vector2 shoulderPos = configuration.MainArmIKCoordinates[3];

            // Get the IK target's angle relative to the shoulder (when pointing the gun right, up or down)
            right = Mathf.Atan2(pointingRight.y - shoulderPos.y, pointingRight.x - shoulderPos.x) * 180 / Mathf.PI;
            up = Mathf.Atan2(pointingUp.y - shoulderPos.y, pointingUp.x - shoulderPos.x) * 180 / Mathf.PI;
            down = Mathf.Atan2(pointingDown.y - shoulderPos.y, pointingDown.x - shoulderPos.x) * 180 / Mathf.PI;

            // Get the IK target's distance from the shoulder (when pointing the gun right, up or down)
            rightVector = (pointingRight - shoulderPos).magnitude;
            upVector = (pointingUp - shoulderPos).magnitude;
            downVector = (pointingDown - shoulderPos).magnitude;
        }

        if (otherArmIKTarget != null)
        {
            // Get this arm's IK target coordinates (when pointing the gun right, up or down)
            Vector2 pointingRight = configuration.OtherArmIKCoordinates[0];
            Vector2 pointingUp = configuration.OtherArmIKCoordinates[1];
            Vector2 pointingDown = configuration.OtherArmIKCoordinates[2];
            Vector2 shoulderPos = configuration.OtherArmIKCoordinates[3];

            // Get the IK target's angle relative to the shoulder (when pointing the gun right, up or down)
            right2 = Mathf.Atan2(pointingRight.y - shoulderPos.y, pointingRight.x - shoulderPos.x) * 180 / Mathf.PI;
            up2 = Mathf.Atan2(pointingUp.y - shoulderPos.y, pointingUp.x - shoulderPos.x) * 180 / Mathf.PI;
            down2 = Mathf.Atan2(pointingDown.y - shoulderPos.y, pointingDown.x - shoulderPos.x) * 180 / Mathf.PI;

            // Get the IK target's distance from the shoulder (when pointing the gun right, up or down)
            rightVector2 = (pointingRight - shoulderPos).magnitude;
            upVector2 = (pointingUp - shoulderPos).magnitude;
            downVector2 = (pointingDown - shoulderPos).magnitude;
        }
    }

    protected virtual void Awake()
    {
        controller = transform.GetComponent<CentralController>();
        phaseTracker = transform.GetComponent<CentralPhaseTracker>();
        shooting = transform.GetComponent<CentralShooting>();
        deathSequence = transform.GetComponent<CentralDeathSequence>();
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        health = transform.GetComponent<Health>();
        body = transform.GetChild(0);
        animator = body.GetComponent<Animator>();
        camera = transform.parent.parent.parent.GetComponent<References>().Camera;
    }

    protected virtual void LateUpdate() 
    {
        if (health.IsDead || phaseTracker.IsDoingSomersault)
            return;
        
        figureOutDirectionToLookIn();
        updateDirectionBodyFaces();
        updateHeadMovementAndArmPosition();
    }
    
    // Get the angle the creature is looking in relative to the direction the creature's front chest/body is facing.
    // Accounts for body tilt on sloped ground. Returns a value btwn -180 and 180
    private float getPOVAngle => MathX.StandardizeAngle(GetAngleOfSight() - GetAngleOfBodyTilt());

    private void updateHeadMovementAndArmPosition() 
    {
        // calculate the angle of sight based off vector direction looked in
        float angleOfSight = Mathf.Atan2(directionToLook.y, Mathf.Abs(directionToLook.x)) * 180 / Mathf.PI;

        // adjust the angle of sight correspondingly when the creature's body is tilted on sloped ground/ramps:
        float zAngle = (transform.eulerAngles.z > 180f) ? 360 - transform.eulerAngles.z : transform.eulerAngles.z;
        zAngle *= (body.localEulerAngles.y / 90 - 1) * Mathf.Sign(transform.eulerAngles.z - 180);
        angleOfSight -= zAngle;

        Debug.Log(getPOVAngle);
        //Debug.Log(Mathf.Cos(angleOfSight * Mathf.PI / 180f) +", " + Mathf.Sin(angleOfSight * Mathf.PI / 180f));
        rotateHead(getPOVAngle);
        updateArmPosition(getPOVAngle);
    }
    
    private void rotateHead(float angleOfSight) 
    {
        float headSlope, headRotation;
        float defaultHeadAngle = 90f;

        // rotate head based on angle of sight
        if (directionToLook.y >= 0)
            headSlope = (153f - defaultHeadAngle) / 90f;
        else
            headSlope = (50f - defaultHeadAngle) / -90f;

        headRotation = headSlope * angleOfSight + defaultHeadAngle;
        head.localEulerAngles = new Vector3(head.localEulerAngles.x, head.localEulerAngles.y, headRotation);

        // move head forward slightly when looking downwards
        float headPos;
        if (directionToLook.y >= 0)
            headPos = 0.05f;
        else
            headPos = 0.05f + angleOfSight * (0.142f - 0.05f) / -90f;

        head.localPosition = new Vector3(headPos, head.localPosition.y, Mathf.Sign(directionToLook.x) * head.localPosition.z);
    }

    private void updateArmPosition(float angleOfSight)
    {
        float aimTargetDistanceFromShoulder, aimTargetAngle;

        // rotate main arm to aim in the right direction
        if (mainArmIKTarget != null)
        {
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

            Vector2 shoulderPos = weaponSystem.CurrentWeaponConfiguration.MainArmIKCoordinates[3];

            mainArmIKTarget.transform.localPosition = aimTargetDistanceFromShoulder
                    * new Vector2(Mathf.Cos(aimTargetAngle * Mathf.PI / 180f), Mathf.Sin(aimTargetAngle * Mathf.PI / 180f)) + shoulderPos;
        }

        // rotate secondary arm (if applicable) to aim in the right direction
        if (otherArmIKTarget != null)
        {
            if (directionToLook.y >= 0)
            {
                float slope = (up2 - right2) / 90f;
                aimTargetAngle = angleOfSight * slope + right2;

                float dirSlope = (upVector2 - rightVector2) / 90f;
                aimTargetDistanceFromShoulder = angleOfSight * dirSlope + rightVector2;
            }
            else
            {
                float slope = (down2 - right2) / -90f;
                aimTargetAngle = angleOfSight * slope + right2;

                float dirSlope = (downVector2 - rightVector2) / -90f;
                aimTargetDistanceFromShoulder = angleOfSight * dirSlope + rightVector2;
            }

            Vector2 shoulderPos = weaponSystem.CurrentWeaponConfiguration.OtherArmIKCoordinates[3];
            otherArmIKTarget.transform.localPosition = aimTargetDistanceFromShoulder
                    * new Vector2(Mathf.Cos(aimTargetAngle * Mathf.PI / 180f), Mathf.Sin(aimTargetAngle * Mathf.PI / 180f)) + shoulderPos;
        }

        // ensure all bones on the creature's arms have their x and y rotation always equal to 0
        // prevents IK glitch where these arm rotations somehow can change by accident
        Transform arm = weaponSystem.CurrentWeaponConfiguration.Arms.transform;
        Queue<Transform> armBones = new Queue<Transform>();
        armBones.Enqueue(arm);

        while (armBones.Count > 0)
        {
            foreach (Transform child in armBones.Peek())
                armBones.Enqueue(child);

            Transform temp = armBones.Dequeue();
            temp.localEulerAngles = new Vector3(0f, 0f, temp.localEulerAngles.z);
        }
    }
}