using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class CentralGrenadeSystem : MonoBehaviour
{
    protected Grenade[] inventory;
    protected int currIdx;
    protected int[] grenadesLeft;
    protected const int maxGrenadesCarried = 2;

    private HashSet<int> openInventoryIndices;
    private Dictionary<Grenade, int> grenadesToIndices;

    protected Transform allGrenadePools;
    protected Dictionary<Grenade, List<Transform>> grenadePools;

    protected Dictionary<Grenade, WeaponConfiguration> grenadeConfigurations;
    protected Dictionary<Grenade, WeaponBehaviour> grenadeImplementations;

    protected CentralShooting shooting;
    protected CentralLookAround lookAround;
    protected Health health;

    protected virtual void Awake()
    {
        shooting = transform.GetComponent<CentralShooting>();
        lookAround = transform.GetComponent<CentralLookAround>();
        health = transform.GetComponent<Health>();

        grenadePools = new Dictionary<Grenade, List<Transform>>();
        grenadeImplementations = new Dictionary<Grenade, WeaponBehaviour>();
        grenadeConfigurations = new Dictionary<Grenade, WeaponConfiguration>();

        inventory = new Grenade[maxGrenadesCarried];
        grenadesLeft = new int[maxGrenadesCarried];
        currIdx = 0;

        allGrenadePools = transform.parent.GetChild(1).transform.GetChild(0);

        foreach (Transform grenadePool in allGrenadePools)
        {
            Grenade grenadeType = grenadePool.GetComponent<Grenade>();

            //make each grenade's pool accesible by a dictionary
            List<Transform> tempGrenadePool = new List<Transform>();
            foreach (Transform grenade in grenadePool)
                tempGrenadePool.Add(grenade);
            grenadePools[grenadeType] = tempGrenadePool;

            //make each grenade's stats configuration and deployment mechanics accessible by a dictionary
            WeaponConfiguration config = grenadePool.GetComponent<WeaponConfiguration>();
            grenadeConfigurations[grenadeType] = config;

            WeaponBehaviour implementation = grenadePool.GetComponent<WeaponBehaviour>();
            implementation.Initialize(config, this, null);
            grenadeImplementations[grenadeType] = implementation;
        }

        GrenadeStats grenadeStats = new GrenadeStats(this);
        //grenadeStats.Initialize();
    }

    private void Start() => Reset();

    public virtual void Reset()
    {
        grenadesToIndices.Clear();
        openInventoryIndices.Clear();
        currIdx = 0;

        for (int i = 0; i < maxGrenadesCarried; i++)
        {
            inventory[i] = Grenade.None;
            grenadesLeft[i] = 0;
            openInventoryIndices.Add(i);
        }  

        equipGrenade(Grenade.Frag);
    }

    // Useful for quickly retrieving info about the selected grenade
    private Grenade currentGrenade => inventory[currIdx];
    public int GrenadesLeft => grenadesLeft[currIdx];

    public WeaponBehaviour CurrentGrenadeImplementation => grenadeImplementations[currentGrenade];
    public WeaponConfiguration CurrentGrenadeConfiguration => grenadeConfigurations[currentGrenade];
    public WeaponConfiguration GetConfiguration(Grenade grenade) => grenadeConfigurations[grenade];

    /// <summary> Lowers grenade count by 1 and returns a physical grenade </summary>
    public Transform UseOneGrenade()
    {
        grenadesLeft[currIdx]--;
        int grenadePoolIdx = grenadesLeft[currIdx] % grenadePools[currentGrenade].Count;
        return grenadePools[currentGrenade][grenadePoolIdx];
    }

    protected virtual void switchGrenades(int idx)
    {
        if (idx < 0 || idx >= maxGrenadesCarried)
            return;

        // if this grenade is already selected, no need to do anything
        if (currIdx == idx)
            return;

        currIdx = idx;
    }

    // equips grenade in the current slot
    protected virtual void equipGrenade(Grenade grenade)
    {
        // if we already have that grenade, just replenish ammo
        if (grenadesToIndices.TryGetValue(grenade, out int newIdx)) 
        {
            grenadesLeft[newIdx] = grenadeConfigurations[grenade].StartingAmmo;
            return;
        }

        // otherwise pickup grenade into the current slot
        inventory[currIdx] = grenade;
        grenadesLeft[currIdx] = CurrentGrenadeConfiguration.StartingAmmo;

        grenadesToIndices[grenade] = currIdx;
        openInventoryIndices.Remove(currIdx);
    }

    // equips grenade in any available slot. Returns
    // whether or not the grenade could be equipped
    protected bool equipGrenadeIfSlotAvailable(Grenade grenade)
    {   
        // if we already have that grenade, just replenish ammo
        if (grenadesToIndices.TryGetValue(grenade, out int idx))
        {
            grenadesLeft[idx] = grenadeConfigurations[grenade].StartingAmmo;
            return true;
        }

        // otherwise put that grenade in any open slot
        foreach (int slot in openInventoryIndices) 
        {
            inventory[slot] = grenade;
            grenadesLeft[slot] = grenadeConfigurations[grenade].StartingAmmo;

            openInventoryIndices.Remove(slot);
            grenadesToIndices[grenade] = slot;
            return true;
        }

        return false;
    }
}
