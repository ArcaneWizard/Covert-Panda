using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Shooting : MonoBehaviour
{

    [Header("Entities")]
    public Camera camera;
    public Transform shootingArm;

    private WeaponSystem weaponSystem;
    private Sideview_Controller sideview_Controller;
    private WeaponAttacks weaponAttacks;

    private Rigidbody2D rig;
    private GameObject ammunition = null;
    [HideInInspector]
    public Vector2 aimDir;
    private float wait;

    public GameObject weaponHeld;
    private GameObject weaponThrown;
    private GameObject lastBoomerangThrown;

    public string combatMode = "gun";

    [HideInInspector]
    public float timeLeftBtwnShots;
    private Rigidbody2D objectRig;

    public Transform bulletSpawnPoint;

    void Awake()
    {
        weaponSystem = transform.GetComponent<WeaponSystem>();
        weaponAttacks = transform.GetComponent<WeaponAttacks>();
        sideview_Controller = transform.GetComponent<Sideview_Controller>();

        rig = transform.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        weaponAttacks.curveBoomerang();

        //Weapons where you right click but don't hold down the right mouse button
        if (Input.GetMouseButtonDown(0) && weaponSystem.weaponSelected != null)
        {
            if (combatMode == "gun" && weaponSystem.getAmmo() > 0 && weaponSystem.getWeapon().tag == "singleFire")
                singleFireAttack();

            else if (combatMode == "handheld" && !weaponAttacks.attackAnimationPlaying && weaponSystem.getAmmo() > 0 && weaponSystem.getWeapon().tag == "singleFire")
                singleFireAttack();

            else if (combatMode == "meelee" && !weaponAttacks.attackAnimationPlaying)
            {
                weaponAttacks.updateEntities(ammunition, objectRig);
                weaponAttacks.meeleeAttack(weaponSystem.weaponSelected);
            }

        }

        //Weapons where you can hold the right mouse button down to continously use and drain the weapon
        if (Input.GetMouseButton(0) && weaponSystem.weaponSelected != null)
        {
            if (combatMode == "gun" && weaponSystem.getAmmo() > 0 && timeLeftBtwnShots <= 0 && weaponSystem.getWeapon().tag == "spamFire")
            {
                ammunition = weaponSystem.getWeapon();
                weaponSystem.useOneAmmo();
                objectRig = ammunition.transform.GetComponent<Rigidbody2D>();

                weaponAttacks.updateEntities(ammunition, objectRig);
                weaponAttacks.spamFireAttack(weaponSystem.weaponSelected);
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

                weaponHeld.transform.position = bulletSpawnPoint.position;
                weaponHeld.transform.rotation = bulletSpawnPoint.rotation;
                weaponHeld.SetActive(true);
            }
            else
                weaponHeld = weaponSystem.getWeapon();
        }
    }

    private void singleFireAttack()
    {
        ammunition = weaponSystem.getWeapon();
        weaponSystem.useOneAmmo();
        objectRig = ammunition.transform.GetComponent<Rigidbody2D>();

        weaponAttacks.updateEntities(ammunition, objectRig);
        weaponAttacks.singleFireAttack(weaponSystem.weaponSelected);
    }

    public void shootAnotherBullet()
    {
        ammunition = weaponSystem.getWeapon();
        weaponSystem.useOneAmmo();
        objectRig = ammunition.transform.GetComponent<Rigidbody2D>();
        weaponAttacks.updateEntities(ammunition, objectRig);
    }

    public void aimWithGun()
    {
        //calculate direction to throw or shoot object in
        aimDir = (Input.mousePosition - camera.WorldToScreenPoint(shootingArm.position)).normalized;

        ammunition.transform.position = bulletSpawnPoint.position;
        ammunition.transform.GetComponent<Collider2D>().isTrigger = false;
        ammunition.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        ammunition.transform.GetComponent<Rigidbody2D>().angularVelocity = 0;

        ammunition.SetActive(false);
        ammunition.SetActive(true);
    }


    public IEnumerator aimWithHands(float trackingMultiplier, float trackingOffset)
    {
        //calculate direction to throw or shoot object in
        aimDir = (Input.mousePosition - camera.WorldToScreenPoint(shootingArm.position)).normalized;
        wait = ((-aimDir.y + 1) * trackingMultiplier + trackingOffset);

        weaponAttacks.shouldThrow = false;
        weaponAttacks.attackAnimationPlaying = true;

        Debug.Log(wait);
        yield return new WaitForSeconds(wait);
        ammunition.transform.position = bulletSpawnPoint.position;
        weaponThrown = ammunition;

        ammunition.transform.GetComponent<Collider2D>().isTrigger = false;
        ammunition.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        ammunition.transform.GetComponent<Rigidbody2D>().angularVelocity = 0;
        ammunition.SetActive(false);
        ammunition.SetActive(true);
        ammunition.transform.localEulerAngles = new Vector3(0, 0, ammunition.transform.localEulerAngles.z);

        weaponAttacks.shouldThrow = true;
    }

}
