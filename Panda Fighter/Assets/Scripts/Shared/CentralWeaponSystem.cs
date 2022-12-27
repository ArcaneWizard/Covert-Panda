using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class CentralWeaponSystem : MonoBehaviour
{
    protected Dictionary<Weapon, int> ammoCount;
    protected Dictionary<Weapon, Transform> bulletPools;
    protected Dictionary<Weapon, WeaponConfiguration> weaponConfigs;
    protected Dictionary<Weapon, WeaponMechanics> weaponMechanics;

    protected Transform allBulletPools;
    protected int bulletPoolIdx = 0;

    public Weapon currentWeapon { private set; get; }
    private List<GameObject> physicalWeaponAndLimbs;

    protected CentralShooting shooting;
    protected CentralLookAround lookAround;
    protected Health health;

    public virtual void Awake()
    {
        shooting = transform.GetComponent<CentralShooting>();
        lookAround = transform.GetComponent<CentralLookAround>();
        health = transform.GetComponent<Health>();

        ammoCount = new Dictionary<Weapon, int>();
        bulletPools = new Dictionary<Weapon, Transform>();
        weaponMechanics = new Dictionary<Weapon, WeaponMechanics>();
        weaponConfigs = new Dictionary<Weapon, WeaponConfiguration>();

        physicalWeaponAndLimbs = new List<GameObject>();
        allBulletPools = transform.parent.GetChild(1).transform.GetChild(0);

        foreach (Transform bulletPool in allBulletPools)
        {
            WeaponTag weapon = bulletPool.GetComponent<WeaponTag>();

            //make each weapon's ammo pool accesible by a dictionary
            bulletPools[weapon] = bulletPool;

            //make each weapon's stats configuration and shooting mechanics accessible by a dictionary
            WeaponConfiguration config = bulletPool.GetComponent<WeaponConfiguration>();
            weaponConfigs[weapon] = config;

            WeaponMechanics mechanics = bulletPool.GetComponent<WeaponMechanics>();
            mechanics.config = config;
            weaponMechanics[weapon] = mechanics;

            //make each weapon's ammo accessible by a dictionary
            ammoCount.Add(weapon, 0);
        }

        WeaponStats weaponStats = new WeaponStats(this);
    }

    private void Start() => InitializeWeaponSystem();

    public virtual void InitializeWeaponSystem() 
    {
        foreach (Weapon weapon in weaponMechanic.Keys)  
            ammoCount[weapon] = 0;
        
        currentWeapon = Weapon.ArcticPistol;
    }

    protected virtual void selectWeapon(Weapon weapon)
    {
        //if the weapon is already selected, no need to do anything
        if (currentWeapon == weapon)
            return;

        currentWeapon = weapon;
        weaponMechanics[currentWeapon].SetDefaultAnimation();
        weaponMechanics[currentWeapon].resetAttackProgress();

        shooting.UpdateCombatMode(CurrentWeaponConfiguration.combatMode);
        lookAround.setAimTarget(CurrentWeaponConfiguration.weaponAimTracker);
        lookAround.calculateShoulderAngles(CurrentWeaponConfiguration.weaponIKCoordinates);

        if (bulletPools.ContainsKey(currentWeapon))
            bulletPoolIdx = ++bulletPoolIdx % bulletPools[currentWeapon].Count;

        // deactivate the old weapon + arm limbs for that weapon 
        foreach (GameObject thing in physicalWeaponAndLimbs)
            thing.SetActive(false);

        physicalWeaponAndLimbs.Clear();

        // store the new weapon + arm limbs and activate them 
        physicalWeaponAndLimbs.Add(CurrentWeaponConfiguration.weapon);
        CurrentWeaponConfiguration.weapon.SetActive(true);

        foreach (GameObject limb in CurrentWeaponConfiguration.limbs)
        {
            physicalWeaponAndLimbs.Add(thing);
            limb.SetActive(true);
        }
    }

    protected virtual void collectNewWeapon(Weapon weapon) => ammoCount[weapon] = weaponConfigs[weapon].startingAmmo;

    protected virtual void useOneAmmo()
    {
        ammoCount[currentWeapon] -= 1;
        bulletPoolIdx = ++bulletPoolIdx % bulletPools[currentWeapon].Count;
    }

    public int CurrentAmmo => ammoCount[currentWeapon];
    public WeaponMechanics CurrentWeaponMechanic => weaponMechanics[currentWeapon];
    public GameObject NextBullet => bulletPools[currentWeapon][bulletPoolIdx].gameObject;
    public WeaponConfiguration CurrentWeaponConfiguration => weaponConfigs[currentWeapon];

    public int GetAmmo(Weapon weapon) => ammoCount[weapon];
    public WeaponMechanics GetWeapon(Weapon weapon) => weaponMechanics[weapon];
    public WeaponConfiguration GetConfiguration(Weapon weapon) => weaponConfigs[weapon];

    // Useful for getting grenades, counting down 1 ammo, and cycling through grenade pool
    public GameObject GetBulletAndUseAmmo(Weapon weapon) 
    {
        ammoCount[weapon.ToString()] -= 1;
        int bulletNumber = ammoCount[weapon.ToString()] % bulletPools[weapon.ToString()].Count;
        return bulletPools[weapon.ToString()][bulletNumber].gameObject;
    }

    // Useful for weapons that shoot more than 1 bullet in a single burst
    public GameObject getLastBullet()
    {
        int totalBullets = bulletPools[currentWeapon].Count;
        return bulletPools[currentWeapon][(bulletPoolIdx + totalBullets - 1) % totalBullets].gameObject;
    }


}
