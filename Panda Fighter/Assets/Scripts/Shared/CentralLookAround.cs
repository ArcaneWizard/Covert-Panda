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
    // the direction the creature is looking at (in world space)
    public Vector2 directionToLook { get; protected set; }

    // whether or not the creature is facing right
    public bool IsFacingRight => body.localEulerAngles.y == 0;

    // calculate and set the direction the creature is looking at
    protected abstract void figureOutDirectionToLookIn();

    // the direction and angle the creature is looking at (relative to their tilt/local x-axis)
    private Vector2 povVector;
    private float povAngle;

    // IK coordinates for main arm when looking up, down or to the side
    private float upVector, downVector, rightVector;
    private float up, right, down;

    // IK coordinates for other arm when looking up, down or to the side (optional)
    private float upVector2, downVector2, rightVector2;
    private float up2, right2, down2;

    [SerializeField] protected Transform head;
    [SerializeField] protected Transform weaponPivot; // used to calculate direction looked at

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

        calculatePOV();
        rotateHead();
        updateArmPosition();
    }

    // calculate and set whether the creature's body faces left or right
    private void updateDirectionBodyFaces()
    {
        if (povVector.x >= 0)
            body.localRotation = Quaternion.Euler(0, 0, 0);
        else
            body.localRotation = Quaternion.Euler(0, 180, 0);
    }

    // Calculate the direction the creature is looking in from its point of view (POV), which should be affected by
    // standing tilted on a slope. The POV vector is obtained by mapping the vector directionToLook in the world's coordinate plane
    // to a coordinate plane whose positive x-axis is defined by the the creature's local positive x-axis. The POV angle is
    // the angle between the creature's local positive x-axis and the direction it is looking
    private void calculatePOV()
    {
        povVector = MathX.RotateVector(directionToLook, -controller.GetAngleOfBodyTilt() * Mathf.Deg2Rad);
        float temp = Mathf.Atan2(povVector.y, Mathf.Abs(povVector.x)) * Mathf.Rad2Deg;
        povAngle = MathX.ClampAngleTo180(temp);
    }
    
    private void rotateHead()
    {
        float headSlope, headRotation;
        float defaultHeadAngle = 90f;

        // rotate head based on angle of sight
        if (povVector.y >= 0)
            headSlope = (153f - defaultHeadAngle) / 90f;
        else
            headSlope = (61f - defaultHeadAngle) / -90f;

        headRotation = headSlope * povAngle + defaultHeadAngle;
        head.localEulerAngles = new Vector3(head.localEulerAngles.x, head.localEulerAngles.y, headRotation);

        // move head forward slightly when looking downwards
        float headPos;
        if (povVector.y >= 0)
            headPos = 0.05f;
        else
            headPos = 0.05f + povAngle * (0.142f - 0.05f) / -90f;

        head.localPosition = new Vector3(headPos, head.localPosition.y, Mathf.Sign(povVector.x) * head.localPosition.z);
    }

    private void updateArmPosition()
    {
        float aimTargetDistanceFromShoulder, aimTargetAngle;

        // rotate main arm to aim in the right direction
        if (mainArmIKTarget != null)
        {
            if (povVector.y >= 0)
            {
                float slope = (up - right) / 90f;
                aimTargetAngle = povAngle * slope + right;

                float dirSlope = (upVector - rightVector) / 90f;
                aimTargetDistanceFromShoulder = povAngle * dirSlope + rightVector;
            }
            else
            {
                float slope = (down - right) / -90f;
                aimTargetAngle = povAngle * slope + right;

                float dirSlope = (downVector - rightVector) / -90f;
                aimTargetDistanceFromShoulder = povAngle * dirSlope + rightVector;
            }

            Vector2 shoulderPos = weaponSystem.CurrentWeaponConfiguration.MainArmIKCoordinates[3];

            mainArmIKTarget.transform.localPosition = aimTargetDistanceFromShoulder
                    * new Vector2(Mathf.Cos(aimTargetAngle * Mathf.PI / 180f), Mathf.Sin(aimTargetAngle * Mathf.PI / 180f)) + shoulderPos;
        }

        // rotate secondary arm (if applicable) to aim in the right direction
        if (otherArmIKTarget != null)
        {
            if (povVector.y >= 0)
            {
                float slope = (up2 - right2) / 90f;
                aimTargetAngle = povAngle * slope + right2;

                float dirSlope = (upVector2 - rightVector2) / 90f;
                aimTargetDistanceFromShoulder = povAngle * dirSlope + rightVector2;
            }
            else
            {
                float slope = (down2 - right2) / -90f;
                aimTargetAngle = povAngle * slope + right2;

                float dirSlope = (downVector2 - rightVector2) / -90f;
                aimTargetDistanceFromShoulder = povAngle * dirSlope + rightVector2;
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