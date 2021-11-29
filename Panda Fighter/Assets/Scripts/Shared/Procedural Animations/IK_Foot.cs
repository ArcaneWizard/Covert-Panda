using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_Foot : MonoBehaviour
{
    public bool unstickToGround { get; private set; }
    public bool slipped { get; private set; }
    private float slipTimer;

    private Animator animator;
    private Rigidbody2D rig;
    private CentralController controller;
    private LayerMask map = 1 << 11;

    public Transform frontLeg, backLeg;
    public Transform frontThigh, backThigh;
    public Transform frontFoot, backFoot;
    public Transform upperBody;

    [Header("Offsets")]
    [Range(0, 4)]
    public float hangOffsetX;
    [Range(0, 4)]
    public float hangOffsetY;
    [Range(0, 1)]
    public float chestXOffset;
    [Range(0, 1)]
    public float chestYOffset;
    [Range(0, 4)]
    public float maxThighStretch;
    [Range(-1, 1)]
    public float frontThighXOffset, backThighXOffset;
    [Range(0, 3)]
    public float midAirFrontWeight, midAirBackWeight;

    [Range(-5, 10)]
    public float frontFootOffset;
    [Range(-5, 10)]
    public float backFootOffset;
    [Range(-4, 4)]
    public float bend;
    [Range(0, 100)]
    public float slipForce;

    private float frontGroundSlope, backGroundSlope;
    private float frontGroundAngle, backGroundAngle;
    private float frontSideWeight, backSideWeight;

    //cached variables
    private Vector2 groundDir;
    private float groundSlope, zAngle;
    private float weightedXCoordinate;
    private float frontLegHangOffset, backLegHangOffset;

    private void Awake()
    {
        animator = transform.GetComponent<Animator>();
        controller = transform.parent.GetComponent<CentralController>();
        rig = transform.parent.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        unstickToGround = animator.GetBool("jumped") || !controller.isGrounded || !controller.isTouchingMap;

        // when entity is in mid air, its legs should straighten out 
        if (unstickToGround)
            updateLegsWhileInMidAir();

        // when entity is on the ground
        else
        {
            // update leg bones so that both foot end up on the ground looking natural. Store whether or not
            // either foot was able to ultimately make contact with the ground  
            bool frontLegMadeContact = legSticksToGround(frontLeg, frontFoot, frontThigh);
            bool backLegMadeContact = legSticksToGround(backLeg, backFoot, backThigh);

            DebugGUI.debugText8 = $"front contact: {frontLegMadeContact} and back contact: {backLegMadeContact}";

            // give each leg a weight based off the ground's steepness (higher weight will mean straighter leg)
            if (frontLegMadeContact && backLegMadeContact)
                initializeLegWeights(frontLegMadeContact, backLegMadeContact);

            // if one of the legs doesn't have ground contact, ie. is off a platform's edge, 
            // the entity should slip "off" the edge
            if (!frontLegMadeContact || !backLegMadeContact)
            {
                //push the entity to the left/right so it slides or falls off the ledge
                if ((controller.leftGround == null && facingRight) || (controller.rightGround == null && !facingRight))
                    rig.AddForce(new Vector2(-slipForce, 0));
                else
                    rig.AddForce(new Vector2(slipForce, 0));

                slipped = true;
                slipTimer = 0.3f;

                updateLegsWhileInMidAir();
            }
        }

        // get the weighted average x position btwn the two legs 
        weightedXCoordinate = ((frontLeg.position.x + bend * directionFacing) * backSideWeight
            + (backLeg.position.x + bend * directionFacing) * frontSideWeight)
            / (frontSideWeight + backSideWeight);

        // set the thighs to the weighted average x position. slight offset so they don't overlap
        float frontThighX = weightedXCoordinate - frontThighXOffset * directionFacing;
        float backThighX = weightedXCoordinate + frontThighXOffset * directionFacing;

        // use pythagorous' theoream to calculate the thigh's y position based off how long the leg+thigh combo can be
        float frontThighRaise = Mathf.Sqrt(maxThighStretch * maxThighStretch - Mathf.Pow(frontThighX - frontLeg.position.x, 2));
        frontThigh.position = new Vector3(frontThighX, frontLeg.position.y + frontThighRaise, frontThigh.position.z);
        backThigh.position = new Vector3(backThighX, frontThigh.position.y, backThigh.position.z);

        // shift the lower chest to the position of the thighs with a slight upwards offsett
        upperBody.position = new Vector3(weightedXCoordinate + chestXOffset * directionFacing,
            frontThigh.position.y + chestYOffset, upperBody.position.z);

        // update slip timer
        if (slipTimer > 0f)
            slipTimer -= Time.deltaTime;
        else
            slipped = false;
    }

    // updates position of leg and foot based on the ground elevation
    // returns whether the leg was successfuly aligned to the ground 
    private bool legSticksToGround(Transform leg, Transform foot, Transform thigh)
    {
        RaycastHit2D hit2D = Physics2D.Raycast(new Vector2(leg.position.x, leg.position.y) + Vector2.up * 2f, Vector2.down, 5f, map);

        //if there is ground directly below the leg
        if (hit2D.collider != null)
        {
            //find the ground's slope and angle
            groundSlope = -hit2D.normal.x / hit2D.normal.y;
            groundDir = new Vector2(1, Mathf.Abs(groundSlope) * 0.7f);
            zAngle = Mathf.Atan2(groundDir.y, groundDir.x) * Mathf.Rad2Deg;

            //set these values globally to be accessed by other methods which alter the leg stance
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

            // ensure the ground is not steeper than 62 degrees (tan 62 = 1.88) 
            if (Mathf.Abs(groundSlope) < 1.88f)
            {
                // update the leg to be on the ground
                leg.position = new Vector2(leg.position.x, hit2D.point.y);

                // rotate the feet to be parallel with the ground 
                zAngle += (foot == frontFoot ? frontFootOffset : backFootOffset) + directionAngle;
                foot.transform.eulerAngles = new Vector3(directionAngle, 0f, zAngle * directionFacing
                    * Mathf.Sign(groundSlope));

                //return that the leg was aligned to the ground successfuly
                return true;
            }
            //debugstuff(leg, foot);
        }

        // when no ground was found, or the ground was too steep, rotate the feet horizontally 
        zAngle = (foot == frontFoot ? frontFootOffset : backFootOffset) + directionAngle;
        foot.transform.eulerAngles = new Vector3(directionAngle, 0f, zAngle * directionFacing
            * Mathf.Sign(groundSlope));

        // return that the leg wasn't aligned to the ground 
        return false;

    }

    //initialize leg weights (higher weight = straighter, lower weight = bends more)
    private void initializeLegWeights(bool frontLegMadeContact, bool backLegMadeContact)
    {
        frontSideWeight = 1;
        backSideWeight = 1;

        //set each leg's weight based off the slope's steepness/angle
        if ((frontGroundSlope + backGroundSlope > 0 && facingRight) || (frontGroundSlope + backGroundSlope < 0 && !facingRight))
            backSideWeight += getWeightFromAngle((frontGroundAngle + backGroundAngle) / 2f);

        if ((frontGroundSlope + backGroundSlope > 0 && !facingRight) || (frontGroundSlope + backGroundSlope < 0 && facingRight))
            frontSideWeight += getWeightFromAngle((frontGroundAngle + backGroundAngle) / 2f);

        //if one of the leg's is "off" the platform, set it's weight to the other leg
        if (!frontLegMadeContact)
            backSideWeight = frontSideWeight;
        else if (!backLegMadeContact)
            frontSideWeight = backSideWeight;

        DebugGUI.debugText10 = $"front slope: {frontGroundSlope} and back slope: {backGroundSlope}";
    }

    private void updateLegsWhileInMidAir()
    {
        //reset leg positions
        frontLeg.position = new Vector2(frontLeg.position.x, transform.parent.position.y - hangOffsetY);
        backLeg.position = new Vector2(backLeg.position.x, transform.parent.position.y - hangOffsetY);

        //if not grounded, set weights so that both legs are relatively straight
        frontSideWeight = midAirFrontWeight;
        backSideWeight = midAirBackWeight;
    }

    private float getWeightFromAngle(float angle) => 5f * angle / 90f;
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
