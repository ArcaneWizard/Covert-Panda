using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using static Validation;

// Note to self: This class is doing too much: movement, collider setup + collision detection, ground info, wall detection.
// Move some of the code to helper classes.

///<summary> Provides functionality for implementing movement and access to wall/ground collision info. </summary>
public abstract class CentralController : MonoBehaviour
{
    // Useful information
    public bool isGrounded { get; protected set; }
    public bool isTouchingMap { get; protected set; }
    public bool IsOnTraversableSlope { get; protected set; }
    public bool isOnSuperSteepSlope { get; protected set; }
    public bool recentlyJumpedOffGround { get; private set; }
    public bool recentlyDoubleJumpedOffGround { get; private set; }

    // Current direction of creature's movement (-1 = left, 0 = idle, 1 = right)
    public int DirX { get; protected set; }

    // Important movement constants
    public const float JUMP_FORCE = 1850f;
    public const float DOUBLE_JUMP_FORCE = 2220f;
    public const float JUMP_PAD_FORCE = 3400f;
    public const float DOWNWARDS_THRUST_FORCE = -2600f;
    public const float MIN_ANGLE_OF_TRAVERSABLE_SLOPE = 65f;

    protected readonly float MAX_Y_NORMAL_OF_WALLS = Mathf.Cos(MIN_ANGLE_OF_TRAVERSABLE_SLOPE * Mathf.Deg2Rad);
    private const float BODY_TILT_ANGLE_RATIO = 0.5f;
    private const float BODY_TILT_UPDATE_SPEED = 7f;

    [Header("Limbs and colliders")]
    [SerializeField] private BoxCollider2D oneWayCollider;
    [SerializeField] private CircleCollider2D somersaultCollider;
    [field: SerializeField] protected Transform shootingArm { get; private set; }
    [SerializeField] private CapsuleCollider2D mainCollider;
    private Vector2 initialMainColliderSize;

    [Header("Ground detection")]
    [SerializeField] private Transform frontGroundRaycaster;
    [SerializeField] private Transform backGroundRaycaster;
    [SerializeField] private Transform wallRaycaster;

    protected Rigidbody2D rig;
    protected Transform body;
    protected CentralPhaseTracker phaseTracker;
    protected Health health;
    protected CentralLookAround lookAround;
    protected Animator animator;

    public const float MaxSpeed = 24f;
    public const float MinSpeed = 16f;
    protected float speed;

    private const float SOMERSAULT_COLLIDER_RADIUS = 1.5f;

    // Info about the ground or walls detected:
    protected GameObject platformBelowFrontLeg, platformUnderBackLeg, centerGround;
    protected bool wallBehindYou, wallInFrontOfYou;

    // Info about the slope of the ground 
    protected Vector2 groundSlope;
    protected float groundAngle;
    private float lastGroundAngle;

    /// <summary> Update body tilt based of the slope of the ground below this creature </summary>
    public void UpdateTiltInstantly()
    {
        checkIfGrounded();

        if (!float.IsNaN(groundAngle))
            tiltAngle = groundAngle * BODY_TILT_ANGLE_RATIO;
    }

    /// <summary> Returns a value between -180 and 180 </summary>
    public float GetAngleOfBodyTilt() => MathX.ClampAngleTo180(tiltAngle);
    private float tiltAngle;

    protected virtual void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        body = transform.GetChild(0).transform;
        animator = transform.GetChild(0).transform.GetComponent<Animator>();
        phaseTracker = transform.GetComponent<CentralPhaseTracker>();
        lookAround = transform.GetComponent<CentralLookAround>();
        health = transform.GetComponent<Health>();

