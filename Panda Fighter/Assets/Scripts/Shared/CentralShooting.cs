using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralShooting : MonoBehaviour
{
    [Header("Entities")]
    public Camera camera;
    public Transform shootingArm;

    protected Rigidbody2D rig;
    protected GameObject ammunition = null;
    [HideInInspector]
    public Vector2 aimDir;
    protected float wait;

    public GameObject weaponHeld = null;
    protected GameObject weaponThrown;
    protected GameObject lastBoomerangThrown;

    public string combatMode = "gun";

    [HideInInspector]
    public float timeLeftBtwnShots;
    protected Rigidbody2D objectRig;

    [HideInInspector]
    public Transform bulletSpawnPoint;

    protected CentralWeaponAttacks weaponAttacks;
    protected CentralWeaponSystem weaponSystem;

    public void Awake()
    {
        weaponAttacks = transform.GetComponent<CentralWeaponAttacks>();
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        rig = transform.GetComponent<Rigidbody2D>();
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

    protected void retrieveWeaponAmmunitionThenAttack()
    {
        ammunition = weaponSystem.getWeapon();
        weaponSystem.useOneAmmo();
        objectRig = ammunition.transform.GetComponent<Rigidbody2D>();

        weaponAttacks.updateEntities(ammunition, objectRig);
        weaponAttacks.singleFireAttack(weaponSystem.weaponSelected);
    }

    public void retrieveWeaponAmmunition()
    {
        ammunition = weaponSystem.getWeapon();
        weaponSystem.useOneAmmo();
        objectRig = ammunition.transform.GetComponent<Rigidbody2D>();
        weaponAttacks.updateEntities(ammunition, objectRig);
    }

    public void SetupGunBullet()
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


    public IEnumerator SetupThrowingObject(float trackingMultiplier, float trackingOffset)
    {
        //calculate direction to throw or shoot object in
        aimDir = (Input.mousePosition - camera.WorldToScreenPoint(shootingArm.position)).normalized;
        wait = ((-aimDir.y + 1) * trackingMultiplier + trackingOffset);

        weaponAttacks.shouldThrow = false;
        weaponAttacks.attackAnimationPlaying = true;

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

    public IEnumerator SetupThrowableBlade(float trackingMultiplier, float trackingOffset)
    {
        //calculate direction to throw or shoot object in
        aimDir = (Input.mousePosition - camera.WorldToScreenPoint(shootingArm.position)).normalized;
        wait = ((-aimDir.y + 1) * trackingMultiplier + trackingOffset);

        weaponAttacks.shouldThrow = false;
        weaponAttacks.attackAnimationPlaying = true;


        yield return new WaitForSeconds(wait);

        WeaponConfig config = weaponSystem.weaponConfigurations[weaponSystem.weaponSelected];
        ammunition.transform.position = config.weapon.transform.position;
        ammunition.transform.localEulerAngles = config.weapon.transform.localEulerAngles;
        ammunition.transform.localScale = config.weapon.transform.localScale;

        config.weapon.gameObject.SetActive(false);

        ammunition.transform.GetComponent<Collider2D>().isTrigger = false;
        ammunition.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        ammunition.transform.GetComponent<Rigidbody2D>().angularVelocity = 0;
        ammunition.SetActive(false);
        ammunition.SetActive(true);
        ammunition.transform.localEulerAngles = new Vector3(0, 0, ammunition.transform.localEulerAngles.z);

        weaponAttacks.shouldThrow = true;
    }

}
