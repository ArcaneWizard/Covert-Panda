using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class CentralWeaponSystem : MonoBehaviour
{
    protected Dictionary<string, int> ammo = new Dictionary<string, int>();
    protected Dictionary<string, List<Transform>> bulletPools = new Dictionary<string, List<Transform>>();
    protected Dictionary<string, IWeapon> IWeapons = new Dictionary<string, IWeapon>();
    protected Dictionary<string, WeaponConfiguration> weaponConfigurations = new Dictionary<string, WeaponConfiguration>();
    protected Transform allBulletPools;

    public string weaponSelected { private set; get; }
    protected int bulletNumber = 0;

    protected CentralShooting shooting;
    protected CentralLookAround lookAround;
    protected Health health;
    private List<GameObject> Limbs_And_Weapons;

    public virtual void Awake()
    {
        shooting = transform.GetComponent<CentralShooting>();
        lookAround = transform.GetComponent<CentralLookAround>();
        health = transform.GetComponent<Health>();

        Limbs_And_Weapons = new List<GameObject>();
        allBulletPools = transform.parent.GetChild(1).transform.GetChild(0);
        weaponSelected = "";

        foreach (Transform bulletPool in allBulletPools)
        {
            string tag = bulletPool.GetComponent<WeaponTag>().Tag;

            //add each weapon's pool of ammo to a dictionary
            List<Transform> tempBulletPool = new List<Transform>();
            foreach (Transform bullet in bulletPool)
            {
                tempBulletPool.Add(bullet);
            }
            bulletPools.Add(tag, tempBulletPool);

            //add each weapon's shooting instructions and weapon configuration to a dictionary
            IWeapon weapon = bulletPool.GetComponent<IWeapon>();
            IWeapons.Add(tag, weapon);
            WeaponConfiguration config = bulletPool.GetComponent<WeaponConfiguration>();
            weaponConfigurations.Add(tag, config);
            weapon.configuration = config;

            //add each weapon's ammo in a dictionary
            ammo.Add(tag, 0);
        }

        WeaponStats weaponStats = new WeaponStats(this);
    }

    private void Start() => InitializeWeaponSystem();

    public virtual void InitializeWeaponSystem() 
    {
        foreach (KeyValuePair<string, IWeapon> weapon in IWeapons)  
            ammo[weapon.Key] = 0;
        
        weaponSelected = "";
    }

    public virtual void selectWeapon(string weapon)
    {
        //if the weapon is already selected, no need to do anything
        if (weapon == weaponSelected || ammo[weapon] <= 0)
            return;

        weaponSelected = weapon;
        IWeapons[weaponSelected].SetDefaultAnimation();
        IWeapons[weaponSelected].resetAttackProgress();

        shooting.UpdateCombatMode(CurrentWeaponConfiguration.combatMode);
        lookAround.setAimTarget(CurrentWeaponConfiguration.weaponAimTracker);
        lookAround.calculateShoulderAngles(CurrentWeaponConfiguration.weaponIKCoordinates);

        if (bulletPools.ContainsKey(weaponSelected))
            bulletNumber = ++bulletNumber % bulletPools[weaponSelected].Count;

        //deactivate the previous animated arm limbs + enable new ones
        foreach (GameObject limb_or_weapon in Limbs_And_Weapons)
            limb_or_weapon.SetActive(false);
        Limbs_And_Weapons.Clear();

        Limbs_And_Weapons.Add(CurrentWeaponConfiguration.weapon);
        CurrentWeaponConfiguration.weapon.SetActive(true);

        foreach (GameObject limb_or_weapon in CurrentWeaponConfiguration.limbs)
        {
            Limbs_And_Weapons.Add(limb_or_weapon);
            limb_or_weapon.SetActive(true);
        }
    }

    public virtual void collectNewWeapon(string weapon) => ammo[weapon] = IWeapons[weapon].configuration.startingAmmo;

    public virtual void useOneAmmo()
    {
        ammo[weaponSelected] -= 1;
        bulletNumber = ++bulletNumber % bulletPools[weaponSelected].Count;
    }

    public int CurrentAmmo => ammo[weaponSelected];
    public IWeapon CurrentWeapon => IWeapons[weaponSelected];
    public GameObject CurrentBullet => bulletPools[weaponSelected][bulletNumber].gameObject;
    public WeaponConfiguration CurrentWeaponConfiguration => weaponConfigurations[weaponSelected];

    public int GetAmmo(WeaponTags weapon) => ammo[weapon.ToString()];
    public IWeapon GetWeapon(WeaponTags weapon) => IWeapons[weapon.ToString()];
    public WeaponConfiguration GetWeaponConfiguration(WeaponTags weapon) => weaponConfigurations[weapon.ToString()];

    // Useful for getting grenades, counting down 1 ammo, and cycling through grenade pool
    public GameObject GetBulletAndUseAmmo(WeaponTags weapon) 
    {
        ammo[weapon.ToString()] -= 1;
        int bulletNumber = ammo[weapon.ToString()] % bulletPools[weapon.ToString()].Count;
        return bulletPools[weapon.ToString()][bulletNumber].gameObject;
    }

    // Useful for weapons that shoot more than 1 bullet in a single burst
    public GameObject getLastBullet()
    {
        int totalBullets = bulletPools[weaponSelected].Count;
        return bulletPools[weaponSelected][(bulletNumber + totalBullets - 1) % totalBullets].gameObject;
    }


}
