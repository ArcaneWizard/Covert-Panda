
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

        foreach (KeyValuePair<string, WeaponMechanics> weapon in weaponBehaviours)  
            ammoText[weapon.Key].text = ammoCount[weapon.Key].ToString();

        collectNewWeapon(Weapon.ArcticPistol.ToString());
        collectNewWeapon(Weapon.Railgun.ToString());
        collectNewWeapon(Weapon.LavaPistol.ToString());
        collectNewWeapon(Weapon.Needler.ToString());
        collectNewWeapon(Weapon.Grenades.ToString());
        collectNewWeapon(Weapon.ArcticSprayer.ToString());
        collectNewWeapon(Weapon.Shotgun.ToString());
        selectWeapon(Weapon.Shotgun.ToString());
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
    }

    // Allow player to select a different weapon 
    protected override void selectWeapon(Weapon weapon)
    {
        string lastWeapon = currentWeapon;
        base.selectWeapon(weapon);

        if (currentWeapon != lastWeapon)
        {
            //unselect previous weapon slot
            if (lastWeapon != "")
                weaponSlot[lastWeapon].sprite = slotNotSelected;

            //select new weapon slot
            weaponSlot[currentWeapon].sprite = slotSelected;
        }
    }

    // Player collects a weapon by physically touching it
    protected override void collectNewWeapon(Weapon weapon)
    {
        base.collectNewWeapon(weapon);
        ammoText[weapon].text = ammoCount[weapon].ToString();
        weaponIcon[weapon].sprite = equipped[weapon];
    }

    // Player uses up ammo of a certain weapon
    protected override void useOneAmmo()
    {
        base.useOneAmmo();
        ammoText[currentWeapon].text = ammoCount[currentWeapon].ToString();

        if (ammoCount[currentWeapon] <= 0)
            weaponIcon[currentWeapon].sprite = notEquipped[currentWeapon];
    }
}

