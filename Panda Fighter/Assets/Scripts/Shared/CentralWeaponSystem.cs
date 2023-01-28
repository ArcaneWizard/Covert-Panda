using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// This class manages the weapon inventory system of the creature. The
// inventory has multiple slots, each of which holds a weapon and tracks ammo. 
// There are also useful methods related to the weapon the creature is currently holding,
// such as accessing it's weapon configuration or retrieving a physical bullet.

public abstract class CentralWeaponSystem : MonoBehaviour
{
    private struct InventorySlot
    {
        public Weapon Weapon;
        public int Ammo;
    }

    private InventorySlot[] inventory;
    private int selectedSlot; // the current inventory slot selected 
    protected const int maxSlotsInInventory = 3; 
    private HashSet<int> openInventorySlots; // inventory slots that don't have a weapon yet
    protected Dictionary<Weapon, int> inventoryWeapons; // maps equipped weapons to their inventory slot

    private Transform allBulletPools;
    private Dictionary<Weapon, List<Transform>> bulletPools;
    private int bulletPoolIdx = 0;

    private Dictionary<Weapon, WeaponConfiguration> weaponConfigurations;
    private Dictionary<Weapon, WeaponBehaviour> weaponBehaviours;

    private List<GameObject> physicalWeaponAndLimbs;
    private CentralLookAround lookAround;
    protected Health health;

    protected virtual void Awake()
    {
        lookAround = transform.GetComponent<CentralLookAround>();
        health = transform.GetComponent<Health>();

        bulletPools = new Dictionary<Weapon, List<Transform>>();
        weaponBehaviours = new Dictionary<Weapon, WeaponBehaviour>();
        weaponConfigurations = new Dictionary<Weapon, WeaponConfiguration>();
        inventoryWeapons = new Dictionary<Weapon, int>();
        openInventorySlots = new HashSet<int>();

        inventory = new InventorySlot[maxSlotsInInventory];
        selectedSlot = 0;

        physicalWeaponAndLimbs = new List<GameObject>();
        allBulletPools = transform.parent.GetChild(1).transform.GetChild(0);

        foreach (Transform bulletPool in allBulletPools)
        {
            Weapon weapon = bulletPool.GetComponent<WeaponTag>().Tag;

            // make each weapon's ammo pool accesible by a dictionary
            List<Transform> tempBulletPool = new List<Transform>();
            foreach (Transform bullet in bulletPool)
                tempBulletPool.Add(bullet);
            bulletPools[weapon] = tempBulletPool;

            WeaponConfiguration config = bulletPool.GetComponent<WeaponConfiguration>();
            weaponConfigurations[weapon] = config;

            WeaponBehaviour behavior = bulletPool.GetComponent<WeaponBehaviour>();
            behavior.Initialize(config, null, this);  
            weaponBehaviours[weapon] = behavior;
        }

        WeaponStats weaponStats = new WeaponStats(this);
        weaponStats.Initialize();
    }

    protected virtual void Start()
    {
        ResetInventory();
    }

     // Info about the current weapon the creature is holding
    public Weapon CurrentWeapon => inventory[selectedSlot].Weapon;
    public int CurrentAmmo => inventory[selectedSlot].Ammo;
    public WeaponBehaviour CurrentWeaponBehaviour => weaponBehaviours[CurrentWeapon];
    public WeaponConfiguration CurrentWeaponConfiguration => weaponConfigurations[CurrentWeapon];
    public WeaponConfiguration GetConfiguration(Weapon weapon) => weaponConfigurations[weapon];


    // Lowers current ammo by 1. Returns a physical bullet for the current weapon
    public Transform UseOneBullet()
    {
        inventory[selectedSlot].Ammo--;
        bulletPoolIdx = inventory[selectedSlot].Ammo % bulletPools[CurrentWeapon].Count;
        return bulletPools[CurrentWeapon][bulletPoolIdx];
    }

    // Reset inventory
    public virtual void ResetInventory()
    {
        inventoryWeapons.Clear();
        openInventorySlots.Clear();
        selectedSlot = 0;

        for (int i = 0; i < maxSlotsInInventory; i++)
        {
            inventory[i].Weapon = Weapon.None;
            inventory[i].Ammo = 0;
            openInventorySlots.Add(i);
        }

        pickupWeaponIntoAvailableSlot(Weapon.ArcticPistol);
        pickupWeaponIntoAvailableSlot(Weapon.RocketLauncher);
    }

