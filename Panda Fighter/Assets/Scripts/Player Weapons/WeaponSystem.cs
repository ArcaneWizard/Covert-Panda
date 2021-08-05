
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponSystem : CentralWeaponSystem
{
    private Dictionary<string, Image> weaponIcon = new Dictionary<string, Image>();
    private Dictionary<string, Image> weaponSlot = new Dictionary<string, Image>();
    private Dictionary<string, Sprite> equipped = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> notEquipped = new Dictionary<string, Sprite>();

    public Transform equippedWeaponSprites;
    public Transform unequippedWeaponSprites;

    public Sprite slotSelected, slotNotSelected;


    public override void Awake()
    {
        //initial setup
        base.Awake();

        //add each weapon's icon image + weapon slot border to a dictionary, accessible by weapon tag
        foreach (Transform weapon in inventory)
        {
            weaponSlot.Add(weapon.tag, weapon.GetChild(0).transform.GetComponent<Image>());
            weaponIcon.Add(weapon.tag, weapon.GetChild(2).transform.GetComponent<Image>());
        }

        //add each weapon sprite (for equipped vs not equipped states) to a dictionary, accessible by weapon tag
        foreach (Transform weapon in equippedWeaponSprites)
            equipped.Add(weapon.tag, weapon.transform.GetComponent<SpriteRenderer>().sprite);
        foreach (Transform weapon in unequippedWeaponSprites)
            notEquipped.Add(weapon.tag, weapon.transform.GetComponent<SpriteRenderer>().sprite);
    }

    // --------------------------------------------------------------------
    // Default weapon you start off with
    // --------------------------------------------------------------------
    void Start()
    {
        EquipNewWeapon("Shielder", 25);
        SelectWeapon("Shielder", "gun");
        List<Vector2> aiming = iKTracking.setIKCoordinates("Shielder");
        controller.calculateShoulderAngles(aiming);
    }

    // --------------------------------------------------------------------
    // Associate each weapon with a different number key
    // --------------------------------------------------------------------
    void Update()
    {
        if (Input.GetKeyDown("1"))
            SelectWeapon("Grenade", "handheld");
        if (Input.GetKeyDown("2"))
            SelectWeapon("Shielder", "gun");
        if (Input.GetKeyDown("3"))
            SelectWeapon("Boomerang", "handheld");
        if (Input.GetKeyDown("4"))
            SelectWeapon("Plasma Orb", "handheld");
        if (Input.GetKeyDown("5"))
            SelectWeapon("Scythe", "meelee");
        if (Input.GetKeyDown("6"))
            SelectWeapon("Sniper", "gun");
        if (Input.GetKeyDown("7"))
            SelectWeapon("Shotgun", "gun");
    }

    // --------------------------------------------------------------------
    // FOR PC VERSION: allow player to select a different weapon 
    // --------------------------------------------------------------------
    public override void SelectWeapon(string weapon, string combatMode)
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
        {
            //unselect previous weapon slot
            if (weaponSelected != "")
                weaponSlot[weaponSelected].sprite = slotNotSelected;

            //select new weapon + new weapon slot
            weaponSelected = weapon;
            weaponSlot[weaponSelected].sprite = slotSelected;
        }
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
    public override void EquipNewWeapon(string weapon, int bullets)
    {
        base.EquipNewWeapon(weapon, bullets);
        weaponIcon[weapon].sprite = equipped[weapon];
    }

    // --------------------------------------------------------------------
    // Player uses up ammo of a certain weapon
    // --------------------------------------------------------------------
    public override void useOneAmmo()
    {
        //decrease ammo by 1 and update ammo text
        int weaponAmmo = Int32.Parse(ammo[weaponSelected].text);
        weaponAmmo--;
        ammo[weaponSelected].text = weaponAmmo.ToString();

        //use diff gameobject bullet next time
        bulletNumber = ++bulletNumber % physicalWeapons[weaponSelected].Count;

        //if weapon is out of ammo, update its sprite
        if (weaponAmmo <= 0)
            weaponIcon[weaponSelected].sprite = notEquipped[weaponSelected];
    }
}

