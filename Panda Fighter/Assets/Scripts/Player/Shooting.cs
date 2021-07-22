using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{

    [Header("Entities")]
    public Camera camera;
    public Transform shootingArm;
    private Animator armAnimator;

    private WeaponSystem weaponSystem;
    private Sideview_Controller sideview_Controller;
    private Rigidbody2D rig;
    private GameObject weapon;

    private float timeLeftBtwnShots;
    private Rigidbody2D objectRig;
    private Vector2 aimDir;


    [Header("Shooting + Throwing")]
    public Transform gunSpawnPoint;
    public Transform handSpawnPoint;
    public GameObject shoulderBones_GUN;
    public GameObject shoulderSprites_GUN;
    public GameObject frontShoulderBone_HAND;
    public GameObject backShoulderBone_HAND;

    private float grenadeThrowForce = 1800;
    private float grenadeYForce = -20;
    private float boomerangSpeed = 41;
    private float plasmaBulletSpeed = 30;
    private float plasmaFireRate = 0.16f;
    private Vector2 objectSpinSpeed = new Vector2(-200, 200);

    void Awake()
    {
        weaponSystem = transform.GetComponent<WeaponSystem>();
        sideview_Controller = transform.GetComponent<Sideview_Controller>();

        rig = transform.GetComponent<Rigidbody2D>();
        armAnimator = transform.GetComponent<Animator>();
    }

    void Update()
    {
        //after throwing, go back to normal swinging hands walking state
        if (armAnimator.GetInteger("Arms Phase") == 1)
            armAnimator.SetInteger("Arms Phase", 0);

        //Weapons where you right click to throw/shoot 
        if (Input.GetMouseButtonDown(0) && weaponSystem.weaponSelected != null)
        {
            if (weaponSystem.getAmmo() > 0 && weaponSystem.getWeapon().tag == "singleFire")
            {
                weapon = weaponSystem.getWeapon();
                weaponSystem.useOneAmmo();
                objectRig = weapon.transform.GetComponent<Rigidbody2D>();

                switch (weaponSystem.weaponSelected)
                {
                    case "Grenade":
                        StartCoroutine(throwGrenade());
                        break;
                    case "Boomerang":
                        shootBoomerang();
                        break;
                    case "Plasma Orb":
                        StartCoroutine(throwPlasmaOrb());
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
                weaponSystem.useOneAmmo();
                timeLeftBtwnShots = plasmaFireRate;
                objectRig = weapon.transform.GetComponent<Rigidbody2D>();


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
    }


    private void aimWithGun()
    {
        //calculate direction to throw or shoot object in
        aimDir = (Input.mousePosition - camera.WorldToScreenPoint(shootingArm.position)).normalized;

        weapon.transform.position = gunSpawnPoint.position;
        weapon.layer = LayerMask.NameToLayer("Deployed Object");
        weapon.SetActive(true);

        weapon.transform.GetComponent<Collider2D>().isTrigger = false;
        weapon.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    }

    private IEnumerator aimWithHands()
    {
        //calculate direction to throw or shoot object in
        aimDir = (Input.mousePosition - camera.WorldToScreenPoint(shootingArm.position)).normalized;
        armAnimator.SetInteger("Arms Phase", 1);

        yield return new WaitForSeconds(0.3f);

        weapon.transform.position = handSpawnPoint.position;
        weapon.layer = LayerMask.NameToLayer("Deployed Object");
        weapon.SetActive(true);

        weapon.transform.GetComponent<Collider2D>().isTrigger = false;
        weapon.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    }

    private IEnumerator throwGrenade()
    {
        //get aim direction from mouse input
        StartCoroutine(aimWithHands());
        yield return new WaitForSeconds(0.3f);

        //apply a large force to throw the grenadeaa
        Vector2 unadjustedForce = grenadeThrowForce * aimDir * new Vector2(1.2f, 1) + new Vector2(0, grenadeYForce);
        objectRig.velocity = new Vector2(0, 0);
        objectRig.AddForce(unadjustedForce * objectRig.mass);
    }


    private IEnumerator throwPlasmaOrb()
    {
        //get aim direction from mouse input
        StartCoroutine(aimWithHands());
        yield return new WaitForSeconds(0.3f);

        //apply a large force to throw the grenade
        Vector2 unadjustedForce = grenadeThrowForce * aimDir * new Vector2(1.2f, 1) + new Vector2(0, grenadeYForce);
        objectRig.velocity = new Vector2(0, 0);
        objectRig.AddForce(unadjustedForce * objectRig.mass);
    }

    private void shootPlasmaBullet()
    {
        //get aim direction from mouse input
        aimWithGun();

        //spawn and orient the bullet correctly
        weapon.transform.right = aimDir;
        objectRig.velocity = aimDir * plasmaBulletSpeed;
    }

    private void shootBoomerang()
    {
        //get aim direction from mouse input
        aimWithGun();

        //set the boomerang's velocity really high
        objectRig.velocity = aimDir * boomerangSpeed;
        objectRig.angularVelocity = Random.Range(objectSpinSpeed.x, objectSpinSpeed.y);
    }

    public void switchMode(string type)
    {
        if (type == "gun")
        {
            shoulderBones_GUN.SetActive(true);
            shoulderSprites_GUN.SetActive(true);
            frontShoulderBone_HAND.gameObject.SetActive(false);
            backShoulderBone_HAND.gameObject.SetActive(false);
        }

        else if (type == "hands")
        {
            shoulderBones_GUN.SetActive(false);
            shoulderSprites_GUN.SetActive(false);
            frontShoulderBone_HAND.gameObject.SetActive(true);
            backShoulderBone_HAND.gameObject.SetActive(true);

            armAnimator.SetInteger("Arms Phase", 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        //Player collides with weapon, so equip it
        if (col.gameObject.layer == LayerMask.NameToLayer("Object"))
        {
            weaponSystem.EquipNewWeapon(col.gameObject.tag, 25);
            col.gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        //Player is on a levitation boost platform and clicks W -> give them a jump boost 
        if (col.gameObject.tag == "Levitation" && Input.GetKeyDown(KeyCode.W) && sideview_Controller.grounded)
            rig.AddForce(Constants.levitationBoost);
    }
}
