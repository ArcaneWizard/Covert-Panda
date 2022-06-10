
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

    public Transform inventory;
    public Transform equippedWeaponSprites, unequippedWeaponSprites;

    public Sprite slotSelected, slotNotSelected;
    private String tag;

    public override void Awake()
    {
        base.Awake();

        //add each weapon's icon image, ammo text and weapon slot border to a dictionary, accessible by weapon tag
        foreach (Transform weapon in inventory)
        {
            tag = weapon.GetComponent<WeaponTag>().Tag;
            weaponSlot.Add(tag, weapon.GetChild(0).transform.GetComponent<Image>());
            ammoText.Add(tag, weapon.GetChild(1).transform.GetComponent<Text>());
            weaponIcon.Add(tag, weapon.GetChild(2).transform.GetComponent<Image>());
        }

        //add each weapon sprite (for equipped vs not equipped states) to a dictionary, accessible by weapon tag
        foreach (Transform weapon in equippedWeaponSprites)
        {
            tag = weapon.GetComponent<WeaponTag>().Tag;
            equipped.Add(tag, weapon.transform.GetComponent<SpriteRenderer>().sprite);
        }

        foreach (Transform weapon in unequippedWeaponSprites)
        {
            tag = weapon.GetComponent<WeaponTag>().Tag;
            notEquipped.Add(tag, weapon.transform.GetComponent<SpriteRenderer>().sprite);
        }
    }

    // Default weapon you start off with
    public override void InitializeWeaponSystem()
    {
        base.InitializeWeaponSystem();

        foreach (KeyValuePair<string, IWeapon> weapon in IWeapons)  
            ammoText[weapon.Key].text = ammo[weapon.Key].ToString();

        collectNewWeapon(WeaponTags.ArcticPistol.ToString());
        collectNewWeapon(WeaponTags.Grenades.ToString());
        collectNewWeapon(WeaponTags.Railgun.ToString());
        collectNewWeapon(WeaponTags.LavaPistol.ToString());
        collectNewWeapon(WeaponTags.Needler.ToString());
        collectNewWeapon(WeaponTags.ArcticSprayer.ToString());
        collectNewWeapon(WeaponTags.Shotgun.ToString());
        selectWeapon(WeaponTags.Shotgun.ToString());
    }

    // Associate each weapon with a different number key on the keyboard
    void Update()
    {
        if (health.isDead)
            return;

        for (int weaponCount = 1; weaponCount <= 9; weaponCount++)
        {
            if (Input.GetKeyDown(weaponCount.ToString()))
                selectWeapon(inventory.GetChild(weaponCount - 1).GetComponent<WeaponTag>().Tag);
        }

        if (Input.GetKeyDown(KeyCode.Minus))
            selectWeapon(inventory.GetChild(9).GetComponent<WeaponTag>().Tag);

        else if (Input.GetKeyDown(KeyCode.Equals))
            selectWeapon(inventory.GetChild(10).GetComponent<WeaponTag>().Tag);

        else if (Input.GetKeyDown(KeyCode.LeftBracket))
            selectWeapon(inventory.GetChild(11).GetComponent<WeaponTag>().Tag);
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

