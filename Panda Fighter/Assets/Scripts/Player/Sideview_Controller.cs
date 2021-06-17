using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sideview_Controller : MonoBehaviour
{
    private Rigidbody2D rig;
    private SpriteRenderer playerSprite;
    public Transform bulletSpawnPoint;
    private Animator animator;

    private WeaponSystem weaponSystem;
    private GameObject weapon;

    private Camera camera;

    public float speed = 4.0f;
    public float jumpForce = 570;

    public float grenadeThrowForce = 650;
    public float grenadeYForce = -20;
    public float boomerangSpeed = 31;
    public float plasmaBulletSpeed = 30;
    public Vector2 objectSpinSpeed = new Vector2(-200, 200);

    private Transform leftFoot;
    private Transform rightFoot;

    private int movementDirX;

    void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        playerSprite = transform.GetChild(0).transform.GetComponent<SpriteRenderer>();
        camera = transform.GetChild(1).transform.GetComponent<Camera>();

        leftFoot = transform.GetChild(2);
        rightFoot = transform.GetChild(3);

        //animator = transform.GetChild(0).transform.GetComponent<Animator>();
    }

    void Start()
    {
        weaponSystem = transform.GetComponent<WeaponSystem>();
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
        if (Input.GetMouseButtonDown(0) && weaponSystem.weaponSelected != null)
        {
            if (weaponSystem.getAmmo() > 0)
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
                    case "Pistol":
                        shootPlasmaBullet();
                        break;
                    default:
                        Debug.LogError("You haven't specified how to throw this particular object");
                        break;
                }
            }
        }

        playerOrientation();
        //playerAnimation();
    }

    private Vector2 configureObjectForThrowing(Transform ammoSpawn)
    {
        //calculate direction to throw object
        Vector2 throwDir = (Input.mousePosition - camera.WorldToScreenPoint(ammoSpawn.position)).normalized;

        weapon.transform.position = transform.position;
        weapon.layer = LayerMask.NameToLayer("Thrown Object");
        weapon.SetActive(true);

        weapon.transform.GetComponent<Collider2D>().isTrigger = false;
        weapon.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

        return throwDir;
    }

    private void throwGrenade()
    {
        //get throw direction from mouse input
        Vector2 throwDir = configureObjectForThrowing(transform);
        Rigidbody2D objectRig = weapon.transform.GetComponent<Rigidbody2D>();

        //apply a large force to throw the grenade
        Vector2 unadjustedForce = grenadeThrowForce * throwDir * new Vector2(1.2f, 1) + new Vector2(0, grenadeYForce);
        objectRig.velocity = new Vector2(0, 0);
        objectRig.AddForce(unadjustedForce * objectRig.mass);
        Debug.LogFormat("{0}, {1}", unadjustedForce, objectRig.mass);
    }

    private void shootPlasmaBullet()
    {
        //get throw direction from mouse input
        Vector2 throwDir = configureObjectForThrowing(bulletSpawnPoint);
        Rigidbody2D objectRig = weapon.transform.GetComponent<Rigidbody2D>();

        //spawn and orient the bullet correctly
        weapon.transform.position = bulletSpawnPoint.position;
        weapon.transform.right = throwDir;

        //shoot the bullet 
        objectRig.velocity = throwDir * plasmaBulletSpeed;
    }

    private void throwBoomerang()
    {
        //get throw direction from mouse input
        Vector2 throwDir = configureObjectForThrowing(transform);
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

    private void playerOrientation()
    {
        if (Input.mousePosition.x >= Screen.width / 2)
            playerSprite.transform.localRotation = Quaternion.Euler(0, 0, 0);
        else
            playerSprite.transform.localRotation = Quaternion.Euler(0, 180, 0);
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
