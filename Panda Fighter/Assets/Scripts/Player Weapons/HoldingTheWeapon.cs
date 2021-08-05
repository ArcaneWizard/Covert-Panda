using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldingTheWeapon : MonoBehaviour
{
    private List<GameObject> WeaponSetup = new List<GameObject>();

    private WeaponSystem weaponSystem;
    private Shooting shooting;
    private Sideview_Controller player_controller;
    private Animator armAnimator;
    private IKTracking iKTracking;


    private void Awake()
    {
        shooting = transform.GetComponent<Shooting>();
        weaponSystem = transform.GetComponent<WeaponSystem>();
        player_controller = transform.GetComponent<Sideview_Controller>();
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

        //set the right aim target for the new weapon
        player_controller.aimTarget = config.aimTarget;

        //activate the animated arms + display the actual weapon )
        if (config.weapon)
            WeaponSetup.Add(config.weapon);
        foreach (GameObject limb in config.limbs)
            WeaponSetup.Add(limb);

        //specify the new bullet point location
        shooting.bulletSpawnPoint = config.bulletSpawnPoint;
        defaultWeaponAnimations(weapon);

        foreach (GameObject limb_Or_Weapon in WeaponSetup)
            limb_Or_Weapon.SetActive(true);

        //use the right coordinates for the weapon's IK aim target
        List<Vector2> aiming = iKTracking.setIKCoordinates(weapon);
        player_controller.calculateShoulderAngles(aiming);
    }

    private void defaultWeaponAnimations(string weapon)
    {
        if (weapon == "Grenade" || weapon == "Plasma Orb" || weapon == "Boomerang")
            armAnimator.SetInteger("Arms Phase", 0);

        else if (weapon == "Scythe")
            armAnimator.SetInteger("Arms Phase", 10);
    }
}
