using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using static Validation;

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

    public float so = 2;

    // Current direction of creature's movement (-1 = left, 0 = idle, 1 = right)
    public int DirX { get; protected set; }

    // Important movement constants
    public const float JUMP_FORCE = 1850f;
    public const float DOUBLE_JUMP_FORCE = 2220f;
    public const float JUMP_PAD_FORCE = 3400f;
    public const float DOWNWARDS_THRUST_FORCE = -2600f;
    public const float GRAVITY = 6.3f;

    // Slopes steeper than this angle can't be walked on
    public const float MIN_ANGLE_OF_STEEP_SLOPE = 65f;

    // If the ground angle is x deg, the creature's body tilts x * (this quantity) degrees
    private const float BODY_TILT_OVER_GROUND_ANGLE = 0.5f;
    public float body_tilt = 0.5f;

    // The creature updates it's body tilt at this speed as the ground slope changes
    private const float BODY_TILT_UPDATE_SPEED = 7f;

    protected readonly float max_y_normal_of_walls = Mathf.Cos(MIN_ANGLE_OF_STEEP_SLOPE * Mathf.Deg2Rad);

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

    private const float somersaultColliderRadius = 1.5f;

    // Info about the ground or walls detected:
    protected GameObject platformBelowFrontLeg, platformUnderBackLeg, centerGround;
    protected bool wallBehindYou, wallInFrontOfYou;

    // Info about the slope of the ground 
    protected Vector2 groundSlope;
    protected float groundAngle;
    private float lastGroundAngle;

    protected virtual void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        body = transform.GetChild(0).transform;
        animator = transform.GetChild(0).transform.GetComponent<Animator>();
        phaseTracker = transform.GetComponent<CentralPhaseTracker>();
        lookAround = transform.GetComponent<CentralLookAround>();
        health = transform.GetComponent<Health>();

        Side side = transform.parent.GetComponent<Role>().side;

        // configuring colliders
        oneWayCollider.gameObject.layer = Layer.OneWayCollider;
        mainCollider.gameObject.layer = (side == Side.Friendly) ? Layer.Friend : Layer.Enemy;
        somersaultCollider.gameObject.layer = (side == Side.Friendly) ? Layer.Friend : Layer.Enemy;

        mainCollider.enabled = true;
        oneWayCollider.enabled = true;
        somersaultCollider.enabled = false;

        initialMainColliderSize = mainCollider.size;
        speed = MaxSpeed;
    }

    /// <summary> Update body tilt based of the slope of the ground below this creature </summary>
    public void UpdateTiltInstantly()
    {
        checkIfGrounded();

        if (!float.IsNaN(groundAngle))
            tiltAngle = groundAngle * body_tilt;
    }

    /// <summary> Returns a value between -180 and 180 </summary>
    public float GetAngleOfBodyTilt() => MathX.ClampAngleTo180(tiltAngle);
    private float tiltAngle;

    protected virtual void Start()
    {
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
        if (somersaultCollider.radius < somersaultColliderRadius)
            somersaultCollider.radius += Time.fixedDeltaTime * so;
        else
            somersaultCollider.radius = somersaultColliderRadius;
    }

    // Update the creature's tilt angle based of the slope of the ground
    private void updateBodyTilt()
    {
        if (float.IsNaN(groundAngle))
            return;

        tiltAngle = MathX.ClampAngleTo180(tiltAngle);
        float targetTiltAngle = groundAngle * body_tilt;

        Debug.Log(targetTiltAngle);

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

        Debug.DrawRay(frontGroundRaycaster.position, Vector2.down * raycastLength, Color.blue, 2f);
        Debug.DrawRay(backGroundRaycaster.position, Vector2.down * raycastLength, Color.blue, 2f);

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
            /*if (((DirX == 1 && lookAround.IsFacingRight) || (DirX == -1 && !lookAround.IsFacingRight)) && platformBelowFrontLeg)
                groundSlope = new Vector2(frontLegContactInfo.normal.y, -frontLegContactInfo.normal.x);

            else if (((DirX == -1 && lookAround.IsFacingRight) || (DirX == 1 && !lookAround.IsFacingRight)) && platformUnderBackLeg)
                groundSlope = new Vector2(backLegContactInfo.normal.y, -backLegContactInfo.normal.x);*/

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
        IsOnTraversableSlope = Mathf.Abs(groundAngle) <= MIN_ANGLE_OF_STEEP_SLOPE && platformBelowFeet;
        isOnSuperSteepSlope = Mathf.Abs(groundAngle) > MIN_ANGLE_OF_STEEP_SLOPE && platformBelowFeet;

        // lastGroundAngle will be used to prevent a bug where the player sometimes spasms when standing still on uneven terrain
        if (checkForAngle)
            lastGroundAngle = groundAngle;
    }

    private void performWallChecks()
    {
        if (health.IsDead)
            return;

        centerRaycasterOnMainCollider(wallRaycaster, 0f);

        // left and right wall raycasts should extend just barely beyond the left/right edges of the main collider
        float raycastSize = body.lossyScale.x * mainCollider.size.x / 2f + 0.1f;
        Vector2 raycastDir = Vector2.right; // isGrounded ? groundSlope : Vector2.right;

        if (body.localEulerAngles.y == 0)
        {
            RaycastHit2D leftWallHit = Physics2D.Raycast(wallRaycaster.position, -raycastDir, raycastSize, LayerMasks.Map);
            wallBehindYou = (leftWallHit.collider != null && Mathf.Abs(leftWallHit.normal.y) < max_y_normal_of_walls);
            Debug.DrawRay(wallRaycaster.position, raycastSize * -raycastDir, Color.blue, 2f);

            RaycastHit2D rightWallHit = Physics2D.Raycast(wallRaycaster.position, raycastDir, raycastSize, LayerMasks.Map);
            wallInFrontOfYou = (rightWallHit.collider != null && Mathf.Abs(rightWallHit.normal.y) < max_y_normal_of_walls);
            Debug.DrawRay(wallRaycaster.position, raycastSize * raycastDir, Color.red, 2f);
        }
        else
        {
            RaycastHit2D leftWallHit = Physics2D.Raycast(wallRaycaster.position, -raycastDir, raycastSize, LayerMasks.Map);
            wallInFrontOfYou = (leftWallHit.collider != null && Mathf.Abs(leftWallHit.normal.y) < max_y_normal_of_walls);
            Debug.DrawRay(wallRaycaster.position, raycastSize * -raycastDir, Color.red, 2f);

            RaycastHit2D rightWallHit = Physics2D.Raycast(wallRaycaster.position, raycastDir, raycastSize, LayerMasks.Map);
            wallBehindYou = (rightWallHit.collider != null && Mathf.Abs(rightWallHit.normal.y) < max_y_normal_of_walls);
            Debug.DrawRay(wallRaycaster.position, raycastSize * raycastDir, Color.blue, 2f);
        }
    }

    protected IEnumerator normalJump()
    {
        phaseTracker.Jump();

        rig.velocity = new Vector2(rig.velocity.x, 0);
        rig.gravityScale = GRAVITY;
        rig.AddForce(new Vector2(0, JUMP_FORCE));

        recentlyJumpedOffGround = true;
        yield return new WaitForSeconds(0.2f);
        recentlyJumpedOffGround = false;
    }

    protected IEnumerator doubleJump()
    {
        StartCoroutine(phaseTracker.DoubleJump());

        rig.velocity = new Vector2(rig.velocity.x, 0);
        rig.gravityScale = GRAVITY;
        rig.AddForce(new Vector2(0, DOUBLE_JUMP_FORCE));

        recentlyDoubleJumpedOffGround = true;
        somersaultCollider.radius = 0.05f;
        yield return new WaitForSeconds(0.2f);
        recentlyDoubleJumpedOffGround = false;
    }

    protected IEnumerator jumpPadBoost()
    {
        phaseTracker.Jump();

        rig.velocity = new Vector2(rig.velocity.x, 0);
        rig.gravityScale = GRAVITY;
        rig.AddForce(new Vector2(0, JUMP_PAD_FORCE));

        recentlyJumpedOffGround = true;
        yield return new WaitForSeconds(0.2f);
        recentlyJumpedOffGround = false;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == Layer.DefaultGround || col.gameObject.layer == Layer.OneWayGround)
        {
            isTouchingMap = true;
        }
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.layer == Layer.DefaultGround || col.gameObject.layer == Layer.OneWayGround)
        {
            isTouchingMap = true;
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.layer == Layer.DefaultGround || col.gameObject.layer == Layer.OneWayGround)
        {
            isTouchingMap = false;
        }
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