using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Implements the creature's movement (for running, jumping, double
// jumping, and getting a jump boost off jump pads). Stores useful
// movement info such as whether the creature is grounded or touching
// the map, and the direction it's moving

public abstract class CentralController : MonoBehaviour
{
    // Useful information
    public bool isGrounded { get; protected set; }
    public bool isTouchingMap { get; protected set; }
    public bool recentlyJumpedOffGround {get; private set; }
    
    // Current direction of creature's movement (-1 = left, 0 = idle, 1 = right)
    public int DirX { get; protected set; } 

    // Important movement constants:
    public const float JumpForce = 1850f; 
    public const float DoubleJumpForce = 2220f; 
    public const float JumpPadForce = 3400f;
    public const float DownwardsThrustForce = -2600f;
    public const float Gravity = 6.3f;
   
    [Header("Limbs and colliders")]
    [SerializeField] private BoxCollider2D oneWayCollider;
    [SerializeField] private CircleCollider2D somersaultCollider;
    [field: SerializeField] public Transform shootingArm { get; private set; }
    [field: SerializeField] public BoxCollider2D mainCollider { get; private set; }
    private Vector2 initialMainColliderSize;
         
    [Header("Ground detection")]
    [SerializeField] private Transform frontGroundRaycaster;
    [SerializeField] private Transform backGroundRaycaster;

    protected Rigidbody2D rig;
    protected Transform body;
    protected CentralPhaseTracker phaseTracker;
    protected Health health;
    protected CentralLookAround lookAround;
    protected Animator animator;

    public const float MaxSpeed = 22f;
    public const float MinSpeed = 16f;
    protected float speed;

    // Info about the ground or walls detected:
    protected RaycastHit2D leftGroundHit, rightGroundHit, centerGroundHit;
    protected GameObject leftFootGround, rightFootGround, centerGround;
    protected bool wallToTheLeft, wallToTheRight;

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

    // Update body tilt based of the slope of the platform that this creature is standing on
    // To be invoked whenever the creature teleports to a new platform (ex. respawning)
    public void UpdateTiltInstantly()
    {
        updateGroundAngle();
        float newGroundAngle = groundAngle <= 180 ? groundAngle / 3f : ((groundAngle - 360) / 3f);

        if (!float.IsNaN(groundAngle))
            transform.eulerAngles = new Vector3(0, 0, newGroundAngle);
    }

    // Get the angle of the creature's body tilt on a slope. Returns a value between -180 and 180
    public float GetAngleOfBodyTilt() => MathX.ClampAngleTo180(transform.eulerAngles.z);

    protected virtual void Start()
    {
        StartCoroutine(performWallChecks());
        StartCoroutine(repeatedlyCheckIfGrounded());
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

        adjustCollidersAndDetectors();
    }

    protected virtual void LateUpdate() 
    {
        if (health.IsDead)
            return;

        updateTilt();
    }

    protected virtual void FixedUpdate()
    {
        if (health.IsDead)
            return;

        oneWayCollider.enabled = rig.velocity.y < 0.1f;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        body.position = new Vector3(body.position.x, body.position.y, 0);
    }

    // Update the creature's body tilt depending on the ground angle
    private void updateTilt()
    {
        if (float.IsNaN(groundAngle)) 
            return;

        float zAngle = transform.eulerAngles.z;

        if (zAngle > 180)
            zAngle -= 360;

        float newGroundAngle = groundAngle <= 180 ? groundAngle / 1.6f : ((groundAngle - 360) / 1.6f);

        if (isGrounded && (DirX != 0 || (DirX == 0 && groundAngle == lastGroundAngle)))
        {
            if (Mathf.Abs(groundAngle - transform.eulerAngles.z) > 0.5f)
                transform.eulerAngles = new Vector3(0, 0, zAngle + (newGroundAngle - zAngle) * 20 * Time.deltaTime);
        }

        else if (!isGrounded && Mathf.Abs(transform.eulerAngles.z) > 0.5f && !phaseTracker.Is(Phase.DoubleJumping))
            transform.eulerAngles = new Vector3(0, 0, zAngle - zAngle * 10 * Time.deltaTime);
    }
    
    // check if the creature is on the ground + update the groundAngle
    private IEnumerator repeatedlyCheckIfGrounded()
    {
        updateGroundAngle();
        yield return new WaitForSeconds(0.06f);
        StartCoroutine(repeatedlyCheckIfGrounded());
    }

