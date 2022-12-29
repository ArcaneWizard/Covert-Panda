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

    protected Dictionary<Weapon, WeaponConfiguration> weaponConfigurations;
    protected Dictionary<Weapon, WeaponBehaviour> weaponBehaviours;
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
        weaponBehaviours = new Dictionary<Weapon, WeaponBehaviour>();
        weaponConfigurations = new Dictionary<Weapon, WeaponConfiguration>();
        weaponSlot = new Dictionary<Weapon, int>();
        openInventorySlots = new HashSet<int>();

        inventory = new Weapon[maxWeaponsInInventory];
        ammo = new int[maxWeaponsInInventory];
        currSlot = 0;

        physicalWeaponAndLimbs = new List<GameObject>();
        allBulletPools = transform.parent.GetChild(1).transform.GetChild(0);

        foreach (Transform bulletPool in allBulletPools)
        {
            Weapon weapon = bulletPool.GetComponent<WeaponTag>().Tag;

            //make each weapon's ammo pool accesible by a dictionary
            List<Transform> tempBulletPool = new List<Transform>();
            foreach (Transform bullet in bulletPool)
                tempBulletPool.Add(bullet);
            bulletPools[weapon] = tempBulletPool;

            //make each weapon's stats configuration and shooting mechanics accessible by a dictionary
            WeaponConfiguration config = bulletPool.GetComponent<WeaponConfiguration>();
            weaponConfigurations[weapon] = config;

            WeaponBehaviour behavior = bulletPool.GetComponent<WeaponBehaviour>();
            behavior.Initialize(config, null, this);  
            weaponBehaviours[weapon] = behavior;
        }

        WeaponStats weaponStats = new WeaponStats(this);
        weaponStats.Initialize();

        Reset();
    }

    // Uses a bullet. Lowers bullet count and returns a physical bullet
    public Transform UseOneBullet()
    {
        ammo[currSlot]--;
        bulletPoolIdx = ammo[currSlot] % bulletPools[CurrentWeapon].Count;
        return bulletPools[CurrentWeapon][bulletPoolIdx];
    }

    // Useful for quickly retrieving info about the current equipped weapon
    public Weapon CurrentWeapon => inventory[currSlot];
    public int CurrentAmmo => ammo[currSlot];

    public WeaponBehaviour CurrentWeaponImplementation => weaponBehaviours[CurrentWeapon];
    public WeaponConfiguration CurrentWeaponConfiguration => weaponConfigurations[CurrentWeapon];
    public WeaponConfiguration GetConfiguration(Weapon weapon) => weaponConfigurations[weapon];

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
        useWeapon(currSlot);
    }

    // switch to the weapon in the specified inventory slot
    protected virtual void switchWeapons(int slot) 
    {
        // invalid inventory slot
        if (slot < 0 || slot >= maxWeaponsInInventory)
            return;

        // slot is already selected or no weapon exists at the specified slot
        if (currSlot == slot || inventory[slot] == Weapon.None)
            return;

        useWeapon(slot);
    }

    // pickup weapon into the current inventory slot
    protected virtual void pickupWeapon(Weapon weapon) 
    {
        // if we already have that weapon, just replenish ammo
        if (weaponSlot.TryGetValue(weapon, out int slot))
        {
            ammo[slot] = weaponConfigurations[weapon].StartingAmmo;
            return;
        }

        // otherwise pickup weapon into the current inventory slot
        inventory[currSlot] = weapon;
        ammo[currSlot] = weaponConfigurations[weapon].StartingAmmo;

        weaponSlot[weapon] = currSlot;
        openInventorySlots.Remove(currSlot);
    }

    // auto pickup weapon into any available inventory slot.
    // Returns false if no inventory slots were available
    protected virtual bool autoPickupWeapon(Weapon weapon)
    {
        // if we already have that weapon, just replenish ammo
        if (weaponSlot.TryGetValue(weapon, out int slot))
        {
            ammo[slot] = weaponConfigurations[weapon].StartingAmmo;
            return true;
        }

        // otherwise put that weapon in any open slot
        foreach (int openSlot in openInventorySlots)
        {
            inventory[openSlot] = weapon;
            ammo[openSlot] = weaponConfigurations[weapon].StartingAmmo;

            openInventorySlots.Remove(slot);
            weaponSlot[weapon] = slot;
            return true;
        }

        return false;
    }

    // use the weapon at the specified slot 
    private void useWeapon(int slot)
    {
        currSlot = slot;
        weaponBehaviours[CurrentWeapon].UponEquippingWeapon();
        weaponBehaviours[CurrentWeapon].ResetAttackProgress();

        lookAround.setAimTarget(CurrentWeaponConfiguration.WeaponAimTracker);
        lookAround.calculateShoulderAngles(CurrentWeaponConfiguration.WeaponIKCoordinates);

        // note: meelee weapons don't have bullet pools
        if (bulletPools.ContainsKey(CurrentWeapon))
            bulletPoolIdx = ++bulletPoolIdx % bulletPools[CurrentWeapon].Count;

        // deactivate the old weapon + arm limbs for that weapon 
        foreach (GameObject thing in physicalWeaponAndLimbs)
            thing.SetActive(false);

        physicalWeaponAndLimbs.Clear();

        // store the new weapon + arm limbs and activate them 
        physicalWeaponAndLimbs.Add(CurrentWeaponConfiguration.PhysicalWeapon);
        CurrentWeaponConfiguration.PhysicalWeapon.SetActive(true);

        foreach (GameObject limb in CurrentWeaponConfiguration.WeaponSpecificArms)
        {
            physicalWeaponAndLimbs.Add(limb);
            limb.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == Layer.Weapons)
        {
            Weapon weapon = col.transform.GetComponent<WeaponTag>().Tag;
            autoPickupWeapon(weapon);
            col.gameObject.SetActive(false);
            col.transform.parent.GetComponent<SpawnRandomWeapon>().startCountdownForNewWeapon();
        }
    }

    /*// Useful for weapons that shoot more than 1 bullet in a single burst
    public GameObject getLastBullet()
    {
        int totalBullets = bulletPools[currentWeapon].Count;
        return bulletPools[currentWeapon][(bulletPoolIdx + totalBullets - 1) % totalBullets].gameObject;
    }*/
}