    // Switch to the weapon in the specified inventory slot
    protected virtual void switchWeapons(int slot) 
    {
        // invalid inventory slot
        if (slot < 0 || slot >= maxSlotsInInventory)
            return;

        // no weapon exists at the specified slot OR slot is already selected
        if (inventory[slot].Weapon == Weapon.None || slot == selectedSlot)
            return;

        switchToNewWeapon(slot);
    }

    // pickup specified weapon into the current inventory slot
    protected virtual void pickupWeaponIntoCurrentSlot(Weapon weapon) 
    {
        // if we already have that weapon, just replenish ammo
        if (inventoryWeapons.TryGetValue(weapon, out int slot))
        {
            inventory[slot].Ammo = weaponConfigurations[weapon].StartingAmmo;
            return;
        }

        // otherwise pickup weapon into the current inventory slot
        inventoryWeapons.Remove(CurrentWeapon);
        inventory[selectedSlot].Weapon = weapon;
        inventory[selectedSlot].Ammo = weaponConfigurations[weapon].StartingAmmo;
        inventoryWeapons[weapon] = selectedSlot;

        openInventorySlots.Remove(selectedSlot);
        switchToNewWeapon(selectedSlot);
    }

    // pickup specified weapon into any available inventory slot.
    // returns whether or not the weapon could be picked up.
    protected virtual bool pickupWeaponIntoAvailableSlot(Weapon weapon)
    {
        // if we already have that weapon, just replenish ammo
        if (inventoryWeapons.TryGetValue(weapon, out int slot))
        {
            inventory[slot].Ammo = weaponConfigurations[weapon].StartingAmmo;
            return true;
        }

        // otherwise put that weapon into any open slot
        foreach (int openSlot in openInventorySlots)
        {
            inventory[openSlot].Weapon = weapon;
            inventory[openSlot].Ammo = weaponConfigurations[weapon].StartingAmmo;

            openInventorySlots.Remove(openSlot);
            inventoryWeapons[weapon] = openSlot;

            if (openSlot == selectedSlot)
                switchToNewWeapon(openSlot);

            return true;
        }

        return false;
    }

    // switches to the new weapon at the specified slot, which becomes the selected inventory slot 
    private void switchToNewWeapon(int slot)
    {
        selectedSlot = slot;
        weaponBehaviours[CurrentWeapon].UponEquippingWeapon();
        weaponBehaviours[CurrentWeapon].ResetAttackProgress();

        lookAround.UpdateArmInverseKinematics();

        // note that not all weapons have bullet pools (ex. meelee weapons)
        if (bulletPools.ContainsKey(CurrentWeapon))
            bulletPoolIdx = ++bulletPoolIdx % bulletPools[CurrentWeapon].Count;

        // deactivate the old weapon + arm limbs for that weapon 
        foreach (GameObject thing in physicalWeaponAndLimbs)
            thing.SetActive(false);
        physicalWeaponAndLimbs.Clear();

        // store the new weapon + arm limbs and activate them 
        physicalWeaponAndLimbs.Add(CurrentWeaponConfiguration.PhysicalWeapon);
        CurrentWeaponConfiguration.PhysicalWeapon.SetActive(true);

        foreach (GameObject limb in CurrentWeaponConfiguration.Arms)
        {
            physicalWeaponAndLimbs.Add(limb);
            limb.SetActive(true);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == Layer.Weapons && col.gameObject.activeSelf)
        {
            Weapon weapon = col.transform.GetComponent<WeaponTag>().Tag;
            bool pickedUp = pickupWeaponIntoAvailableSlot(weapon);

            if (pickedUp) 
            {
                col.gameObject.SetActive(false);
                col.transform.parent.GetComponent<SpawnRandomWeapon>().startCountdownForNewWeapon();
            }
        }
    }

    /*// Useful for weapons that shoot more than 1 bullet in a single burst
    public GameObject getLastBullet()
    {
        int totalBullets = bulletPools[currentWeapon].Count;
        return bulletPools[currentWeapon][(bulletPoolIdx + totalBullets - 1) % totalBullets].gameObject;
    }*/
}
