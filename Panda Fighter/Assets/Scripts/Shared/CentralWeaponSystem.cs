using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class CentralWeaponSystem : MonoBehaviour
{
    protected Weapon[] inventory;
    protected int[] ammo;
    protected int currSlot;
    protected const int maxWeaponsInInventory = 3;

    private HashSet<int> openInventorySlots;
    private Dictionary<Weapon, int> weaponSlot;

    protected Transform allBulletPools;
    protected Dictionary<Weapon, List<Transform>> bulletPools;
    protected int bulletPoolIdx = 0;

    protected Dictionary<Weapon, WeaponConfiguration> weaponConfiguration;
    protected Dictionary<Weapon, WeaponImplementation> weaponImplementations;
    private List<GameObject> physicalWeaponAndLimbs;

    protected CentralShooting shooting;
    protected CentralLookAround lookAround;
    protected Health health;

    protected virtual void Awake()
    {
        shooting = transform.GetComponent<CentralShooting>();
        lookAround = transform.GetComponent<CentralLookAround>();
        health = transform.GetComponent<Health>();

        bulletPools = new Dictionary<Weapon, List<Transform>>();
        weaponImplementations = new Dictionary<Weapon, WeaponImplementation>();
        weaponConfiguration = new Dictionary<Weapon, WeaponConfiguration>();

        inventory = new Weapon[maxWeaponsInInventory];
        ammo = new int[maxWeaponsInInventory];
        currSlot = 0;

        physicalWeaponAndLimbs = new List<GameObject>();
        allBulletPools = transform.parent.GetChild(1).transform.GetChild(0);

        foreach (Transform bulletPool in allBulletPools)
        {
            Weapon weapon = bulletPool.GetComponent<Weapon>();

            //make each weapon's ammo pool accesible by a dictionary
            List<Transform> tempBulletPool = new List<Transform>();
            foreach (Transform bullet in bulletPool)
                tempBulletPool.Add(bullet);
            bulletPools[weapon] = tempBulletPool;

            //make each weapon's stats configuration and shooting mechanics accessible by a dictionary
            WeaponConfiguration config = bulletPool.GetComponent<WeaponConfiguration>();
            weaponConfiguration[weapon] = config;

            WeaponImplementation implementation = bulletPool.GetComponent<WeaponImplementation>();
            implementation.Initialize(config);  
            weaponImplementations[weapon] = implementation;
        }

        WeaponStats weaponStats = new WeaponStats(this);
    }

    void Start() => Reset();

    // Uses a bullet. Lowers bullet count and returns a physical bullet gameobject
    public GameObject UseOneBullet()
    {
        ammo[currSlot]--;
        bulletPoolIdx = ammo[currSlot] % bulletPools[currentWeapon].Count;
        return bulletPools[currentWeapon][bulletPoolIdx].gameObject;
    }

    // Useful for quickly retrieving info about the current equipped weapon 
    public int CurrentAmmo => ammo[currSlot];
    public WeaponImplementation CurrentWeaponImplementation => weaponImplementations[currentWeapon];
    public WeaponConfiguration CurrentWeaponConfiguration => weaponConfiguration[currentWeapon];

    // Current weapon selected in inventory
    private Weapon currentWeapon => inventory[currSlot];

    // to be made private
    public virtual void Reset()
    {
        weaponSlot.Clear();
        openInventorySlots.Clear();
        currSlot = 0;

        for (int i = 0; i < maxWeaponsInInventory; i++)
        {
            inventory[i] = Weapon.None;
            ammo[i] = 0;
            openInventorySlots.Add(i);
        }

        pickupWeapon(Weapon.ArcticPistol);
        pickupWeapon(Weapon.LeafScythe);
    }

    protected virtual void switchWeapons(int slot)
    {
        if (slot < 0 || slot >= maxWeaponsInInventory)
            return;

        // if this weapon is already selected, no need to do anything
        if (currSlot == slot)
            return;

        currSlot = slot;
        weaponImplementations[currentWeapon].SetDefaultAnimation();
        weaponImplementations[currentWeapon].resetAttackProgress();

        lookAround.setAimTarget(CurrentWeaponConfiguration.weaponAimTracker);
        lookAround.calculateShoulderAngles(CurrentWeaponConfiguration.weaponIKCoordinates);

        // note: meelee weapons don't have bullet pools
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
            physicalWeaponAndLimbs.Add(limb);
            limb.SetActive(true);
        }
    }

    // pickup weapon into the current inventory slot
    protected virtual void pickupWeapon(Weapon weapon) 
    {
        // if we already have that weapon, just replenish ammo
        if (weaponSlot.TryGetValue(weapon, out int slot))
        {
            ammo[slot] = weaponConfiguration[weapon].startingAmmo;
            return;
        }

        // otherwise pickup weapon into the current inventory slot
        inventory[currSlot] = weapon;
        ammo[currSlot] = weaponConfiguration[weapon].startingAmmo;

        weaponSlot[weapon] = currSlot;
        openInventorySlots.Remove(currSlot);
    }

    // pickup weapon into any available slot. Returns whether or not
    // the weapon could be picked up
    protected virtual bool pickupWeaponIfInventoryNotFull(Weapon weapon)
    {
        // if we already have that weapon, just replenish ammo
        if (weaponSlot.TryGetValue(weapon, out int slot))
        {
            ammo[slot] = weaponConfiguration[weapon].startingAmmo;
            return true;
        }

        // otherwise put that weapon in any open slot
        foreach (int openSlot in openInventorySlots)
        {
            inventory[openSlot] = weapon;
            ammo[openSlot] = weaponConfiguration[weapon].startingAmmo;

            openInventorySlots.Remove(slot);
            weaponSlot[weapon] = slot;
            return true;
        }

        return false;
    }

    /*// Useful for weapons that shoot more than 1 bullet in a single burst
    public GameObject getLastBullet()
    {
        int totalBullets = bulletPools[currentWeapon].Count;
        return bulletPools[currentWeapon][(bulletPoolIdx + totalBullets - 1) % totalBullets].gameObject;
    }*/
}
