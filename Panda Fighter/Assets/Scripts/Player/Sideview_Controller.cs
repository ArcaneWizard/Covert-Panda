using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sideview_Controller : MonoBehaviour
{
    private Rigidbody2D rig;
    private Transform player;
    private Animator animator;
    public Transform shootingArm;
    public Transform head;

    public Transform bulletSpawnPoint;
    public Transform gun;
    public BoxCollider2D footCollider;

    private WeaponSystem weaponSystem;
    private GameObject weapon;
    private float timeLeftBtwnShots;

    private Camera camera;

    public float speed = 4.0f;
    public float jumpForce = 570;

    public float grenadeThrowForce = 650;
    public float grenadeYForce = -20;
    public float boomerangSpeed = 31;
    public float plasmaBulletSpeed = 30;
    public float plasmaFireRate = 0.16f;
    public Vector2 objectSpinSpeed = new Vector2(-200, 200);

    public Transform leftFoot;
    public Transform rightFoot;
    private bool grounded;

    private int movementDirX;

    private float timer;

    void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        player = transform.GetChild(0).transform;
        camera = transform.GetChild(1).transform.GetComponent<Camera>();
        animator = transform.GetChild(0).transform.GetComponent<Animator>();
    }

    void Start()
    {
        weaponSystem = transform.GetComponent<WeaponSystem>();
    }

    void Update()
    {
        //use A and D keys for left or right movement
        movementDirX = 0;
        if (Input.GetKey(KeyCode.D))
            movementDirX++;
        if (Input.GetKey(KeyCode.A))
            movementDirX--;

        rig.velocity = new Vector2(speed * movementDirX, rig.velocity.y);

        //store as variable so it can be reused multiple times without redoing the method's raycast calculations
        grounded = isGrounded();

        //use jump animation/movement if player isn't grounded
        if (!grounded && animator.GetInteger("Phase") != 2)
            setAnimation("jumping");

        //use W and S keys for jumping up or thrusting downwards
        if (Input.GetKeyDown(KeyCode.W) && grounded)
        {
            rig.AddForce(new Vector2(0, jumpForce));
            animator.SetBool("jumped", true);
        }
        if (Input.GetKeyDown(KeyCode.S))
            rig.AddForce(new Vector2(0, -jumpForce));

        //Weapons where you right click to use/shoot the weapon once
        if (Input.GetMouseButtonDown(0) && weaponSystem.weaponSelected != null)
        {
            if (weaponSystem.getAmmo() > 0 && weaponSystem.getWeapon().tag == "singleFire")
            {
                weapon = weaponSystem.getWeapon();
                weaponSystem.usedOneAmmo();

                switch (weaponSystem.weaponSelected)
                {
                    case "Grenade":
                        throwGrenade();
                        break;
                    case "Boomerang":
                        throwBoomerang();
                        break;
                    default:
                        Debug.LogError("You haven't specified how to shoot this particular object");
                        break;
                }
            }
        }

        //Weapons where you hold the right mouse button down to continously shoot
        if (Input.GetMouseButton(0) && weaponSystem.weaponSelected != null)
        {
            if (weaponSystem.getAmmo() > 0 && timeLeftBtwnShots <= 0 && weaponSystem.getWeapon().tag == "spamFire")
            {
                weapon = weaponSystem.getWeapon();
                weaponSystem.usedOneAmmo();
                timeLeftBtwnShots = plasmaFireRate;

                switch (weaponSystem.weaponSelected)
                {
                    case "Pistol":
                        shootPlasmaBullet();
                        break;
                    default:
                        Debug.LogError("You haven't specified how to shoot this particular object");
                        break;
                }
            }
        }

        if (timeLeftBtwnShots > 0)
            timeLeftBtwnShots -= Time.deltaTime;

        playerLimbsOrientation();
        playerAnimation();

        //disable main foot's collider when jumping
        footCollider.enabled = animator.GetInteger("Phase") != 2;

        //tuck right foot in when jumping
        rightFoot.transform.localPosition = animator.GetInteger("Phase") != 2 ?
        new Vector3(0.719f, rightFoot.transform.localPosition.y, 0) : new Vector3(0.404f, rightFoot.transform.localPosition.y, 0);
    }

    private Vector2 aimDirection()
    {
        //calculate direction to throw object
        Vector2 dir = (Input.mousePosition - camera.WorldToScreenPoint(shootingArm.position)).normalized;

        weapon.transform.position = bulletSpawnPoint.position;
        weapon.layer = LayerMask.NameToLayer("Thrown Object");
        weapon.SetActive(true);

        weapon.transform.GetComponent<Collider2D>().isTrigger = false;
        weapon.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

        return dir;
    }

    private void throwGrenade()
    {
        //get throw direction from mouse input
        Vector2 dir = aimDirection();
        Rigidbody2D objectRig = weapon.transform.GetComponent<Rigidbody2D>();

        //apply a large force to throw the grenade
        Vector2 unadjustedForce = grenadeThrowForce * dir * new Vector2(1.2f, 1) + new Vector2(0, grenadeYForce);
        objectRig.velocity = new Vector2(0, 0);
        objectRig.AddForce(unadjustedForce * objectRig.mass);
    }

    private void shootPlasmaBullet()
    {
        //get throw direction from mouse input
        Vector2 dir = aimDirection();
        Rigidbody2D objectRig = weapon.transform.GetComponent<Rigidbody2D>();

        //spawn and orient the bullet correctly
        weapon.transform.right = dir;
        objectRig.velocity = dir * plasmaBulletSpeed;
    }

    private void throwBoomerang()
    {
        //get throw direction from mouse input
        Vector2 throwDir = aimDirection();
        Rigidbody2D objectRig = weapon.transform.GetComponent<Rigidbody2D>();

        //set the boomerang's velocity really high
        objectRig.velocity = throwDir * boomerangSpeed;
        objectRig.angularVelocity = Random.Range(objectSpinSpeed.x, objectSpinSpeed.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Player collides with weapon, so equip it
        if (collision.gameObject.layer == LayerMask.NameToLayer("Object"))
        {
            weaponSystem.EquipNewWeapon(collision.gameObject.tag);
            collision.gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Player is on a levitation boost platform and clicks W -> give them a boost 
        if (collision.gameObject.tag == "Levitation" && Input.GetKeyDown(KeyCode.W) && isGrounded())
            rig.AddForce(Constants.levitationBoost);
    }

    private void playerLimbsOrientation()
    {
        //player faces left or right depending on mouse cursor
        if (Input.mousePosition.x >= camera.WorldToScreenPoint(shootingArm.parent.position).x)
            player.localRotation = Quaternion.Euler(0, 0, 0);
        else
            player.localRotation = Quaternion.Euler(0, 180, 0);

        //player's shooting arm (w/ gun) rotates towards the mouse cursor
        Vector2 shootDirection = (Input.mousePosition - camera.WorldToScreenPoint(shootingArm.position)).normalized;
        Vector2 offset = Quaternion.Euler(0, 0, -40f) * shootDirection;
        Vector2 aimDirection = shootDirection + offset;

        Vector2 pointingRight = new Vector2(0.817f, 2.077f);
        Vector2 pointingUp = new Vector2(-0.276f, 3.389f);
        Vector2 pointingDown = new Vector2(-0.548f, 0.964f);
        Vector2 shoulderPos = new Vector2(-0.434f, 2.128f);

        float up = Mathf.Atan2(pointingUp.y - shoulderPos.y, pointingUp.x - shoulderPos.x) * 180 / Mathf.PI;
        float right = Mathf.Atan2(pointingRight.y - shoulderPos.y, pointingRight.x - shoulderPos.x) * 180 / Mathf.PI;
        float down = Mathf.Atan2(pointingDown.y - shoulderPos.y, pointingDown.x - shoulderPos.x) * 180 / Mathf.PI;

        shootDirection = new Vector2(Mathf.Abs(shootDirection.x), shootDirection.y);
        float shootAngle = Mathf.Atan2(shootDirection.y, shootDirection.x) * 180 / Mathf.PI;

        if (shootDirection.y >= 0)
        {
            float slope = (up - right) / 90f;
            float weaponRotation = shootAngle * slope + right;

            float dirSlope = (1.252f - 1.271f) / 90f;
            float weaponDirMagnitude = shootAngle * dirSlope + 1.271f;

            Vector2 gunLocation = weaponDirMagnitude * new Vector2(Mathf.Cos(weaponRotation * Mathf.PI / 180f), Mathf.Sin(weaponRotation * Mathf.PI / 180f)) + shoulderPos;
            gun.transform.localPosition = gunLocation;

            float headSlope = (122f - 92.4f) / 90f;
            head.eulerAngles = new Vector3(head.eulerAngles.x, head.eulerAngles.y, headSlope * shootAngle + 92.4f);
        }

        if (shootDirection.y < 0)
        {
            float slope = (down - right) / -90f;
            float weaponRotation = shootAngle * slope + right;

            float dirSlope = (1.17f - 1.271f) / -90f;
            float weaponDirMagnitude = shootAngle * dirSlope + 1.271f;

            Vector2 gunLocation = weaponDirMagnitude * new Vector2(Mathf.Cos(weaponRotation * Mathf.PI / 180f), Mathf.Sin(weaponRotation * Mathf.PI / 180f)) + shoulderPos;
            gun.transform.localPosition = gunLocation;

            float headSlope = (67f - 92.4f) / -90f;
            head.eulerAngles = new Vector3(head.eulerAngles.x, head.eulerAngles.y, headSlope * shootAngle + 92.4f);
        }


        /*

        if (Input.mousePosition.x >= camera.WorldToScreenPoint(shootingArm.position).x)
        {
            shootingArm.transform.right = aimDirection;
        }
        else
        {
            shootingArm.transform.right = aimDirection;
            shootingArm.localEulerAngles = new Vector3(shootingArm.localEulerAngles.x, 0, 140 - shootingArm.localEulerAngles.z);
        }*/
    }

    private void playerAnimation()
    {
        //if the player isn't mid-air
        if (animator.GetInteger("Phase") != 2)
        {
            //play walking animation if A or D is pressed down
            if (movementDirX != 0)
                setAnimation("walking");
            else
                setAnimation("idle");

            bool facingRight = Input.mousePosition.x >= camera.WorldToScreenPoint(shootingArm.parent.position).x;

            //if you're looking in the opposite direction you're walking, play walking animation backwards
            if (animator.GetInteger("Phase") == 1)
            {
                if ((movementDirX == 1 && facingRight) || movementDirX == -1 && !facingRight)
                    animator.SetFloat("walking speed", 1);
                else if (movementDirX != 0)
                    animator.SetFloat("walking speed", -1);
            }
        }

        //if you are grounded, exit out of jump animation
        if (animator.GetInteger("Phase") == 2 && grounded)
        {
            animator.SetBool("jumped", false);
            setAnimation("idle");
        }
    }


    private bool isGrounded()
    {
        RaycastHit2D leftFootGrounded = Physics2D.Raycast(leftFoot.position, Vector2.down, 0.17f, Constants.map);
        RaycastHit2D rightFootGrounded = Physics2D.Raycast(rightFoot.position, Vector2.down, 0.17f, Constants.map);
        return (rightFootGrounded || leftFootGrounded) ? true : false;
    }

    private void setAnimation(string mode)
    {
        int newMode = 0;

        if (mode == "idle")
            newMode = 0;
        else if (mode == "walking")
            newMode = 1;
        else if (mode == "jumping")
            newMode = 2;
        else
            Debug.LogError("mode not defined");

        //animation progress (always positive regardless of whether animation is played backwards)
        float t = ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) + 1) % 1;

        //if currently walking
        if (animator.GetInteger("Phase") == 1)
        {
            //go idle
            if (newMode == 0 && ((t >= 0.31f && t <= 0.41f) || (t >= 0.8633f && t <= 0.975f)))
                StartCoroutine(walkingToIdle());

            //go jump
            if (newMode == 2 && ((t >= 0.31f && t <= 0.41f) || (t >= 0.773f && t <= 0.975f)))
                animator.SetInteger("Phase", 2);
        }

        else
            animator.SetInteger("Phase", newMode);
    }

    //add slight delay before switching from walking to idle animation 
    //allows for an even cleaner switch from walking to non-idle animations 
    private IEnumerator walkingToIdle()
    {
        yield return new WaitForSeconds(0.045f);

        if (movementDirX != 0)
            yield return null;
        else
            animator.SetInteger("Phase", 0);
    }
}
