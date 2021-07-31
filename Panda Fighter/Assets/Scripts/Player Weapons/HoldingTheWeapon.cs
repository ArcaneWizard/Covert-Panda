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


    private void Awake()
    {
        shooting = transform.GetComponent<Shooting>();
        weaponSystem = transform.GetComponent<WeaponSystem>();
        controller = transform.GetComponent<Sideview_Controller>();
        armAnimator = transform.GetComponent<Animator>();
    }

    //specify which limbs, weapon and aim target to activate (the latter helps a weapon track while aiming) 
    public void configureWeaponAndArms()
    {
        string weapon = weaponSystem.weaponSelected;

        //deactivate the previous animated arm limbs + weapon
        foreach (GameObject limb_Or_Weapon in WeaponSetup)
            limb_Or_Weapon.SetActive(false);
        WeaponSetup.Clear();

        //activate the animated arm limbs + weapon + aim target (if needed) + bullet spawn point (if applicable)
        if (weapon == "Grenade" || weapon == "Plasma Orb")
        {
            WeaponSetup.Add(Hand_limb_back);
            WeaponSetup.Add(Hand_limb_front);
            shooting.bulletSpawnPoint = grenadeSpawnPoint;
            armAnimator.SetInteger("Arms Phase", 0);
        }

        else if (weapon == "Pistol")
        {
            WeaponSetup.Add(Gun_limb);
            WeaponSetup.Add(Beamer);
            shooting.bulletSpawnPoint = beamerSpawnPoint;
            controller.aimTarget = beamerTarget;
        }

        else if (weapon == "Boomerang")
        {
            WeaponSetup.Add(Gun_limb);
            WeaponSetup.Add(BoomerangLauncher);
            shooting.bulletSpawnPoint = beamerSpawnPoint;
            controller.aimTarget = beamerTarget;
        }

        else if (weapon == "Scythe")
        {
            WeaponSetup.Add(Scythe_limb);
            WeaponSetup.Add(Scythe);
            controller.aimTarget = scytheTarget;
            armAnimator.SetInteger("Arms Phase", 10);
        }

        else if (weapon == "Sniper")
        {
            WeaponSetup.Add(Long_Barrel_limb);
            WeaponSetup.Add(Sniper);
            shooting.bulletSpawnPoint = sniperBulletPoint;
            controller.aimTarget = longBarrelTarget;
        }

        foreach (GameObject limb_Or_Weapon in WeaponSetup)
            limb_Or_Weapon.SetActive(true);
    }
}
