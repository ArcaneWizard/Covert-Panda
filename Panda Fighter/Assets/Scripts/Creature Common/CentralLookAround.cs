using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// When the the creature looks around with their weapon, update the arm limbs/weapon
/// to aim in the correct direction and update the head to look in the correct direction.
/// </summary>
public abstract class CentralLookAround : MonoBehaviour
{
    /// <summary> the direction the creature is looking at (in world space) </summary>
    public Vector2 DirectionToLook { get; protected set; }

    public bool IsFacingRight { get; protected set; }

    protected abstract void figureOutDirectionToLookIn();

    // the direction and angle the creature is looking at (relative to its local x-axis)
    private Vector2 povVector;
    private float povAngle;

    // IK coordinates for main arm when looking up, down or to the side
    private float upVector, downVector, rightVector;
    private float up, right, down;

    // IK coordinates for back arm when looking up, down or to the side (optional)
    private float upVector2, downVector2, rightVector2;
    private float up2, right2, down2;

    [SerializeField] protected Transform head;
    [SerializeField] protected Transform weaponPivot; // used to calculate direction looked at

    protected new Camera camera;
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

    /// <summary> Update the creature's arms' Inverse Kinematic settings (IKS) so it aims its 
    /// current equipped weapon correctly. Different weapon types (long barrel, pistol grip, etc.) 
    /// require different IKS </summary> 
    public void SetupIKForCurrentWeapon()
    {
        var configuration = weaponSystem.CurrentWeaponConfiguration;
        mainArmIKTarget = configuration.MainArmIKTracker;
        otherArmIKTarget = configuration.OtherArmIKTracker;

        if (mainArmIKTarget != null) {
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

        if (otherArmIKTarget != null) {
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

    // wtf, why so many dependencies lol
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
        camera = References.Instance.Camera;
    }

    protected virtual void LateUpdate()
    {
        if (health.IsDead || phaseTracker.IsDoingSomersault)
            return;

        figureOutDirectionToLookIn();
        IsFacingRight = povVector.x >= 0;

        calculatePOV();
        updateHead();
        updateArmsHoldingWeapon();
    }

    private void calculatePOV()
    {
        // The creature's POV is affected by standing tilted on a slope.
        // The POV vector is obtained by mapping the vector directionToLook in the world's coordinate plane
        // to a coordinate plane whose positive x-axis is defined by the the creature's local positive x-axis.
        // The POV angle is he angle between the creature's local positive x-axis and the direction it is looking

        povVector = MathX.RotateVector(DirectionToLook, -controller.GetAngleOfBodyTilt() * Mathf.Deg2Rad);
        float temp = Mathf.Atan2(povVector.y, Mathf.Abs(povVector.x)) * Mathf.Rad2Deg;
        povAngle = MathX.ClampAngleTo180(temp);
    }

    private void updateHead()
    {
        float headSlope, headRotation;
        float defaultHeadAngle = 90f;

        // rotate head based on POV angle
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

    private void updateArmsHoldingWeapon()
    {
        float aimTargetDistanceFromShoulder, aimTargetAngle;

        // rotate main arm based on POV angle
        if (mainArmIKTarget != null) {
            if (povVector.y >= 0) {
                float slope = (up - right) / 90f;
                aimTargetAngle = povAngle * slope + right;

                float dirSlope = (upVector - rightVector) / 90f;
                aimTargetDistanceFromShoulder = povAngle * dirSlope + rightVector;
            } else {
                float slope = (down - right) / -90f;
                aimTargetAngle = povAngle * slope + right;

                float dirSlope = (downVector - rightVector) / -90f;
                aimTargetDistanceFromShoulder = povAngle * dirSlope + rightVector;
            }

            Vector2 shoulderPos = weaponSystem.CurrentWeaponConfiguration.MainArmIKCoordinates[3];

            mainArmIKTarget.transform.localPosition = aimTargetDistanceFromShoulder
                    * new Vector2(Mathf.Cos(aimTargetAngle * Mathf.PI / 180f), Mathf.Sin(aimTargetAngle * Mathf.PI / 180f)) + shoulderPos;
        }

        // rotate secondary arm (if applicable) based on POV angle
        if (otherArmIKTarget != null) {
            if (povVector.y >= 0) {
                float slope = (up2 - right2) / 90f;
                aimTargetAngle = povAngle * slope + right2;

                float dirSlope = (upVector2 - rightVector2) / 90f;
                aimTargetDistanceFromShoulder = povAngle * dirSlope + rightVector2;
            } else {
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

        while (armBones.Count > 0) {
            foreach (Transform child in armBones.Peek())
                armBones.Enqueue(child);

            Transform temp = armBones.Dequeue();
            temp.localEulerAngles = new Vector3(0f, 0f, temp.localEulerAngles.z);
        }
    }
}
