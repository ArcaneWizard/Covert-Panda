using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralShooting : MonoBehaviour
{
    [Header("Entities")]
    public Camera camera;
    [HideInInspector]
    public Transform shootingArm;

    protected Rigidbody2D rig;
    protected Animator armAnimator;

    [HideInInspector]
    public Vector2 aimDir;
    protected float wait;

    public GameObject weaponHeld = null;
    protected GameObject weaponThrown;
    protected GameObject lastBoomerangThrown;

    public string combatMode = "gun";

    [HideInInspector]
    public float timeLeftBtwnShots;

    [HideInInspector]
    public Transform bulletSpawnPoint;

    protected CentralWeaponAttacks weaponAttacks;
    protected CentralWeaponSystem weaponSystem;
    protected CentralController controller;

    private List<GameObject> WeaponSetup = new List<GameObject>();

    public void Awake()
    {
        weaponAttacks = transform.GetComponent<CentralWeaponAttacks>();
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        controller = transform.GetComponent<CentralController>();

        armAnimator = transform.GetComponent<Animator>();
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

    //Get direction to aim in, and spawn the ammunition at the right gun tip
    public void SpawnGunBulletAndAim(GameObject ammunition)
    {
        aimDir = calculateAimDirection();
        ammunition.transform.position = bulletSpawnPoint.position;
        resetAmmunitionSettingsWhenSpawned(ammunition);
    }

    //Get direction to aim in, help track the object on the hand, and confirm when the "ammunition" can be let go off / force applied
    public IEnumerator HandleThrowingMechanics(GameObject ammunition, float trackingMultiplier, float trackingOffset)
    {
        aimDir = calculateAimDirection();
        wait = ((-aimDir.y + 1) * trackingMultiplier + trackingOffset);

        weaponAttacks.shouldLetGoOfObject = false;
        weaponAttacks.attackAnimationPlaying = true;
        yield return new WaitForSeconds(wait);

        ammunition.transform.position = bulletSpawnPoint.position;
        weaponThrown = ammunition;

        resetAmmunitionSettingsWhenSpawned(ammunition);
        weaponAttacks.shouldLetGoOfObject = true;
    }

    //Get direction to aim in, help track the object on the hand, and confirm when the "ammunition" can be let go off / force applied
    public IEnumerator HandleThrowableBladeMechanics(GameObject ammunition, float trackingMultiplier, float trackingOffset)
    {
        aimDir = calculateAimDirection();
        wait = ((-aimDir.y + 1) * trackingMultiplier + trackingOffset);

        weaponAttacks.shouldLetGoOfObject = false;
        weaponAttacks.attackAnimationPlaying = true;
        yield return new WaitForSeconds(wait);

        WeaponConfig config = weaponSystem.weaponConfigurations[weaponSystem.weaponSelected];
        ammunition.transform.position = config.weapon.transform.position;
        ammunition.transform.localEulerAngles = config.weapon.transform.localEulerAngles;
        ammunition.transform.localScale = config.weapon.transform.localScale;
        config.weapon.gameObject.SetActive(false);

        resetAmmunitionSettingsWhenSpawned(ammunition);
        weaponAttacks.shouldLetGoOfObject = true;
    }

    private void resetAmmunitionSettingsWhenSpawned(GameObject ammunition)
    {
        ammunition.transform.GetComponent<Collider2D>().isTrigger = false;
        ammunition.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        ammunition.transform.GetComponent<Rigidbody2D>().angularVelocity = 0;
        ammunition.SetActive(false);
        ammunition.SetActive(true);
        ammunition.transform.localEulerAngles = new Vector3(0, 0, ammunition.transform.localEulerAngles.z);
    }

    private Vector2 calculateAimDirection()
    {
        return (Input.mousePosition - camera.WorldToScreenPoint(shootingArm.position)).normalized;
    }

    //specify which limbs, weapon and aim target to activate (the latter helps a weapon track while aiming) 
    public void configureWeaponAndArms()
    {
        string weapon = weaponSystem.weaponSelected;
        WeaponConfig config = weaponSystem.weaponConfigurations[weapon];

        //deactivate the previous animated arm limbs + weapon
        foreach (GameObject limb_Or_Weapon in WeaponSetup)
            limb_Or_Weapon.SetActive(false);
        WeaponSetup.Clear();

        //set the right aim target for the new weapon
        controller.setAimTarget(config.aimTarget);

        //activate the animated arms + display the actual weapon )
        if (config.weapon)
            WeaponSetup.Add(config.weapon);
        foreach (GameObject limb in config.limbs)
            WeaponSetup.Add(limb);

        //specify the new bullet point location
        bulletSpawnPoint = config.bulletSpawnPoint;
        defaultWeaponAnimations(weapon);

        foreach (GameObject limb_Or_Weapon in WeaponSetup)
            limb_Or_Weapon.SetActive(true);

        //use the right coordinates for the weapon's IK aim target
        List<Vector2> aiming = IKTracking.setIKCoordinates(weapon);
        controller.calculateShoulderAngles(aiming);
    }

    private void defaultWeaponAnimations(string weapon)
    {
        if (weapon == "Grenade" || weapon == "Plasma Orb" || weapon == "Boomerang")
            armAnimator.SetInteger("Arms Phase", 0);

        else if (weapon == "Scythe")
            armAnimator.SetInteger("Arms Phase", 10);
    }

}
