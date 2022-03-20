using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralLookAround : MonoBehaviour
{
    //ideal aim coordinates when looking to the side, up or down 
    [HideInInspector]
    private Vector2 pointingRight, pointingUp, pointingDown, shoulderPos;
    private float upVector, downVector, rightVector;
    private float up, right, down;

    public Transform head;
    public Transform shootingArm;
    public Camera camera;

    protected Transform body;
    private Transform aimTarget;

    protected CentralAnimationController animController;
    protected CentralController controller;
    protected CentralShooting shooting;
    protected CentralWeaponSystem weaponSystem;
    protected Health health;

    public virtual void Awake()
    {
        controller = transform.GetComponent<CentralController>();
        animController = transform.GetComponent<CentralAnimationController>();
        shooting = transform.GetComponent<CentralShooting>();
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        health = transform.GetComponent<Health>();
        body = transform.GetChild(0);
    }

    protected void rotateHeadAndWeapon(Vector2 shootDirection, float shootAngle)
    {
        if (shootDirection.y >= 0)
        {
            float slope = (up - right) / 90f;
            float weaponRotation = shootAngle * slope + right;

            float dirSlope = (upVector - rightVector) / 90f;
            float weaponDirMagnitude = shootAngle * dirSlope + rightVector;

            Vector2 targetLocation = weaponDirMagnitude * new Vector2(Mathf.Cos(weaponRotation * Mathf.PI / 180f), Mathf.Sin(weaponRotation * Mathf.PI / 180f)) + shoulderPos;
            if (aimTarget)
                aimTarget.transform.localPosition = targetLocation;

            // head rotation is calculated with a linear equation mapping shooting dir/angle to the corresponding head rotation
            /*float headSlope = (135f - 92.4f) / 90f;
            float headRotation = headSlope * shootAngle + 92.4f;

            // offset the head rotation by the angle of the ground that the creature is standing on (ie. the angle the creature is tilted)
            headRotation += Mathf.Sign(shootDirection.x) * transform.parent.localEulerAngles.z;
            head.eulerAngles = new Vector3(head.eulerAngles.x, head.eulerAngles.y, headRotation);*/
        }

        if (shootDirection.y < 0)
        {
            float slope = (down - right) / -90f;
            float weaponRotation = shootAngle * slope + right;

            float dirSlope = (downVector - rightVector) / -90f;
            float weaponDirMagnitude = shootAngle * dirSlope + rightVector;

            Vector2 targetLocation = weaponDirMagnitude * new Vector2(Mathf.Cos(weaponRotation * Mathf.PI / 180f), Mathf.Sin(weaponRotation * Mathf.PI / 180f)) + shoulderPos;
            if (aimTarget)
                aimTarget.transform.localPosition = targetLocation;

            /*float headSlope = (40f - 92.4f) / -90f;
            float headRotation = headSlope * shootAngle + 92.4f;

            headRotation += Mathf.Sign(shootDirection.x) * transform.parent.localEulerAngles.z;
            head.eulerAngles = new Vector3(head.eulerAngles.x, head.eulerAngles.y, headRotation);*/
        }
    }

    public void calculateShoulderAngles(List<Vector2> aiming)
    {
        //get specific weapon aim coordinates
        pointingRight = aiming[0];
        pointingUp = aiming[1];
        pointingDown = aiming[2];
        shoulderPos = aiming[3];

        //ideal angle from shoulder to specific gun coordinates
        up = Mathf.Atan2(pointingUp.y - shoulderPos.y, pointingUp.x - shoulderPos.x) * 180 / Mathf.PI;
        right = Mathf.Atan2(pointingRight.y - shoulderPos.y, pointingRight.x - shoulderPos.x) * 180 / Mathf.PI;
        down = Mathf.Atan2(pointingDown.y - shoulderPos.y, pointingDown.x - shoulderPos.x) * 180 / Mathf.PI;

        //ideal vector magnitudes from shoulder to specific gun coordinates
        upVector = (pointingUp - shoulderPos).magnitude;
        rightVector = (pointingRight - shoulderPos).magnitude;
        downVector = (pointingDown - shoulderPos).magnitude;
    }

    public void setAimTarget(Transform aimTarget)
    {
        this.aimTarget = aimTarget;
    }
}