    // update the ground angle
    private void updateGroundAngle() 
    {
        if (health.IsDead)
            return;

        // you need to be be further from the ground to get ungrounded then you need to be close
        // to the ground to be grounded
        float raycastLength = isGrounded ? 2.6f : 2.0f;

        // use raycasts to check for ground below the left foot and right foot (+ draw raycasts for debugging)
        leftGroundHit = Physics2D.Raycast(frontGroundRaycaster.position, Vector2.down, raycastLength, LayerMasks.map);
       // if (leftGroundHit.collider != null)
       //     leftGroundHit = Physics2D.Raycast(frontGroundRaycaster.position + 3 * Vector3.up, Vector2.down, 3f + raycastLength, LayerMasks.map);

        rightGroundHit = Physics2D.Raycast(backGroundRaycaster.position, Vector2.down, raycastLength, LayerMasks.map);
       // if (rightGroundHit.collider != null)
       //     rightGroundHit = Physics2D.Raycast(backGroundRaycaster.position + 3 * Vector3.up, Vector2.down, 3f + raycastLength, LayerMasks.map);

        leftFootGround = (leftGroundHit.collider != null && leftGroundHit.normal.y >= 0.3f) ? leftGroundHit.collider.gameObject : null;
        rightFootGround = (rightGroundHit.collider != null && rightGroundHit.normal.y >= 0.3f) ? rightGroundHit.collider.gameObject : null;

        // if the creature was just grounded after falling OR the player been moving on the ground, enable this bool (used later to prevent bug)
        bool checkForAngle = false;
        if ((!isGrounded || DirX != 0) && (leftFootGround || rightFootGround))
            checkForAngle = true;

        // determine if creature is grounded if either foot raycast hit the ground
        if (!phaseTracker.IsDoingSomersault)
            isGrounded = leftFootGround || rightFootGround;

        // register the angle of the ground
        if (isGrounded)
        {
            // moving right while facing right or moving left while facing left -> use "right foot" gameobject
            if (((DirX == 1 && body.localEulerAngles.y == 0) || (DirX == -1 && body.localEulerAngles.y == 180)) && rightFootGround)
            {
                groundSlope = new Vector2(rightGroundHit.normal.y, -rightGroundHit.normal.x);
                float f = groundSlope.y / groundSlope.x;
                groundAngle = Mathf.Atan(f) * 180f / Mathf.PI;
            }

            // moving left while facing right or moving right facing left -> use "left foot" gameobject
            else if (((DirX == -1 && body.localEulerAngles.y == 0) || (DirX == 1 && body.localEulerAngles.y == 180)) && leftFootGround)
            {
                groundSlope = new Vector2(leftGroundHit.normal.y, -leftGroundHit.normal.x);
                float f = groundSlope.y / groundSlope.x;
                groundAngle = Mathf.Atan(f) * 180f / Mathf.PI;
            }

            // not moving 
            else if (rightFootGround && rightGroundHit.normal.y != 1)
            {
                groundSlope = new Vector2(rightGroundHit.normal.y, -rightGroundHit.normal.x);
                float f = groundSlope.y / groundSlope.x;
                groundAngle = Mathf.Atan(f) * 180f / Mathf.PI;
            }

            // not moving
            else if (leftFootGround)
            {
                groundSlope = new Vector2(leftGroundHit.normal.y, -leftGroundHit.normal.x);
                float f = groundSlope.y / groundSlope.x;
                groundAngle = Mathf.Atan(f) * 180f / Mathf.PI;
            }

            // not moving
            else
            {
                groundSlope = new Vector2(rightGroundHit.normal.y, -rightGroundHit.normal.x);
                float f = groundSlope.y / groundSlope.x;
                groundAngle = Mathf.Atan(f) * 180f / Mathf.PI;
            }
        }

        else
        {
            groundAngle = 0;
            groundSlope = new Vector2(1, 0);
        }

        // lastGroundAngle will be used to prevent a bug where the player sometimes spasms when standing still on uneven terrain
        if (checkForAngle)
            lastGroundAngle = groundAngle;
    }

