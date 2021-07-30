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
    private GameObject weapon = null;

    [HideInInspector]
    public GameObject weaponHeld;
    private GameObject weaponThrown;
    private GameObject lastBoomerangThrown;

    [HideInInspector]
    public string combatMode = "gun";

    private float timeLeftBtwnShots;
    private Rigidbody2D objectRig;
    private Vector2 aimDir;

    [Header("Bullet Spawn Points")]
    public Transform gunSpawnPoint;
    public Transform handSpawnPoint;
    public Transform grenadeSpawnPoint;

    [Header("Arm Bones + Sprites")]
    public GameObject Gun_limb;
    public GameObject Hand_limb_front, Hand_limb_back, Scythe_limb;

    [Header("Weapons")]
    public GameObject Beamer;
    public GameObject BoomerangLauncher, Scythe;

    private List<GameObject> WeaponSetup = new List<GameObject>();

    private float grenadeThrowForce = 1800;
    private float grenadeYForce = -20;
    private float boomerangSpeed = 41;
    private float plasmaBulletSpeed = 30;
    private float plasmaFireRate = 0.16f;
    private Vector2 boomerangSpinSpeed = new Vector2(180, 250);

    void Awake()
    {
        weaponSystem = transform.GetComponent<WeaponSystem>();
        sideview_Controller = transform.GetComponent<Sideview_Controller>();

        rig = transform.GetComponent<Rigidbody2D>();
        armAnimator = transform.GetComponent<Animator>();
    }

    void Update()
    {
        curveBoomerang();

        //after throwing, go back to normal swinging hands walking state
        if (armAnimator.GetInteger("Arms Phase") == 1)
            armAnimator.SetInteger("Arms Phase", 0);

        //Weapons where you right click but don't hold down the right mouse button
        if (Input.GetMouseButtonDown(0) && weaponSystem.weaponSelected != null)
        {
            if (combatMode != "meelee" && weaponSystem.getAmmo() > 0 && weaponSystem.getWeapon().tag == "singleFire")
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

            else if (combatMode == "meelee")
            {
                switch (weaponSystem.weaponSelected)
                {
                    case "Scythe":
                        attackWithScythe();
                        break;
                    default:
                        Debug.LogError("You haven't specified how to shoot this particular object");
                        break;
                }
            }
        }


        //Weapons where you can hold the right mouse button down to continously use and drain the weapon
        if (Input.GetMouseButton(0) && weaponSystem.weaponSelected != null)
        {
            if (combatMode != "meelee" && weaponSystem.getAmmo() > 0 && timeLeftBtwnShots <= 0 && weaponSystem.getWeapon().tag == "spamFire")
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

    private void LateUpdate()
    {
        //the grenade or holdable weapon stays in your palm during normal animation cycles until its thrown
        if (combatMode == "handheld")
        {
            if (!GameObject.Equals(weaponHeld, weaponThrown) && weaponSystem.getAmmo() > 0)
            {
                weaponHeld.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                weaponHeld.transform.GetComponent<Collider2D>().isTrigger = true;

                weaponHeld.transform.position = grenadeSpawnPoint.position;
                weaponHeld.transform.rotation = grenadeSpawnPoint.rotation;
                weaponHeld.SetActive(true);
            }
            else
                weaponHeld = weaponSystem.getWeapon();
        }
    }


    private void aimWithGun()
    {
        //calculate direction to throw or shoot object in
        aimDir = (Input.mousePosition - camera.WorldToScreenPoint(shootingArm.position)).normalized;

        weapon.transform.position = gunSpawnPoint.position;
        weapon.SetActive(true);

        weapon.transform.GetComponent<Collider2D>().isTrigger = false;
        weapon.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    }


    private IEnumerator aimWithHands()
    {
        //calculate direction to throw or shoot object in
        aimDir = (Input.mousePosition - camera.WorldToScreenPoint(shootingArm.position)).normalized;
        armAnimator.SetInteger("Arms Phase", 1);

        yield return new WaitForSeconds(0.299f);

        weapon.transform.position = grenadeSpawnPoint.position;
        weapon.transform.GetComponent<Collider2D>().isTrigger = false;
        weapon.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        weapon.SetActive(true);
        weaponThrown = weapon;
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
        weapon.transform.GetComponent<Animator>().SetBool("glare", false);
        aimWithGun();

        //set the boomerang's velocity really high
        objectRig.velocity = aimDir * boomerangSpeed;
        objectRig.angularVelocity = Random.Range(boomerangSpinSpeed.x, boomerangSpinSpeed.y) * (Random.Range(0, 2) * 2 - 1);
    }

    private void curveBoomerang()
    {
        if (weaponSystem.weaponSelected == "Boomerang" && Input.GetMouseButtonDown(1) && weapon && weapon.activeSelf)
        {
            if (aimDir.x >= 0)
                objectRig.velocity = Quaternion.Euler(0, 0, -90) * aimDir * boomerangSpeed;
            else
                objectRig.velocity = Quaternion.Euler(0, 0, 90) * aimDir * boomerangSpeed;

            objectRig.angularVelocity = Random.Range(boomerangSpinSpeed.x, boomerangSpinSpeed.y) * (Random.Range(0, 2) * 2 - 1);
            weapon.transform.GetComponent<Animator>().SetBool("glare", true);
        }
    }

    private void attackWithScythe()
    {
        aimDir = (Input.mousePosition - camera.WorldToScreenPoint(shootingArm.position)).normalized;
    }

    public void configureWeaponAndArms()
    {
        string weapon = weaponSystem.weaponSelected;

        //deactivate the previous animated arm limbs + weapon
        foreach (GameObject limb_Or_Weapon in WeaponSetup)
            limb_Or_Weapon.SetActive(false);
        WeaponSetup.Clear();

        if (weapon == "Grenade" || weapon == "Plasma Orb")
        {
            WeaponSetup.Add(Hand_limb_back);
            WeaponSetup.Add(Hand_limb_front);
            armAnimator.SetInteger("Arms Phase", 0);
        }

        else if (weapon == "Pistol")
        {
            WeaponSetup.Add(Gun_limb);
            WeaponSetup.Add(Beamer);
        }

        else if (weapon == "Boomerang")
        {
            WeaponSetup.Add(Gun_limb);
            WeaponSetup.Add(BoomerangLauncher);
        }

        else if (weapon == "Scythe")
        {
            WeaponSetup.Add(Scythe_limb);
            WeaponSetup.Add(Scythe);
        }

        foreach (GameObject limb_Or_Weapon in WeaponSetup)
            limb_Or_Weapon.SetActive(true);
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
