using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sideview_Controller : MonoBehaviour
{
    private Rigidbody2D rig;
    private SpriteRenderer sR;
    private Animator animator;

    private GameObject objectHeld;
    private Camera camera;

    private float speed = 4.0f;
    private float jumpForce = 570;

    private float grenadeThrowForce = 650;
    private float boomerangSpeed = 31;
    private Vector2 objectSpinSpeed = new Vector2(-200, 200);

    private Transform leftFoot;
    private Transform rightFoot;

    private int movementDirX;

    void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        animator = transform.GetChild(0).transform.GetComponent<Animator>();
        sR = transform.GetChild(0).transform.GetComponent<SpriteRenderer>();
        camera = transform.GetChild(1).transform.GetComponent<Camera>();

        leftFoot = transform.GetChild(2);
        rightFoot = transform.GetChild(3);

        objectHeld = null;
    }

    void Update()
    {
        //convert A and D keys to left or right movement
        movementDirX = 0;
        if (Input.GetKey(KeyCode.D))
            movementDirX++;
        if (Input.GetKey(KeyCode.A))
            movementDirX--;

        rig.velocity = new Vector2(speed * movementDirX, rig.velocity.y);

        //convert W and S keys to jumping up or thrusting downwards
        if (Input.GetKeyDown(KeyCode.W) && isGrounded())
            rig.AddForce(new Vector2(0, jumpForce));
        if (Input.GetKeyDown(KeyCode.S))
            rig.AddForce(new Vector2(0, -jumpForce));

        //Right click to throw
        if (Input.GetMouseButtonDown(0) && objectHeld)
        {
            switch (objectHeld.tag)
            {
                case "Grenade":
                    throwGrenade();
                    break;
                case "Boomerang":
                    throwBoomerang();
                    break;
                default:
                    Debug.LogError("You haven't specified how to throw this particular object");
                    break;
            }
        }

        //orient the player to face the same direction they're moving in
        playerOrientation();

        //set idle or move animations
        playerAnimation();
    }

    private Vector2 configureObjectForThrowing()
    {
        //calculate direction to throw object
        Vector2 throwDir = (Input.mousePosition - camera.WorldToScreenPoint(transform.position)).normalized;

        //detach object from Player
        objectHeld.transform.parent = null;
        objectHeld.layer = LayerMask.NameToLayer("Thrown Object");
        objectHeld.transform.GetComponent<Collider2D>().isTrigger = false;

        //launch object and apply gravity
        objectHeld.AddComponent<Rigidbody2D>();
        objectHeld.transform.GetComponent<Rigidbody2D>().gravityScale = 1;

        return throwDir;
    }

    private void throwGrenade()
    {
        //get throw direction from mouse input
        Vector2 throwDir = configureObjectForThrowing();

        //apply a large force to throw the grenade
        objectHeld.transform.GetComponent<Rigidbody2D>().AddForce(grenadeThrowForce * throwDir + new Vector2(0, 20));
        objectHeld.transform.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(objectSpinSpeed.x, objectSpinSpeed.y);

        //Unequip the grenade as its thrown
        objectHeld = null;
    }

    private void throwBoomerang()
    {
        //get throw direction from mouse input
        Vector2 throwDir = configureObjectForThrowing();

        //set the boomerang's velocity really high
        objectHeld.transform.GetComponent<Rigidbody2D>().velocity = throwDir * boomerangSpeed;
        objectHeld.transform.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(objectSpinSpeed.x, objectSpinSpeed.y);

        //Unequip the boomerang as its thrown
        objectHeld = null;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Player collides with object it can pick up
        if (collision.gameObject.layer == LayerMask.NameToLayer("Object") && !objectHeld)
        {
            //object becomes child of Player gameObject and teleports to Player
            objectHeld = collision.gameObject;
            objectHeld.transform.parent = transform;
            objectHeld.transform.position = transform.GetChild(0).transform.position;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Player collides with object it can pick up
        if (collision.gameObject.layer == LayerMask.NameToLayer("Object") && !objectHeld)
        {
            //object becomes child of Player gameObject and teleports to Player
            objectHeld = collision.gameObject;
            objectHeld.transform.parent = transform;
            objectHeld.transform.position = transform.GetChild(0).transform.position;
        }

        //Player is on a levitation boost platform and clicks W -> give them a boost 
        if (collision.gameObject.tag == "Levitation" && Input.GetKeyDown(KeyCode.W) && isGrounded())
            rig.AddForce(Constants.levitationBoost);
    }

    private void playerOrientation()
    {
        if (rig.velocity.x > 0)
            sR.flipX = true;
        else if (rig.velocity.x < 0)
            sR.flipX = false;
    }

    private void playerAnimation()
    {
        if (Mathf.Abs(rig.velocity.x) > 0)
            animator.SetInteger("State", 1);
        else
            animator.SetInteger("State", 0);
    }

    private bool isGrounded()
    {
        RaycastHit2D leftFootGrounded = Physics2D.Raycast(leftFoot.position, Vector2.down, 0.1f, Constants.map);
        RaycastHit2D rightFootGrounded = Physics2D.Raycast(rightFoot.position, Vector2.down, 0.1f, Constants.map);
        return (rightFootGrounded || leftFootGrounded) ? true : false;
    }
}
