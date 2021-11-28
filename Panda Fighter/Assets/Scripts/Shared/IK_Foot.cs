using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_Foot : MonoBehaviour
{
    public bool stickToGround { get; private set; }
    private Animator animator;
    private LayerMask map = 1 << 11;

    public Transform frontLeg, backLeg;
    public Transform frontThigh, backThigh;
    public Transform frontFoot, backFoot;
    public Transform upperBody;

    [Header("Thigh and Chest Offsets")]
    [Range(0, 4)]
    public float chestYOffset;
    [Range(0, 4)]
    public float maxThighStretch;
    [Range(-1, 1)]
    public float frontThighXOffset, backThighXOffset;

    [Range(-5, 10)]
    public float frontFootOffset;
    [Range(-5, 10)]
    public float backFootOffset;

    private float frontGroundSlope, backGroundSlope;
    private float frontGroundAngle, backGroundAngle;
    private float frontSideWeight, backSideWeight;

    //cached variables
    private Vector2 groundDir;
    private float groundSlope, zAngle;
    private float weightedXCoordinate;

    private void Awake()
    {
        animator = transform.GetComponent<Animator>();
        DebugGUI.debugText5 = frontFoot.transform.right.x + ", " + frontFoot.transform.right.y
        + ", " + backFoot.transform.right.x + ", " + backFoot.transform.right.y;
    }

    public void StickToGround() => stickToGround = true;
    public void UnstickToGround() => stickToGround = false;

    private void Update()
    {
        legSticksToGround(frontLeg, frontFoot);
        legSticksToGround(backLeg, backFoot);

        frontSideWeight = 1;
        backSideWeight = 1;

        if (frontGroundSlope + backGroundSlope > 0)
            backSideWeight += getWeightFromAngle((frontGroundAngle + backGroundAngle) / 2f);

        if (frontGroundSlope + backGroundSlope < 0)
            frontSideWeight += getWeightFromAngle((frontGroundAngle + backGroundAngle) / 2f);

        DebugGUI.debugText10 = $"front weight: {frontSideWeight} and back weight: {backSideWeight}";

        weightedXCoordinate = (frontLeg.position.x * backSideWeight + backLeg.position.x * frontSideWeight)
            / (frontSideWeight + backSideWeight);

        float frontThighX = weightedXCoordinate - frontThighXOffset * directionFacing;
        float backThighX = weightedXCoordinate + frontThighXOffset * directionFacing;

        float frontThighRaise = Mathf.Sqrt(maxThighStretch * maxThighStretch - Mathf.Pow(frontThighX - frontLeg.position.x, 2));
        frontThigh.position = new Vector3(frontThighX, frontLeg.position.y + frontThighRaise, frontThigh.position.z);
        backThigh.position = new Vector3(backThighX, frontThigh.position.y, backThigh.position.z);

        upperBody.position = new Vector3(weightedXCoordinate, frontThigh.position.y + chestYOffset, upperBody.position.z);
    }

    // updates position of leg and foot based on the ground elevation
    private void legSticksToGround(Transform leg, Transform foot)
    {
        RaycastHit2D hit2D = Physics2D.Raycast(new Vector2(leg.position.x, leg.position.y) + Vector2.up, Vector2.down, 5f, map);

        if (hit2D.collider != null)
        {
            leg.position = new Vector2(leg.position.x, hit2D.point.y);

            groundSlope = -hit2D.normal.x / hit2D.normal.y;
            groundDir = new Vector2(1, Mathf.Abs(groundSlope) * 0.7f);
            zAngle = Mathf.Atan2(groundDir.y, groundDir.x) * Mathf.Rad2Deg;

            if (foot == backFoot)
            {
                backGroundSlope = groundSlope;
                backGroundAngle = zAngle;
            }
            else
            {
                frontGroundSlope = groundSlope;
                frontGroundAngle = zAngle;
            }

            zAngle += (foot == frontFoot ? frontFootOffset : backFootOffset) + directionAngle;
            foot.transform.eulerAngles = new Vector3(directionAngle, 0f, zAngle * directionFacing
                * Mathf.Sign(groundSlope));

            debugstuff(leg, foot);
        }
    }

    private float getWeightFromAngle(float angle) => 9f * angle / 90f;
    private int directionFacing => transform.localEulerAngles.y == 0 ? 1 : -1;
    private int directionAngle => transform.localEulerAngles.y == 0 ? 0 : 180;
    private bool facingRight => transform.localEulerAngles.y == 0;

    private void debugstuff(Transform leg, Transform foot)
    {
        if (leg == frontLeg)
        {
            DebugGUI.debugText3 = frontFoot.eulerAngles.ToString() + ", "
            + new Vector3(
            (directionFacing == 1 ? 0 : 180),
            foot.transform.eulerAngles.y,
            foot.transform.eulerAngles.z
            ).ToString();
        }
        if (leg == backLeg)
        {
            DebugGUI.debugText4 = backFoot.eulerAngles.ToString() + ", "
             + new Vector3(
            (directionFacing == 1 ? 0 : 180),
            foot.transform.eulerAngles.y,
            foot.transform.eulerAngles.z
            ).ToString();
        }

        if (leg == frontLeg)
            DebugGUI.debugText1 = groundSlope.ToString() + ", " + groundDir.ToString();
        if (leg == backLeg)
            DebugGUI.debugText2 = groundSlope.ToString() + ", " + groundDir.ToString();


        /*DebugGUI.debugText7 = zAngle.ToString() + ", " + (directionFacing == 1 ? 0 : 180) +
        ", " + (foot == frontFoot ? frontFootOffset : backFootOffset) + "," + testAngle +
        ", " + foot.transform.eulerAngles.y;*/

    }
}
