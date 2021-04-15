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

    public float speed = 2.0f;
    public float jumpForce = 200;

    public float throwForce = 500;
    public Vector2 objectSpinSpeed = new Vector2(-200, 200);

    private int movementDirX;

    void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        animator = transform.GetChild(0).transform.GetComponent<Animator>();
        sR = transform.GetChild(0).transform.GetComponent<SpriteRenderer>();
        camera = transform.GetChild(1).transform.GetComponent<Camera>();

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
        if (Input.GetKeyDown(KeyCode.W))
            rig.AddForce(new Vector2(0, jumpForce));
        if (Input.GetKeyDown(KeyCode.S))
            rig.AddForce(new Vector2(0, -jumpForce));

        //Right click to throw
        if (Input.GetMouseButtonDown(0) && objectHeld)
            throwObject();

        //orient the player to face the same direction they're moving in
        playerOrientation();

        //set idle or move animations
        playerAnimation();
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

    private void throwObject()
    {
        //calculate direction to throw object
        Vector2 throwDir = (Input.mousePosition - camera.WorldToScreenPoint(transform.position)).normalized;

        //detach object from Player
        objectHeld.transform.parent = null;
        objectHeld.layer = LayerMask.NameToLayer("Thrown Object");

        //launch object and apply gravity
        objectHeld.AddComponent<Rigidbody2D>();
        objectHeld.transform.GetComponent<Rigidbody2D>().gravityScale = 1;
        objectHeld.transform.GetComponent<Rigidbody2D>().AddForce(throwForce * throwDir);
        objectHeld.transform.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(objectSpinSpeed.x, objectSpinSpeed.y);

        //Player no longer is holding that object
        objectHeld = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
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
}
