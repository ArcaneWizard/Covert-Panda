
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
        EquipNewWeapon("Shielder");
        selectWeapon("Shielder", "gun");
        List<Vector2> aiming = getWeaponConfig().config.IK_Coordinates;
        lookAround.calculateShoulderAngles(aiming);
    }

    // --------------------------------------------------------------------
    // Associate each weapon with a different number key
    // --------------------------------------------------------------------
    void Update()
    {
        if (Input.GetKeyDown("1"))
            selectWeapon("Grenade", "handheld");
        if (Input.GetKeyDown("2"))
            selectWeapon("Shielder", "gun");
        if (Input.GetKeyDown("3"))
            selectWeapon("Boomerang", "handheld");
        if (Input.GetKeyDown("4"))
            selectWeapon("Plasma Orb", "handheld");
        if (Input.GetKeyDown("5"))
            selectWeapon("Scythe", "meelee");
        if (Input.GetKeyDown("6"))
            selectWeapon("Sniper", "gun");
        if (Input.GetKeyDown("7"))
            selectWeapon("Shotgun", "gun");
    }

    // --------------------------------------------------------------------
    // FOR PC VERSION: allow player to select a different weapon 
    // --------------------------------------------------------------------
    public override void selectWeapon(string weapon, string combatMode)
    {
        string lastWeapon = weaponSelected;
        base.selectWeapon(weapon, combatMode);

        if (weaponSelected != lastWeapon) {
            //unselect previous weapon slot
            if (lastWeapon != "")
                weaponSlot[lastWeapon].sprite = slotNotSelected;

            //select new weapon slot
            weaponSlot[weaponSelected].sprite = slotSelected;
        }
    }

    // --------------------------------------------------------------------
    // Player collects a weapon by physically touching it
    // --------------------------------------------------------------------
    public override void EquipNewWeapon(string weapon)
    {
        base.EquipNewWeapon(weapon);
        weaponIcon[weapon].sprite = equipped[weapon];
    }

    // --------------------------------------------------------------------
    // Player uses up ammo of a certain weapon
    // --------------------------------------------------------------------
    public override void useOneAmmo()
    {
        base.useOneAmmo();

        //if weapon is out of ammo, update its sprite
        if (ammo[weaponSelected] <= 0)
            weaponIcon[weaponSelected].sprite = notEquipped[weaponSelected];
    }
}