        this.Validate(
            new NotNull(rig, nameof(rig)),
            new RequireTag(oneWayCollider, nameof(oneWayCollider), "lightHouse"), 
            new NotNull(body, nameof(body)),
            new NotNull(animator, nameof(animator)),
            new NotNull(phaseTracker, nameof(phaseTracker)),
            new NotNull(lookAround, nameof(lookAround)),
            new NotNull(health, nameof(health))
        );
    }

    protected virtual void Start()
    {
        // configure colliders
        mainCollider.gameObject.layer = Layer.DefaultCollider;
        somersaultCollider.gameObject.layer = Layer.DefaultCollider;
        oneWayCollider.gameObject.layer = Layer.OneSidedPlatformCollider;

        mainCollider.enabled = true;
        oneWayCollider.enabled = true;
        somersaultCollider.enabled = false;

        // configure vars
        initialMainColliderSize = mainCollider.size;
        speed = MaxSpeed;

        // other
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    protected virtual void Update()
    {
        if (health.IsDead)
        {
            mainCollider.enabled = false;
            oneWayCollider.enabled = false;
            somersaultCollider.enabled = false;
            return;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (health.IsDead)
            return;

        updateBodyTilt();
        checkIfGrounded();
        performWallChecks();
        adjustCollidersAndDetectors();

        // body updates
        body.localEulerAngles = new Vector3(0, lookAround.IsFacingRight ? 0 : 180, lookAround.IsFacingRight ? tiltAngle : -tiltAngle);
        body.position = new Vector3(body.position.x, body.position.y, 0);

        // grow double jump collider over time when double jumping
        if (somersaultCollider.radius < SOMERSAULT_COLLIDER_RADIUS)
            somersaultCollider.radius += Time.fixedDeltaTime * 2f;
        else
            somersaultCollider.radius = SOMERSAULT_COLLIDER_RADIUS;
    }

    // Update the creature's tilt angle based of the slope of the ground
    private void updateBodyTilt()
    {
        if (float.IsNaN(groundAngle))
            return;

        tiltAngle = MathX.ClampAngleTo180(tiltAngle);
        float targetTiltAngle = groundAngle * BODY_TILT_ANGLE_RATIO;

        if (isGrounded && (DirX != 0 || (DirX == 0 && groundAngle == lastGroundAngle)))
        {
            if (Mathf.Abs(groundAngle - tiltAngle) > 0.5f)
                tiltAngle += (targetTiltAngle - tiltAngle) * BODY_TILT_UPDATE_SPEED * Time.fixedDeltaTime;
        }

        else if (!isGrounded && Mathf.Abs(tiltAngle) > 0.5f && !phaseTracker.Is(Phase.DoubleJumping))
            tiltAngle -= tiltAngle * 10 * Time.fixedDeltaTime;
    }

    // update whether the creature is grounded, and what the ground's angle and slope is
    private void checkIfGrounded()
    {
        if (health.IsDead)
            return;

        centerRaycasterOnMainCollider(backGroundRaycaster, 0.9f);
        centerRaycasterOnMainCollider(frontGroundRaycaster, -0.9f);

        float raycastLength = isGrounded ? 2.3f : 2.0f;
        RaycastHit2D backLegContactInfo = Physics2D.Raycast(backGroundRaycaster.position, Vector2.down, raycastLength, LayerMasks.Map);
        RaycastHit2D frontLegContactInfo = Physics2D.Raycast(frontGroundRaycaster.position, Vector2.down, raycastLength, LayerMasks.Map);

        platformBelowFrontLeg = (frontLegContactInfo.collider != null)  ? frontLegContactInfo.collider.gameObject : null;
        platformUnderBackLeg = (backLegContactInfo.collider != null) ? backLegContactInfo.collider.gameObject : null;

        // this bool is used later to prevent bug (spasming when standing still on uneven terrain)
        bool checkForAngle = false;
        if ((!isGrounded || DirX != 0) && (platformBelowFrontLeg || platformUnderBackLeg))
            checkForAngle = true;

        bool platformBelowFeet = false;
        if (!phaseTracker.IsDoingSomersault)
            platformBelowFeet = platformBelowFrontLeg || platformUnderBackLeg;

        if (!platformBelowFeet)
        {
            groundSlope = new Vector2(1, 0);
            groundAngle = 0;
        }
        else
        {
            if (platformBelowFrontLeg && !platformUnderBackLeg)
                groundSlope = new Vector2(frontLegContactInfo.normal.y, -frontLegContactInfo.normal.x);
            
            else if (platformUnderBackLeg && !platformBelowFrontLeg)
                groundSlope = new Vector2(backLegContactInfo.normal.y, -backLegContactInfo.normal.x);

            else if (platformBelowFrontLeg && platformUnderBackLeg)
            {
                // if each leg is on a different slope, choose the less steep slope to determine if grounded
                RaycastHit2D lowerOfTwoSlopesBelowLegs = (Mathf.Abs(frontLegContactInfo.normal.y) >= Mathf.Abs(backLegContactInfo.normal.y))
                     ? frontLegContactInfo
                     : backLegContactInfo;

                groundSlope = new Vector2(lowerOfTwoSlopesBelowLegs.normal.y, -lowerOfTwoSlopesBelowLegs.normal.x);
            }

            // update ground angle
            if (groundSlope.x == 0)
                groundAngle = 90;
            else
            {
                float tempGroundAngle = Mathf.Atan(groundSlope.y / groundSlope.x) * 180f / Mathf.PI;
                groundAngle = MathX.ClampAngleTo180(tempGroundAngle);
            }
        }

        // confirm if creature is grounded
        isGrounded = platformBelowFeet;
        IsOnTraversableSlope = Mathf.Abs(groundAngle) <= MIN_ANGLE_OF_TRAVERSABLE_SLOPE && platformBelowFeet;
        isOnSuperSteepSlope = Mathf.Abs(groundAngle) > MIN_ANGLE_OF_TRAVERSABLE_SLOPE && platformBelowFeet;

        // lastGroundAngle will be used to prevent a bug where the player sometimes spasms when standing still on uneven terrain
        if (checkForAngle)
            lastGroundAngle = groundAngle;
    }

    private void performWallChecks()
    {
        if (health.IsDead)
            return;

        centerRaycasterOnMainCollider(wallRaycaster, 0f);

        // left and right wall raycasts start just outside the edges of the main collider
        float raycastSize = body.lossyScale.x * mainCollider.size.x / 2f + 0.1f;
        Vector2 raycastDir = isGrounded ? groundSlope : Vector2.right;

        if (body.localEulerAngles.y == 0)
        {
            RaycastHit2D leftWallHit = Physics2D.Raycast(wallRaycaster.position, -raycastDir, raycastSize, LayerMasks.Map);
            wallBehindYou = (leftWallHit.collider != null && Mathf.Abs(leftWallHit.normal.y) < MAX_Y_NORMAL_OF_WALLS);
            //Debug.DrawRay(wallRaycaster.position, raycastSize * -raycastDir, Color.blue, 2f);

            RaycastHit2D rightWallHit = Physics2D.Raycast(wallRaycaster.position, raycastDir, raycastSize, LayerMasks.Map);
            wallInFrontOfYou = (rightWallHit.collider != null && Mathf.Abs(rightWallHit.normal.y) < MAX_Y_NORMAL_OF_WALLS);
            //Debug.DrawRay(wallRaycaster.position, raycastSize * raycastDir, Color.red, 2f);
        }
        else
        {
            RaycastHit2D leftWallHit = Physics2D.Raycast(wallRaycaster.position, -raycastDir, raycastSize, LayerMasks.Map);
            wallInFrontOfYou = (leftWallHit.collider != null && Mathf.Abs(leftWallHit.normal.y) < MAX_Y_NORMAL_OF_WALLS);
            //Debug.DrawRay(wallRaycaster.position, raycastSize * -raycastDir, Color.red, 2f);

            RaycastHit2D rightWallHit = Physics2D.Raycast(wallRaycaster.position, raycastDir, raycastSize, LayerMasks.Map);
            wallBehindYou = (rightWallHit.collider != null && Mathf.Abs(rightWallHit.normal.y) < MAX_Y_NORMAL_OF_WALLS);
            //Debug.DrawRay(wallRaycaster.position, raycastSize * raycastDir, Color.blue, 2f);
        }
    }

    protected IEnumerator normalJump()
    {
        phaseTracker.Jump();

        rig.velocity = new Vector2(rig.velocity.x, 0);
        rig.gravityScale = Game.GRAVITY;
        rig.AddForce(new Vector2(0, JUMP_FORCE));

        recentlyJumpedOffGround = true;
        yield return new WaitForSeconds(0.2f);
        recentlyJumpedOffGround = false;
    }

    protected IEnumerator doubleJump()
    {
        StartCoroutine(phaseTracker.DoubleJump());

        rig.velocity = new Vector2(rig.velocity.x, 0);
        rig.gravityScale = Game.GRAVITY;
        rig.AddForce(new Vector2(420, DOUBLE_JUMP_FORCE));

        recentlyDoubleJumpedOffGround = true;
        somersaultCollider.radius = 0.05f;
        yield return new WaitForSeconds(0.2f);
        recentlyDoubleJumpedOffGround = false;
    }

    protected IEnumerator jumpPadBoost()
    {
        phaseTracker.Jump();

        rig.velocity = new Vector2(rig.velocity.x, 0);
        rig.gravityScale = Game.GRAVITY;
        rig.AddForce(new Vector2(0, JUMP_PAD_FORCE));

        recentlyJumpedOffGround = true;
        yield return new WaitForSeconds(0.2f);
        recentlyJumpedOffGround = false;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == Layer.DefaultPlatform || col.gameObject.layer == Layer.OneSidedPlatform)
            isTouchingMap = true;
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.layer == Layer.DefaultPlatform || col.gameObject.layer == Layer.OneSidedPlatform)
            isTouchingMap = true;
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.layer == Layer.DefaultPlatform || col.gameObject.layer == Layer.OneSidedPlatform)
            isTouchingMap = false;
    }

    // Adjust creature's colliders and ground detectors as required 
    private void adjustCollidersAndDetectors()
    {
        // if creature is mid-air, main collider becomes thinner 
        float x = phaseTracker.IsMidAir ? initialMainColliderSize.x * 0.62f : initialMainColliderSize.x;
        mainCollider.size = new Vector2(x, mainCollider.size.y);

        // activiate the right colliders for the creature based on the situation
        mainCollider.enabled =  !phaseTracker.IsSomersaulting;
        oneWayCollider.enabled = !phaseTracker.IsSomersaulting && rig.velocity.y < 0.1f;
        somersaultCollider.enabled = phaseTracker.IsSomersaulting;
    }

    // offset = 0 (centered), -1 = (left edge), 1 (right edge)
    private void centerRaycasterOnMainCollider(Transform rayCaster, float offset)
    {
        if (Mathf.Abs(offset) > 1)
            Debug.LogError("Invalid raycaster offset specified");

        rayCaster.transform.localPosition = new Vector3(
            mainCollider.offset.x + mainCollider.size.x / 2f * offset,
            rayCaster.transform.localPosition.y,
            rayCaster.transform.localPosition.z
        );
        rayCaster.transform.localScale = new Vector3(mainCollider.size.x, 1f, 1f);
    }
}