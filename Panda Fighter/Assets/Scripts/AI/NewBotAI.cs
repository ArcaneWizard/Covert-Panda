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
    }


    //check if the bot is on the ground + update the groundAngle
    public bool isGrounded()
    {
        RaycastHit2D leftFootGrounded = Physics2D.Raycast(leftFoot.position, Vector2.down, 0.3f, Constants.map);
        RaycastHit2D rightFootGrounded = Physics2D.Raycast(rightFoot.position, Vector2.down, 0.3f, Constants.map);

        GameObject collider = null;

        if (leftFootGrounded.collider != null && rightFootGrounded.collider == null)
            collider = leftFootGrounded.collider.gameObject;
        else if (rightFootGrounded.collider != null && leftFootGrounded.collider == null)
            collider = rightFootGrounded.collider.gameObject;
        else if (rightFootGrounded.collider != null && leftFootGrounded.collider != null)
        {
            RaycastHit2D centerOfMassGrounded = Physics2D.Raycast(transform.position, Vector2.down, 3.22f, Constants.map);

            if (centerOfMassGrounded.collider != null)
                collider = centerOfMassGrounded.collider.gameObject;
        }

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

        return (rightFootGrounded || leftFootGrounded) ? true : false;
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
