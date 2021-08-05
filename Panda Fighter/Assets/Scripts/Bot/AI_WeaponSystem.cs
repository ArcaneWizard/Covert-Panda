
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AI_WeaponSystem : MonoBehaviour
{
    private Dictionary<string, Text> ammo = new Dictionary<string, Text>();

    private Dictionary<string, List<Transform>> physicalWeapons = new Dictionary<string, List<Transform>>();
    public Dictionary<string, WeaponConfig> weaponConfigurations = new Dictionary<string, WeaponConfig>();

    public Transform inventory;
    public Transform physicalWeapon;

    public string weaponSelected;
    private int bulletNumber = 0;

    private AI_Shooting shooting;
    private AI_HoldingTheWeapon holdTheWeapon;
    private AI_Controller alien_Controller;
    private IKTracking iKTracking;

    void Awake()
    {
        //define components
        shooting = transform.GetComponent<AI_Shooting>();
        holdTheWeapon = transform.GetComponent<AI_HoldingTheWeapon>();
        iKTracking = transform.GetComponent<IKTracking>();
        alien_Controller = transform.GetComponent<AI_Controller>();

        //add each weapon's image + ammo text to a dictionary, accessible by weapon tag
        foreach (Transform weapon in inventory)
            ammo.Add(weapon.tag, weapon.GetChild(1).transform.GetComponent<Text>());

        //add 1) each weapon's weapon/limb/aiming configuration to a dictionary, accessible by weapon tag
        //add 2) each weapon's physical ammo to a dictionary, accessible by weapon tag
        foreach (Transform weaponType in physicalWeapon)
        {
            List<Transform> physicalAmmo = new List<Transform>();

            foreach (Transform ammo in weaponType)
                physicalAmmo.Add(ammo);

            physicalWeapons.Add(weaponType.tag, physicalAmmo);
            weaponConfigurations.Add(weaponType.tag, weaponType.transform.GetComponent<WeaponConfig>());
        }
    }

    // --------------------------------------------------------------------
    // Default weapon you start with
    // --------------------------------------------------------------------
    void Start()
    {
        EquipNewWeapon("Scythe", 1);
        SelectWeapon("Scythe", "meelee");
        List<Vector2> aiming = iKTracking.setIKCoordinates("Scythe");
        alien_Controller.calculateShoulderAngles(aiming);
    }

    // --------------------------------------------------------------------
    // FOR MOBILE VERSION: allow player to tap to select a different weapon 
    // --------------------------------------------------------------------
    public void SelectWeapon(string combatMode)
    {
        string weapon = (EventSystem.current.currentSelectedGameObject) ? EventSystem.current.currentSelectedGameObject.transform.tag : "Pistol";
        int weaponAmmo = Int32.Parse(ammo[weapon].text);

        //if that weapon has ammo, equip it
        if (weaponAmmo > 0)
            weaponSelected = weapon;
        else
            return;

        //update which bullet in the bullet list to use
        bulletNumber = ++bulletNumber % physicalWeapons[weaponSelected].Count;
        shooting.combatMode = combatMode;
    }

    // --------------------------------------------------------------------
    // FOR PC VERSION: allow player to select a different weapon 
    // --------------------------------------------------------------------
    public void SelectWeapon(string weapon, string combatMode)
    {
        //if the weapon is already selected, no need to do anything
        if (weapon == weaponSelected)
            return;

        //if a weapon/grenade is currently held by the player but not "thrown", hide it before selecting the new weapon
        if (weapon != weaponSelected && shooting.weaponHeld)
            shooting.weaponHeld.gameObject.SetActive(false);

        int weaponAmmo = Int32.Parse(ammo[weapon].text);

        //if the selected weapon has ammo, equip it
        if (weaponAmmo > 0)
            weaponSelected = weapon;
        else
            return;

        //use the next bullet in the bullet pool next time you fire
        if (combatMode != "meelee" || physicalWeapons.ContainsKey(weaponSelected))
            bulletNumber = ++bulletNumber % physicalWeapons[weaponSelected].Count;

        //switch combat mode for this specific weapon (update arm limb animations)
        shooting.combatMode = combatMode;
        holdTheWeapon.configureWeaponAndArms();

        if (combatMode == "handheld")
            shooting.weaponHeld = getWeapon();
    }

    // --------------------------------------------------------------------
    // Player collects a weapon by physically touching it
    // --------------------------------------------------------------------
    public void EquipNewWeapon(string weapon, int bullets)
    {
        //update weapon sprite + ammo
        ammo[weapon].text = bullets.ToString();
    }

    // --------------------------------------------------------------------
    // Player uses up ammo of a certain weapon
    // --------------------------------------------------------------------
    public void useOneAmmo()
    {
        //decrease ammo by 1 and update ammo text
        int weaponAmmo = Int32.Parse(ammo[weaponSelected].text);
        weaponAmmo--;
        ammo[weaponSelected].text = weaponAmmo.ToString();

        //use diff gameobject bullet next time
        bulletNumber = ++bulletNumber % physicalWeapons[weaponSelected].Count;
    }

    // --------------------------------------------------------------------
    // Get the weapon bullet from the list in the dictionary + return different bullet next time
    // --------------------------------------------------------------------
    public GameObject getWeapon()
    {
        Transform theWeapon = physicalWeapons[weaponSelected][bulletNumber];
        return theWeapon.gameObject;
    }

    // --------------------------------------------------------------------
    // Get the weapon's ammo
    // --------------------------------------------------------------------
    public int getAmmo()
    {
        return Int32.Parse(ammo[weaponSelected].text);
    }

    // --------------------------------------------------------------------
    //Player collides with weapon, so equip it
    // --------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Weapon Pickup"))
        {
            EquipNewWeapon(col.gameObject.tag, 25);
            col.gameObject.SetActive(false);
        }
    }
}

