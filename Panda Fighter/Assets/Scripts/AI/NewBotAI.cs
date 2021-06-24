using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBotAI : MonoBehaviour
{
    private Rigidbody2D rig;
    private Transform alien;
    private Animator animator;

    private float speed = 8f;
    private float jumpForce = 600;

    public Transform leftFoot;
    public Transform rightFoot;
    public Transform centerFoot;
    public GameObject leftFootGround;
    public GameObject rightFootGround;
    public GameObject generalGround;
    public string nextToWall;

    private GameObject groundSurface;
    public List<bool> rightGround = new List<bool>(new bool[10]);
    public List<bool> leftGround = new List<bool>(new bool[10]);
    public Transform leftGroundColliders;
    public Transform rightGroundColliders;

    private Vector2 leftWall;
    private Vector2 rightWall;
    private Vector2 leftHole;
    private Vector2 rightHole;
    private Vector3 groundDetectionOffset;
    private Vector3 pathCollidersOffset;
    public Transform groundDetection;
    public Transform pathColliders;

    public bool grounded;
    [SerializeField]
    private bool touchingMap;
    [SerializeField]
    private float groundAngle;
    private Vector2 groundDir;

    public int movementDirX = 1;
    private float zAngle;
    private float symmetricGroundAngle;

    public GameObject pathCollider;
    public Jump rightJump;
    public Jump leftJump;

    // Start is called before the first frame update
    void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        alien = transform.GetChild(0).transform;
        animator = transform.GetChild(0).transform.GetComponent<Animator>();

        movementDirX = 1;

        groundDetectionOffset = groundDetection.position - transform.position;
        pathCollidersOffset = pathColliders.position - transform.position;
    }

    void Start()
    {
        //Invoke("jump", 1f);
        //Invoke("jump2", 1.3f);

        setConfiguration();
        InvokeRepeating("findWalls", 0.2f, 0.25f);
    }

    private void jump()
    {
        speed = 2.4f;
        rig.velocity = new Vector2(rig.velocity.x, 0);
        rig.AddForce(new Vector2(0, jumpForce));
        animator.SetBool("jumped", true);
    }

    private void jump2()
    {
        movementDirX = 1;
        speed = 3.6f;
        rig.gravityScale = 1.4f;
        rig.velocity = new Vector2(rig.velocity.x, 0);
        rig.AddForce(new Vector2(0, jumpForce * 1.3f));
    }

    // Update is called once per frame
    void Update()
    {
        grounded = isGrounded();

        setAlienVelocity();
        tilt();
        setConfiguration();

        if (pathColliders.transform.GetComponent<Rigidbody2D>().IsSleeping())
            Debug.LogError("Pathcollider rigidbody is sleeping");
    }

    private void printPathColliders()
    {
        Instantiate(pathCollider, alien.transform.position, Quaternion.identity);
    }

    private void setConfiguration()
    {
        groundDetection.position = transform.position + groundDetectionOffset;
        pathColliders.position = transform.position + pathCollidersOffset;

        groundDetection.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, symmetricGroundAngle);
        pathColliders.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
    }

    private void determineClosestHole()
    {
        leftHole = Vector2.zero;
        rightHole = Vector2.zero;

        for (int i = 0; i < leftGround.Count; i++)
        {
            if (leftGroundColliders.transform.GetChild(i).position.x < leftWall.x)
                break;

            else if (leftGround[i] == false)
            {
                leftHole = leftGroundColliders.transform.GetChild(i).position;
                break;
            }
        }

        for (int i = 0; i < rightGround.Count; i++)
        {
            if (rightGroundColliders.transform.GetChild(i).position.x > rightWall.x)
                break;

            if (rightGround[i] == false)
            {
                rightHole = rightGroundColliders.transform.GetChild(i).position;
                break;
            }
        }

        if (leftHole != Vector2.zero)
            Debug.DrawRay(leftHole, Vector2.down * 4f, Color.gray, 2f);
        if (rightHole != Vector2.zero)
            Debug.DrawRay(rightHole, Vector2.down * 4f, Color.gray, 2f);
    }

    private void findWalls()
    {
        if (grounded)
        {
            RaycastHit2D leftWallHit = Physics2D.Raycast(centerFoot.position, -groundDir, 20f, Constants.map);
            leftWall = (leftWallHit.collider != null) ? leftWallHit.point : new Vector2(centerFoot.position.x, centerFoot.position.y) - groundDir * 20;
            Debug.DrawLine(new Vector2(centerFoot.position.x, centerFoot.position.y), leftWall, Color.blue, 2f);

            RaycastHit2D rightWallHit = Physics2D.Raycast(centerFoot.position, groundDir, 20f, Constants.map);
            rightWall = (rightWallHit.collider != null) ? rightWallHit.point : new Vector2(centerFoot.position.x, centerFoot.position.y) + groundDir * 20;
            Debug.DrawLine(new Vector2(centerFoot.position.x, centerFoot.position.y), rightWall, Color.red, 2f);

            determineClosestHole();
        }
    }

    //check if the bot is on the ground + update the groundAngle
    public bool isGrounded()
    {
        if (leftFootGround != null && rightFootGround == null)
            groundSurface = leftFootGround;
        else if (rightFootGround != null && leftFootGround == null)
            groundSurface = rightFootGround;
        else if (rightFootGround != null && leftFootGround != null)
            groundSurface = generalGround;
        else
            groundSurface = null;

        if (groundSurface)
        {
            groundAngle = groundSurface.transform.eulerAngles.z;
            float tangent = Mathf.Tan(groundAngle * Mathf.PI / 180);
            Vector2 dir = new Vector2(1, tangent).normalized;

            groundDir = dir;
        }
        else
        {
            groundAngle = 0;
            groundDir = new Vector2(1, 0);
        }

        return (rightFootGround || leftFootGround) ? true : false;
    }

    private void setAlienVelocity()
    {
        //when player is on the ground, player velocity is parallel to the slanted ground 
        if (!animator.GetBool("jumped") && grounded && touchingMap)
        {
            //no player velocity when running into a wall
            if (movementDirX == 1 && ((alien.localEulerAngles.y == 0 && nextToWall == "Forward") || (alien.localEulerAngles.y == 180 && nextToWall == "Backward")))
                rig.velocity = new Vector2(0, 0);

            //no player velocity when running into a wall 
            else if (movementDirX == -1 && ((alien.localEulerAngles.y == 0 && nextToWall == "Backward") || (alien.localEulerAngles.y == 180 && nextToWall == "Forward")))
                rig.velocity = new Vector2(0, 0);

            //otherwise player velocity is parallel to the slanted ground
            else
                rig.velocity = groundDir * speed * movementDirX;
        }

        //when player is not on the ground, player velocity is just left/right with gravity applied
        else
            rig.velocity = new Vector2(speed * movementDirX, rig.velocity.y);
    }

    //alien's body should tilt slightly on the slanted platform
    private void tilt()
    {
        zAngle = transform.eulerAngles.z;
        symmetricGroundAngle = groundAngle;

        if (zAngle > 180)
            zAngle -= 360;

        if (symmetricGroundAngle > 180)
            symmetricGroundAngle -= 360;

        if (grounded)
        {
            float newGroundAngle = ((groundAngle - 360) / 1.4f);

            if (Mathf.Abs(groundAngle - transform.eulerAngles.z) > 0.5f && groundAngle <= 180)
                transform.eulerAngles = new Vector3(0, 0, zAngle + (groundAngle / 1.4f - zAngle) * 20 * Time.deltaTime);
            else if (Mathf.Abs(groundAngle - transform.eulerAngles.z) > 0.5f)
                transform.eulerAngles = new Vector3(0, 0, zAngle + (newGroundAngle - zAngle) * 20f * Time.deltaTime);
        }
        else if (!grounded && Mathf.Abs(transform.eulerAngles.z) > 0.5f)
            transform.eulerAngles = new Vector3(0, 0, zAngle - zAngle * 10 * Time.deltaTime);

    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 11)
            touchingMap = true;
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.layer == 11)
            touchingMap = true;
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.layer == 11)
            touchingMap = false;
    }
}
