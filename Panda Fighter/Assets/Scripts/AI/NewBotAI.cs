using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBotAI : MonoBehaviour
{
    private Rigidbody2D rig;
    private Transform alien;
    private Animator animator;

    private float speed = 8.0f;
    private float jumpForce = 600;

    public Transform leftFoot;
    public Transform rightFoot;
    public GameObject leftFootGround;
    public GameObject rightFootGround;
    public GameObject generalGround;
    private string nextToWall;

    public bool grounded;
    [SerializeField]
    private bool touchingMap;
    [SerializeField]
    private float groundAngle;
    private Vector2 groundDir;

    public int movementDirX;

    // Start is called before the first frame update
    void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        alien = transform.GetChild(0).transform;
        animator = transform.GetChild(0).transform.GetComponent<Animator>();

        movementDirX = 1;
        rig.velocity = new Vector2(1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        grounded = isGrounded();

        setAlienVelocity();
        tilt();
    }

    //check if the bot is on the ground + update the groundAngle
    public bool isGrounded()
    {

        GameObject collider = null;

        if (leftFootGround != null && rightFootGround == null)
            collider = leftFootGround;
        else if (rightFootGround != null && leftFootGround == null)
            collider = rightFootGround;
        else if (rightFootGround != null && leftFootGround != null)
            collider = generalGround;

        if (collider)
        {
            groundAngle = collider.transform.eulerAngles.z;
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
        float zAngle = transform.eulerAngles.z;

        if (zAngle > 180)
            zAngle = zAngle - 360;

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
