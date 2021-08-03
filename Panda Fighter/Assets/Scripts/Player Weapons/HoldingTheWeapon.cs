using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldingTheWeapon : MonoBehaviour
{
    [Header("Arm Bones + Sprites")]
    public GameObject Gun_limb;
    public GameObject Hand_limb_front, Hand_limb_back, Scythe_limb, Long_Barrel_limb;

    [Header("Weapons")]
    public GameObject Beamer;
    public GameObject BoomerangLauncher, Scythe, Sniper;

    [Header("IK targets")]
    public Transform beamerTarget;
    public Transform scytheTarget;
    public Transform longBarrelTarget;

    [Header("Ammo spawn points")]
    public Transform beamerSpawnPoint;
    public Transform grenadeSpawnPoint;
    public Transform sniperBulletPoint;

    private List<GameObject> WeaponSetup = new List<GameObject>();

    private WeaponSystem weaponSystem;
    private Shooting shooting;
    private Sideview_Controller controller;
    private Animator armAnimator;
    private IKTracking iKTracking;


    private void Awake()
    {
        shooting = transform.GetComponent<Shooting>();
        weaponSystem = transform.GetComponent<WeaponSystem>();
        controller = transform.GetComponent<Sideview_Controller>();
        armAnimator = transform.GetComponent<Animator>();
        iKTracking = transform.GetComponent<IKTracking>();
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

        //activate the animated arm limbs + weapon and configure the aim target + bullet spawn point (if applicable)
        if (config.weapon)
            WeaponSetup.Add(config.weapon);
        foreach (GameObject limb in config.limbs)
            WeaponSetup.Add(limb);

        shooting.bulletSpawnPoint = config.bulletSpawnPoint;
        controller.aimTarget = config.aimTarget;
        defaultWeaponAnimations(weapon);

        foreach (GameObject limb_Or_Weapon in WeaponSetup)
            limb_Or_Weapon.SetActive(true);

        //use the right coordinates for the weapon's IK aim target
        iKTracking.setIKCoordinates(weapon);
    }

    private void defaultWeaponAnimations(string weapon)
    {
        if (weapon == "Grenade" || weapon == "Plasma Orb" || weapon == "Boomerang")
            armAnimator.SetInteger("Arms Phase", 0);

        else if (weapon == "Scythe")
            armAnimator.SetInteger("Arms Phase", 10);
    }
}
