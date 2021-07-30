using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Shooting : MonoBehaviour
{

    [Header("Entities")]
    public Camera camera;
    public Transform shootingArm;
    [HideInInspector]
    public Animator armAnimator;

    private WeaponSystem weaponSystem;
    private Sideview_Controller sideview_Controller;

    private Rigidbody2D rig;
    private GameObject ammunition = null;
    private string weapon;

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
    public Transform handHeldSpawnPoint;

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
            weapon = weaponSystem.weaponSelected;

            if (combatMode != "meelee" && weaponSystem.getAmmo() > 0 && weaponSystem.getWeapon().tag == "singleFire")
            {
                ammunition = weaponSystem.getWeapon();
                weaponSystem.useOneAmmo();
                objectRig = ammunition.transform.GetComponent<Rigidbody2D>();

                if (combatMode == "gun")
                    StartCoroutine(aimWithHands());

                if (weapon == "Grenade")
                    StartCoroutine(throwGrenade());
                else if (weapon == "Boomerang")
                    shootBoomerang();
                else if (weapon == "Plasma Orb")
                    StartCoroutine(throwPlasmaOrb());
                else
                    Debug.LogError("You haven't specified how to shoot this particular object");
            }

            else if (combatMode == "meelee")
            {
                if (weapon == "Scythe")
                    attackWithScythe();
                else
                    Debug.LogError("You haven't specified how to shoot this particular object");
            }
        }


        //Weapons where you can hold the right mouse button down to continously use and drain the weapon
        if (Input.GetMouseButton(0) && weaponSystem.weaponSelected != null)
        {
            if (combatMode != "meelee" && weaponSystem.getAmmo() > 0 && timeLeftBtwnShots <= 0 && weaponSystem.getWeapon().tag == "spamFire")
            {
                ammunition = weaponSystem.getWeapon();
                weaponSystem.useOneAmmo();
                timeLeftBtwnShots = plasmaFireRate;
                objectRig = ammunition.transform.GetComponent<Rigidbody2D>();
                weapon = weaponSystem.weaponSelected;

                if (weapon == "Pistol")
                    shootPlasmaBullet();
                else
                    Debug.LogError("You haven't specified how to shoot this particular object");
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

                weaponHeld.transform.position = handHeldSpawnPoint.position;
                weaponHeld.transform.rotation = handHeldSpawnPoint.rotation;
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

        ammunition.transform.position = gunSpawnPoint.position;
        ammunition.SetActive(true);

        ammunition.transform.GetComponent<Collider2D>().isTrigger = false;
        ammunition.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    }


    private IEnumerator aimWithHands()
    {
        //calculate direction to throw or shoot object in
        aimDir = (Input.mousePosition - camera.WorldToScreenPoint(shootingArm.position)).normalized;
        armAnimator.SetInteger("Arms Phase", 1);

        yield return new WaitForSeconds(0.299f);

        ammunition.transform.position = handHeldSpawnPoint.position;
        ammunition.transform.GetComponent<Collider2D>().isTrigger = false;
        ammunition.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        ammunition.SetActive(true);
        weaponThrown = ammunition;
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
        ammunition.transform.right = aimDir;
        objectRig.velocity = aimDir * plasmaBulletSpeed;
    }

    private void shootBoomerang()
    {
        //get aim direction from mouse input
        ammunition.transform.GetComponent<Animator>().SetBool("glare", false);
        aimWithGun();

        //set the boomerang's velocity really high
        objectRig.velocity = aimDir * boomerangSpeed;
        objectRig.angularVelocity = Random.Range(boomerangSpinSpeed.x, boomerangSpinSpeed.y) * (Random.Range(0, 2) * 2 - 1);
    }

    private void curveBoomerang()
    {
        if (weaponSystem.weaponSelected == "Boomerang" && Input.GetMouseButtonDown(1) && ammunition && ammunition.activeSelf)
        {
            if (aimDir.x >= 0)
                objectRig.velocity = Quaternion.Euler(0, 0, -90) * aimDir * boomerangSpeed;
            else
                objectRig.velocity = Quaternion.Euler(0, 0, 90) * aimDir * boomerangSpeed;

            objectRig.angularVelocity = Random.Range(boomerangSpinSpeed.x, boomerangSpinSpeed.y) * (Random.Range(0, 2) * 2 - 1);
            ammunition.transform.GetComponent<Animator>().SetBool("glare", true);
        }
    }

    private void attackWithScythe()
    {
        aimDir = (Input.mousePosition - camera.WorldToScreenPoint(shootingArm.position)).normalized;
    }

}
