
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
    private String weaponTag;

    public void Awake()
    {
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
        base.Start();
        
        collectNewWeapon("Shielder");
        selectWeapon("Shielder");
    }

    // --------------------------------------------------------------------
    // Associate each weapon with a different number key on the keyboard
    // --------------------------------------------------------------------
    void Update()
    {
        for (int weaponCount = 1; weaponCount <= inventory.childCount; weaponCount++) {
            if (Input.GetKeyDown(weaponCount.ToString())) 
                selectWeapon(inventory.GetChild(weaponCount-1).tag);
        }
    }

    // --------------------------------------------------------------------
    // FOR PC VERSION: allow player to select a different weapon 
    // --------------------------------------------------------------------
    public override void selectWeapon(string weapon)
    {
        string lastWeapon = weaponSelected;
        base.selectWeapon(weapon);

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
    public override void collectNewWeapon(string weapon)
    {
        base.collectNewWeapon(weapon);
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

