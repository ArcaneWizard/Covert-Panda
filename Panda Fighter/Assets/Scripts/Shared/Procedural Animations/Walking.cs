using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walking : ProceduralAnimation
{

    [Range(0, 0.5f)]
    public float earlyRise = 0.3f;
    [Range(0, 10)]
    public float arcTimeInSeconds = 1;
    [Range(0, 5)]
    public float climbRate = 1;
    [Range(0, 1)]
    public float peak_ProgressValue = 0.85f;
    [Range(0, 10)]
    public float legSeperation = 3.4f;
    [Range(0, 2)]
    public float stepTime = 0.3f;
    [Range(-10, 10)]
    public float footAngleOffset = 0f;
    [Range(0, 50)]
    public float squaredStrideDistance = 4f;
    [Range(0, 10)]
    public float trackerLookAhead = 1f;
    [Range(-5, 5)]
    public float centerOfMassXOffset = -0.28f;
    [Range(-5, 5)]
    public float centerOfMassYOffset;
    [Range(-5, 5)]
    public float upperBodyOffsetX;
    [Range(-5, 5)]
    public float upperBodyOffsetY;
    [Range(-5, 5)]
    public float frontThighXOffset;
    [Range(-5, 5)]
    public float backThighXOffset;
    [Range(-5, 5)]
    public float frontThighYOffset;
    [Range(-5, 5)]
    public float backThighYOffset;
    [Range(-10, 10)]
    public float frontFootOffset;
    [Range(-10, 10)]
    public float backFootOffset;
    [Range(0, 2)]
    public float groundYOffset;

    //cached variables
    private Vector2 trackerPosition;
    private Vector2 centerOfMass;

    private float frontGroundSlope, backGroundSlope;
    private float frontGroundAngle, backGroundAngle;

    private Vector2 frontLegStartingPos, backLegStartPos;
    private Vector2 f_targetLegPosition, b_targetLegPosition;

    private Vector2 groundDir;
    private float groundSlope, footAngle;
    private Vector3 frontFootAngle, backFootAngle;
    private float frontVelocity, backVelocity;

    private bool liftFrontFoot;
    private float f_arcProgress, b_arcProgress;
    private Vector2 f_startingLegPosition, b_startingLegPosition;

    private void Awake()
    {
        frontLegStartingPos = new Vector2(0.28f, -0.4f);
        backLegStartPos = new Vector2(-2.18f, -0.4f);
    }

    public override void OnEnter()
    {
        frontLeg.localPosition = frontLegStartingPos;
        backLeg.localPosition = backLegStartPos;

        f_targetLegPosition = frontLeg.position;
        b_targetLegPosition = backLeg.position;

        //temporary null values (not set later on when x coordinate equals Pi)
        frontFootAngle = new Vector3(Mathf.PI, 0, 0);
        backFootAngle = new Vector3(Mathf.PI, 0, 0);

        liftFrontFoot = (facingRight && controller.dirX == 1) || (!facingRight && controller.dirX == -1);
        b_arcProgress = 1f;
        f_arcProgress = 1f;
    }

    public override void Tick()
    {
        centerOfMass = new Vector2(
            entity.transform.position.x + directionFacing * centerOfMassXOffset,
            entity.transform.position.y + centerOfMassYOffset
        );

        float frontThighX = centerOfMass.x - frontThighXOffset * directionFacing;
        float backThighX = centerOfMass.x + backThighXOffset * directionFacing;

        frontThigh.position = new Vector2(frontThighX, centerOfMass.y);
        backThigh.position = new Vector2(backThighX, centerOfMass.y);
        upperBody.position = centerOfMass + new Vector2(upperBodyOffsetX * directionFacing, upperBodyOffsetY);

        //update tracker that tracks the ground a step ahead of the player
        updateTrackerOnGround(centerOfMass + new Vector2(controller.dirX * trackerLookAhead, 0));

        legsFollowArcMotion();
        updateLegTargetPositions();
        updateFeetRotations();

        // print position of both legs and tracker
        Debug.DrawRay(frontLeg.position, Vector2.down * 6, Color.red, 2f);
        Debug.DrawRay(backLeg.position, Vector2.down * 6, Color.cyan, 2f);
        Debug.DrawRay(trackerPosition, Vector2.up * 6, Color.green, 2f);
    }

    private void legsFollowArcMotion()
    {
        // legs carry out arc motion from a specified starting position to a specified target position 
        if (f_arcProgress < 1)
        {
            f_arcProgress += arcTimeInSeconds * Time.deltaTime;

            float x = (f_targetLegPosition.x - f_startingLegPosition.x) * f_arcProgress + f_startingLegPosition.x;
            float y = runningInterpolatedYValue(f_arcProgress, f_startingLegPosition.y, f_targetLegPosition.y);
            frontLeg.position = new Vector2(x, y);
        }
        else
            frontLeg.position = f_targetLegPosition;

        if (b_arcProgress < 1)
        {
            b_arcProgress += arcTimeInSeconds * Time.deltaTime;

            float x = (b_targetLegPosition.x - b_startingLegPosition.x) * b_arcProgress + b_startingLegPosition.x;
            float y = runningInterpolatedYValue(b_arcProgress, b_startingLegPosition.y, b_targetLegPosition.y);
            backLeg.position = new Vector2(x, y);
        }
        else
            backLeg.position = b_targetLegPosition;
    }

    private void updateFeetRotations()
    {
        // update feet rotation
        if (frontFootAngle.x != Mathf.PI)
            frontFoot.transform.eulerAngles = frontFootAngle;
        if (backFootAngle.x != Mathf.PI)
            backFoot.transform.eulerAngles = backFootAngle;
    }

    private void updateLegTargetPositions()
    {
        // bool indicates whether to lift front foot or back foot, depending on which one's further 
        // behind the tracker/body 
        liftFrontFoot = squareDistance(f_targetLegPosition, trackerPosition)
        > squareDistance(b_targetLegPosition, trackerPosition);

        // check if the tracker is far enough from the further ahead leg (ensuring leg seperation)
        bool timeToMoveFrontLeg = squareDistance(b_targetLegPosition, trackerPosition) > legSeperation * legSeperation;
        bool timeToMoveBackLeg = squareDistance(f_targetLegPosition, trackerPosition) > legSeperation * legSeperation;

        // if the tracker has gotten far from the front leg position (greather than the stride distance),
        // and it is the leg that needs to move, and it is a good time to move it, then update its new target  
        // position to the tracker's position and update the foot angle. Also, configure the arc the leg will follow 
        if (squareDistance(f_targetLegPosition, trackerPosition) > squaredStrideDistance && liftFrontFoot && timeToMoveFrontLeg)
        {
            f_targetLegPosition = trackerPosition;
            frontFootAngle = new Vector3(directionAngle, 0f,
            footAngle * directionFacing * Mathf.Sign(groundSlope));

            f_arcProgress = 0;
            f_startingLegPosition = frontLeg.position;
        }

        // same as above but for back leg
        if (squareDistance(b_targetLegPosition, trackerPosition) > squaredStrideDistance && !liftFrontFoot && timeToMoveBackLeg)
        {
            b_targetLegPosition = trackerPosition;
            backFootAngle = new Vector3(directionAngle, 0f,
            footAngle * directionFacing * Mathf.Sign(groundSlope));

            b_arcProgress = 0;
            b_startingLegPosition = backLeg.position;
        }
    }

    private void updateTrackerOnGround(Vector2 tracker)
    {
        RaycastHit2D hit2D = Physics2D.Raycast(tracker + Vector2.up * 2.3f, Vector2.down, 8f, map);

        if (hit2D.collider != null)
        {
            trackerPosition = hit2D.point;

            groundSlope = -hit2D.normal.x / hit2D.normal.y;
            groundDir = new Vector2(1, Mathf.Abs(groundSlope) * 0.7f);
            footAngle = Mathf.Atan2(groundDir.y, groundDir.x) * Mathf.Rad2Deg;

            // ensure the ground is not steeper than 62 degrees (tan 62 = 1.88) 
            if (Mathf.Abs(groundSlope) < 1.88f)
                footAngle += footAngleOffset * directionFacing + directionAngle;

            return;
        }
        else
            trackerPosition = tracker + Vector2.down * 2.5f;

        footAngle = footAngleOffset * directionFacing + directionAngle;
    }

    private float squareDistance(Vector2 a, Vector2 b) => (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);

    private float runningInterpolatedYValue(float progress, float start, float end)
    {
        if (progress >= 0 && progress < earlyRise)
            return start + progress * climbRate;
        else if (progress >= earlyRise && progress < peak_ProgressValue)
            return start + earlyRise * climbRate;
        else
            return start + earlyRise * climbRate + (end - start - earlyRise * climbRate) * (progress - peak_ProgressValue) / (1 - peak_ProgressValue);
    }
}