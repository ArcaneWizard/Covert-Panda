
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
    private Dictionary<string, Text> ammoText = new Dictionary<string, Text>();

    public Transform equippedWeaponSprites;
    public Transform unequippedWeaponSprites;

    public Sprite slotSelected, slotNotSelected;
    private String weaponTag;

    public void Awake()
    {
        //add each weapon's icon image, ammo text and weapon slot border to a dictionary, accessible by weapon tag
        foreach (Transform weapon in inventory)
        {
            weaponSlot.Add(weapon.tag, weapon.GetChild(0).transform.GetComponent<Image>());
            ammoText.Add(weapon.tag, weapon.GetChild(1).transform.GetComponent<Text>());
            weaponIcon.Add(weapon.tag, weapon.GetChild(2).transform.GetComponent<Image>());
        }

        //add each weapon sprite (for equipped vs not equipped states) to a dictionary, accessible by weapon tag
        foreach (Transform weapon in equippedWeaponSprites)
            equipped.Add(weapon.tag, weapon.transform.GetComponent<SpriteRenderer>().sprite);
        foreach (Transform weapon in unequippedWeaponSprites)
            notEquipped.Add(weapon.tag, weapon.transform.GetComponent<SpriteRenderer>().sprite);
    }

    // Default weapon you start off with
    public override void Start()
    {
        base.Start();

        //DEBUGGING: START WITH ALL WEAPONS AVAILABLE TO BEGIN WITH
        foreach (String weapon in weapons.Keys)
            collectNewWeapon(weapon);

        selectWeapon("Shielder");
    }

    // Associate each weapon with a different number key on the keyboard
    void Update()
    {
        for (int weaponCount = 1; weaponCount <= inventory.childCount; weaponCount++)
        {
            if (Input.GetKeyDown(weaponCount.ToString()))
                selectWeapon(inventory.GetChild(weaponCount - 1).tag);
        }
    }

    // Allow player to select a different weapon 
    public override void selectWeapon(string weapon)
    {
        string lastWeapon = weaponSelected;
        base.selectWeapon(weapon);

        if (weaponSelected != lastWeapon)
        {
            //unselect previous weapon slot
            if (lastWeapon != "")
                weaponSlot[lastWeapon].sprite = slotNotSelected;

            //select new weapon slot
            weaponSlot[weaponSelected].sprite = slotSelected;
        }
    }

    // Player collects a weapon by physically touching it
    public override void collectNewWeapon(string weapon)
    {
        base.collectNewWeapon(weapon);
        ammoText[weapon].text = ammo[weapon].ToString();
        weaponIcon[weapon].sprite = equipped[weapon];
    }

    // Player uses up ammo of a certain weapon
    public override void useOneAmmo()
    {
        base.useOneAmmo();
        ammoText[weaponSelected].text = ammo[weaponSelected].ToString();

        if (ammo[weaponSelected] <= 0)
            weaponIcon[weaponSelected].sprite = notEquipped[weaponSelected];
    }
}