    private IEnumerator performWallChecks()
    {
        while (health.IsDead)
            yield return null;

        if (body.localEulerAngles.y == 0)
        {
            RaycastHit2D leftWallHit = Physics2D.Raycast(frontGroundRaycaster.position, -groundSlope, 2f, LayerMasks.map);
            wallToTheLeft = (leftWallHit.collider != null && leftWallHit.normal.y < 0.3f) ? true : false;
            //Debug.DrawRay(leftGroundChecker.position, 2 * -groundDir, Color.blue, 2f);

            RaycastHit2D rightWallHit = Physics2D.Raycast(backGroundRaycaster.position, groundSlope, 2f, LayerMasks.map);
            wallToTheRight = (rightWallHit.collider != null && rightWallHit.normal.y < 0.3f) ? true : false;
            //Debug.DrawRay(leftGroundChecker.position, 2 * groundDir, Color.red, 2f);
        }
        else
        {
            RaycastHit2D leftWallHit = Physics2D.Raycast(backGroundRaycaster.position, -groundSlope, 2f, LayerMasks.map);
            wallToTheLeft = (leftWallHit.collider != null && leftWallHit.normal.y < 0.3f) ? true : false;
            //Debug.DrawRay(leftGroundChecker.position, 2 * -groundDir, Color.blue, 2f);

            RaycastHit2D rightWallHit = Physics2D.Raycast(frontGroundRaycaster.position, groundSlope, 2f, LayerMasks.map);
            wallToTheRight = (rightWallHit.collider != null && rightWallHit.normal.y < 0.3f) ? true : false;
            //Debug.DrawRay(leftGroundChecker.position, 2 * groundDir, Color.red, 2f);
        }

        yield return new WaitForSeconds(Time.deltaTime * 2);
        StartCoroutine(performWallChecks());
    }

    protected void normalJump()
    {
        StartCoroutine(updateRecentlyJumpingOffGround());
        phaseTracker.EnterJumpPhase();

        rig.velocity = new Vector2(rig.velocity.x, 0);
        rig.gravityScale = Gravity;
        rig.AddForce(new Vector2(0, JumpForce));
    }

    protected void doubleJump()
    {
        StartCoroutine(phaseTracker.EnterDoubleJumpPhase());

        rig.velocity = new Vector2(rig.velocity.x, 0);
        rig.gravityScale = Gravity;
        rig.AddForce(new Vector2(0, DoubleJumpForce));
    }

    protected void jumpPadBoost()
    {
        StartCoroutine(updateRecentlyJumpingOffGround());
        phaseTracker.EnterJumpPhase();

        rig.velocity = new Vector2(rig.velocity.x, 0);
        rig.gravityScale = Gravity;
        rig.AddForce(new Vector2(0, JumpPadForce));
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == Layer.DefaultGround || col.gameObject.layer == Layer.OneWayGround)
            isTouchingMap = true;
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.layer == Layer.DefaultGround || col.gameObject.layer == Layer.OneWayGround)
            isTouchingMap = true;
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.layer == Layer.DefaultGround || col.gameObject.layer == Layer.OneWayGround)
            isTouchingMap = false;
    }

    private IEnumerator updateRecentlyJumpingOffGround()
    {
        recentlyJumpedOffGround = true;
        yield return new WaitForSeconds(0.2f);
        recentlyJumpedOffGround = false;
    }

    // Adjust creature's colliders and ground detectors as required 
    private void adjustCollidersAndDetectors()
    {
        // if creature is mid-air, bring ground raycasters closer to creature's centerline
        frontGroundRaycaster.localPosition = (!phaseTracker.IsMidAir)
        ? new Vector3(-0.167f, frontGroundRaycaster.localPosition.y, 0)
        : new Vector3(-0.167f, frontGroundRaycaster.localPosition.y, 0);

        backGroundRaycaster.localPosition = (!phaseTracker.IsMidAir)
        ? new Vector3(0.6f, backGroundRaycaster.localPosition.y, 0)
        : new Vector3(0.27f, backGroundRaycaster.localPosition.y, 0);

        // if creature is mid-air, main collider becomes thinner 
        float x = phaseTracker.IsMidAir ? initialMainColliderSize.x * 0.62f : initialMainColliderSize.x;
        mainCollider.size = new Vector2(x, mainCollider.size.y);

        // creature's collider depends on it's current phase
        mainCollider.enabled = !phaseTracker.IsSomersaulting;
        oneWayCollider.enabled = !phaseTracker.IsSomersaulting;
        somersaultCollider.enabled = phaseTracker.IsSomersaulting;
    }
}