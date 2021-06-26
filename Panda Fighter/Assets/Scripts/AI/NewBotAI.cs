using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBotAI : MonoBehaviour
{
    private Rigidbody2D rig;
    private Transform alien;
    private Animator animator;

    public float speed = 8f;
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

    public Vector2 obstacleToTheLeft;
    public Vector2 obstacleToTheRight;
    public bool wallToTheLeft;
    public bool wallToTheRight;
    public Vector2 leftHole;
    public Vector2 rightHole;
    private Vector3 groundDetectionOffset;
    private Vector3 pathCollidersOffset;
    public Transform groundDetection;
    public Transform pathColliders;

    public bool grounded;
    public bool touchingMap;
    [SerializeField]
    private float groundAngle;
    private Vector2 groundDir;

    public int movementDirX = 1;
    private float zAngle;
    private float symmetricGroundAngle;

    public GameObject pathCollider;
    private DecisionMaking decision;

    // Start is called before the first frame update
    void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        alien = transform.GetChild(0).transform;
        animator = transform.GetChild(0).transform.GetComponent<Animator>();
        decision = transform.GetComponent<DecisionMaking>();

        movementDirX = 1;

        groundDetectionOffset = groundDetection.position - transform.position;
        pathCollidersOffset = pathColliders.position - transform.position;

        findWalls();
    }

    void Start()
    {
        StartCoroutine(jumpBruh());
        InvokeRepeating("printPathColliders", 0.4f, 0.2f);

        setConfiguration();
    }

    private IEnumerator jumpBruh()
    {
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(decision.executeJump(new Jump("right mini u-turn", 7.0f, 0.42f, 0f, Vector3.zero)));
    }

    private void printPathColliders()
    {
        Instantiate(pathCollider, alien.transform.position, Quaternion.identity);
    }

    public void jump(float speed)
    {
        this.speed = speed;
        rig.velocity = new Vector2(rig.velocity.x, 0);
        rig.AddForce(new Vector2(0, jumpForce));
        animator.SetBool("jumped", true);
    }

    public void doublejump(float speed, int movementDirX)
    {
        this.speed = speed;
        this.movementDirX = movementDirX;

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
            Debug.LogWarning("Pathcollider rigidbody is sleeping");
    }

    private void setConfiguration()
    {
        Vector3 alienRotationOffset = new Vector3((alien.localEulerAngles.y == 0) ? 0f : 0.54f, 0, 0);

        groundDetection.position = transform.position + groundDetectionOffset + alienRotationOffset;
        pathColliders.position = transform.position + pathCollidersOffset + alienRotationOffset;

        groundDetection.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, symmetricGroundAngle);
        pathColliders.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
    }

    private void determineClosestHole()
    {
        leftHole = Vector2.zero;
        rightHole = Vector2.zero;

        for (int i = 0; i < leftGround.Count; i++)
        {
            if (leftGroundColliders.transform.GetChild(i).position.x < obstacleToTheLeft.x)
                break;

            else if (leftGround[i] == false)
            {
                leftHole = leftGroundColliders.transform.GetChild(i).position;
                break;
            }
        }

        for (int i = 0; i < rightGround.Count; i++)
        {
            if (rightGroundColliders.transform.GetChild(i).position.x > obstacleToTheRight.x)
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

    public void findWalls()
    {
        RaycastHit2D leftWallHit = Physics2D.Raycast(centerFoot.position, -groundDir, 20f, Constants.map);
        wallToTheLeft = (leftWallHit.collider != null && leftWallHit.normal.y < 0.3f) ? true : false;
        obstacleToTheLeft = (leftWallHit.collider != null) ? leftWallHit.point : new Vector2(centerFoot.position.x, centerFoot.position.y) - groundDir * 20;
        Debug.DrawLine(new Vector2(centerFoot.position.x, centerFoot.position.y), obstacleToTheLeft, Color.blue, 2f);

        RaycastHit2D rightWallHit = Physics2D.Raycast(centerFoot.position, groundDir, 20f, Constants.map);
        wallToTheRight = (rightWallHit.collider != null && rightWallHit.normal.y < 0.3f) ? true : false;
        obstacleToTheRight = (rightWallHit.collider != null) ? rightWallHit.point : new Vector2(centerFoot.position.x, centerFoot.position.y) + groundDir * 20;
        Debug.DrawLine(new Vector2(centerFoot.position.x, centerFoot.position.y), obstacleToTheRight, Color.red, 2f);

        if (grounded)
            determineClosestHole();
    }

    //check if the bot is on the ground + update the groundAngle
    public bool isGrounded()
    {
        if (leftFootGround != null && rightFootGround == null)
            groundSurface = leftFootGround;
        else if (rightFootGround != null && leftFootGround == null)
            groundSurface = rightFootGround;
        else if (rightFootGround != null && leftFootGround != null)
        {
            if ((movementDirX >= 0 && alien.localEulerAngles.y == 0) || (movementDirX == -1 && alien.localEulerAngles.y == 180))
                groundSurface = rightFootGround;
            else
                groundSurface = leftFootGround;
        }
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
        //nullify the slight bounce on a slope glitch when changing slopes
        if (!animator.GetBool("jumped") && rig.velocity.y > 0)
            rig.velocity = new Vector2(0, 0);

        //when alien is on the ground, alien velocity is parallel to the slanted ground 
        if (!animator.GetBool("jumped") && grounded && touchingMap)
        {
            //no alien velocity when running into a wall
            if (movementDirX == 1 && ((alien.localEulerAngles.y == 0 && nextToWall == "Forward") || (alien.localEulerAngles.y == 180 && nextToWall == "Backward")))
                rig.velocity = new Vector2(0, 0);

            //no alien velocity when running into a wall 
            else if (movementDirX == -1 && ((alien.localEulerAngles.y == 0 && nextToWall == "Backward") || (alien.localEulerAngles.y == 180 && nextToWall == "Forward")))
                rig.velocity = new Vector2(0, 0);

            //otherwise alien velocity is parallel to the slanted ground
            else
                rig.velocity = groundDir * speed * movementDirX;
        }

        //when alien is not on the ground, alien velocity is just left/right with gravity applied
        else
        {
            //no x velocity when falling and running into a wall mid-air to avoid clipping glitch
            if (movementDirX == 1 && !animator.GetBool("jumped") && ((alien.localEulerAngles.y == 0 && nextToWall == "Forward") || (alien.localEulerAngles.y == 180 && nextToWall == "Backward")))
                rig.velocity = new Vector2(0, rig.velocity.y);

            //no x velocity when falling and running into a wall mid-air to avoid clipping glitch
            else if (movementDirX == -1 && !animator.GetBool("jumped") && ((alien.localEulerAngles.y == 0 && nextToWall == "Backward") || (alien.localEulerAngles.y == 180 && nextToWall == "Forward")))
                rig.velocity = new Vector2(0, rig.velocity.y);

            //kill x velocity when falling next to a wall
            else if (!animator.GetBool("jumped") && ((movementDirX == 1 && obstacleToTheRight.x - transform.position.x < 1.4f && obstacleToTheRight.x - transform.position.x > 0)
            || (movementDirX == -1 && transform.position.x - obstacleToTheLeft.x < 1.4f && transform.position.x - obstacleToTheLeft.x > 0)))
                rig.velocity = new Vector2(0.01f * movementDirX, rig.velocity.y);

            //alien velocity just tells it to move left or right (with gravity pulling the player down)
            else
                rig.velocity = new Vector2(speed * movementDirX, rig.velocity.y);
        }
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
