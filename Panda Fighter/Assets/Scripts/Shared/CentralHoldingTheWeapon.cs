using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralHoldingTheWeapon : MonoBehaviour
{
    protected List<GameObject> WeaponSetup = new List<GameObject>();

    protected Animator armAnimator;
    protected IKTracking iKTracking;
    protected CentralWeaponSystem weaponSystem;
    protected CentralController controller;
    protected CentralShooting shooting;


    public virtual void Awake()
    {
        armAnimator = transform.GetComponent<Animator>();
        iKTracking = transform.GetComponent<IKTracking>();
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        controller = transform.GetComponent<CentralController>();
        shooting = transform.GetComponent<CentralShooting>();
    }

    protected void defaultWeaponAnimations(string weapon)
    {
        if (weapon == "Grenade" || weapon == "Plasma Orb" || weapon == "Boomerang")
            armAnimator.SetInteger("Arms Phase", 0);

        else if (weapon == "Scythe")
            armAnimator.SetInteger("Arms Phase", 10);
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
        controller.aimTarget = config.aimTarget;

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
        controller.calculateShoulderAngles(aiming);
    }

}
