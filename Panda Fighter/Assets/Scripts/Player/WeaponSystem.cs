
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponSystem : CentralWeaponSystem
{
    private HashSet<GameObject> pickableWeapons = new HashSet<GameObject>();
   /* private Dictionary<string, Image> weaponIcon = new Dictionary<string, Image>();
    private Dictionary<string, Image> weaponSlot = new Dictionary<string, Image>();
    private Dictionary<string, Sprite> equipped = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> notEquipped = new Dictionary<string, Sprite>();
    private Dictionary<string, Text> ammoText = new Dictionary<string, Text>();

    public Transform inventory;
    public Transform equippedWeaponSprites, unequippedWeaponSprites;
    public Sprite slotSelected, slotNotSelected;


    protected override void Awake()
    {
        base.Awake();

        /add each weapon's icon image, ammo text and weapon slot border to a dictionary, accessible by weapon tag
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
        */
    

    void Update()
    {
        if (health.IsDead)
            return;

        // use number keys to switch between weapons in your inventory
        for (int i = 1; i <= maxSlotsInInventory; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
                switchWeapons(i-1);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            foreach (GameObject pickableWeapon in pickableWeapons)
            {
                Weapon weapon = pickableWeapon.transform.GetComponent<WeaponTag>().Tag;
                pickupWeaponIntoCurrentSlot(weapon);
                pickableWeapon.SetActive(false);
                pickableWeapon.transform.parent.GetComponent<SpawnRandomWeapon>().startCountdownForNewWeapon();
                return;
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        base.OnTriggerEnter2D(col);

        if (!col.gameObject.activeSelf)
            return;

        if (col.gameObject.layer == Layer.Weapons)
            pickableWeapons.Add(col.gameObject);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == Layer.Weapons)
            pickableWeapons.Remove(col.gameObject);
    }


    /* protected virtual void switchWeapons(int inventoryIdx)
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
     protected override void pickupWeapon(Weapon weapon)
     {
         base.pickupWeapon(weapon);
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
     }*/
}

