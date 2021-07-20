using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{

    public Camera camera;
    public Transform shootingArm;

    private WeaponSystem weaponSystem;
    private Sideview_Controller sideview_Controller;
    private Rigidbody2D rig;
    private GameObject weapon;
    private float timeLeftBtwnShots;

    public Transform bulletSpawnPoint;

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
    }

    void Update()
    {
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
        //Player is on a levitation boost platform and clicks W -> give them a jump boost 
        if (collision.gameObject.tag == "Levitation" && Input.GetKeyDown(KeyCode.W) && sideview_Controller.grounded)
            rig.AddForce(Constants.levitationBoost);
    }
}
