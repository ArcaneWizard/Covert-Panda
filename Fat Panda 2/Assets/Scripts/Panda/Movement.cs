using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private int movementDirX;
    private int movementDirY;   

    private float pandaSpeed = 4.5f;

    private float objectLaunchForce = 500;
    private Vector2 objectSpinSpeed = new Vector2(200, 250);

    private Rigidbody2D rig;
    public static GameObject objectHeld;

    private VariableJoystick movementJoystick;
    public VariableJoystick aimingJoystick;

    private Vector2 aim;
    private bool readyToShoot = false;

    void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        objectHeld = null;
    }

    void Update()
    {
        //Get x and y magnitudes based off keys pressed
        movementDirX = 0;
        if (Input.GetKey(KeyCode.D))
            movementDirX++;
        if (Input.GetKey(KeyCode.A))
            movementDirX--;

        movementDirY = 0;
        if (Input.GetKey(KeyCode.W))
            movementDirY++;
        if (Input.GetKey(KeyCode.S))
            movementDirY--;

        //panda follows movement vector at a certain speed
        rig.velocity = pandaSpeed * new Vector2(movementDirX, movementDirY);

        PrepareToThrowAnObject();

        gameObject.layer = (objectHeld) ? LayerMask.NameToLayer("Panda Lifting") : LayerMask.NameToLayer("Panda");
    }

    private void PrepareToThrowAnObject()
    {
        //Keep track of aim when Player is using joystick to aim
        if (objectHeld && aimingJoystick.Direction != default(Vector2))
        {
            aim = aimingJoystick.Direction;
            readyToShoot = true;
        }

        //Throw object when joystick is released
        if (readyToShoot && aimingJoystick.Direction == default(Vector2))
        {
            ThrowObject(aim);
            readyToShoot = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Panda holds an object it picks up
        if (collision.gameObject.layer == LayerMask.NameToLayer("Object") && !objectHeld)
        {
            //object becomes child of Panda gameObject and teleports to Panda
            objectHeld = collision.gameObject;
            objectHeld.transform.parent = transform;
            objectHeld.transform.position = transform.GetChild(0).transform.position;
        }
    }

    private void ThrowObject(Vector2 dir)
    {
        
        //detach object from Panda
        objectHeld.transform.parent = null;
        objectHeld.layer = 10;

        //launch object and apply curve effect through gravity
        objectHeld.AddComponent<Rigidbody2D>();
        objectHeld.transform.GetComponent<Rigidbody2D>().gravityScale = Mathf.Sign(dir.y);
        objectHeld.transform.GetComponent<Rigidbody2D>().AddForce(dir * objectLaunchForce);
        objectHeld.transform.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(objectSpinSpeed.x, objectSpinSpeed.y) * (Random.Range(0,2) * 2 - 1);

        //Object must stop eventually, as if it landed on the ground
        StartCoroutine(objectLandsOnFloorEventually(objectHeld, gameObject.transform.position.y, Mathf.Sign(dir.y)));

        //Panda no longer is holding an object
        objectHeld = null;
    }

    private IEnumerator objectLandsOnFloorEventually(GameObject thrownObject, float groundLevel, float curveDirection)
    {
        //object is in the air for max 22*0.05 = 1.1 seconds
        for (int i = 1; i <= 26; i++)
        {
            yield return new WaitForSeconds(0.05f);

            if (thrownObject.transform.position.y < groundLevel - 1f && curveDirection == 1f)
                break;
            else if (thrownObject.transform.position.y > groundLevel + 1f && curveDirection == -1f)
                break;
        }

        //Stop object's movement 
        Destroy(thrownObject.transform.GetComponent<Rigidbody2D>());

        //Make the object repickable for testing purposes
        //thrownObject.layer = 8;
    }
}
