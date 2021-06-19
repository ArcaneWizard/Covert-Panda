using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponSystem : MonoBehaviour
{
    private Dictionary<string, Image> weapons = new Dictionary<string, Image>();
    private Dictionary<string, Text> ammo = new Dictionary<string, Text>();
    private Dictionary<string, Sprite> equipped = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> notEquipped = new Dictionary<string, Sprite>();

    private Dictionary<string, List<Transform>> physicalWeapons = new Dictionary<string, List<Transform>>();

    public Transform inventory;
    public Transform weaponSprites;
    public Transform physicalWeapon;

    [HideInInspector]
    public string weaponSelected;
    private int bulletNumber = 0;

    void Awake()
    {
        //add each weapon's image + ammo text to a dictionary, accessible by weapon tag
        foreach (Transform weapon in inventory)
        {
            weapons.Add(weapon.tag, weapon.transform.GetComponent<Image>());
            ammo.Add(weapon.tag, weapon.GetChild(0).transform.GetComponent<Text>());
        }

        //add each weapon sprite (equipped vs not equipped) to a dictionary, accessible by weapon tag
        foreach (Transform weapon in weaponSprites)
        {
            if (weapon.name == "Equipped")
                equipped.Add(weapon.tag, weapon.transform.GetComponent<SpriteRenderer>().sprite);
            else
                notEquipped.Add(weapon.tag, weapon.transform.GetComponent<SpriteRenderer>().sprite);
        }

        //add each weapon's physical ammo to a dictionary, accessible by weapon tag
        foreach (Transform weaponType in physicalWeapon)
        {
            List<Transform> physicalAmmo = new List<Transform>();
            foreach (Transform ammo in weaponType)
                physicalAmmo.Add(ammo);
            physicalWeapons.Add(weaponType.tag, physicalAmmo); ;
        }

        //start with grenade equipped
        weaponSelected = "Pistol";
        EquipNewWeapon("Pistol");
    }

    //allow player to select a different weapon 
    public void SelectWeapon()
    {
        string weapon = EventSystem.current.currentSelectedGameObject.transform.tag;
        int weaponAmmo = Int32.Parse(ammo[weapon].text);

        //if that weapon has ammo, equip it
        if (weaponAmmo > 0)
            weaponSelected = weapon;
        else
            return;

        //update which bullet in the bullet list to use
        bulletNumber = ++bulletNumber % physicalWeapons[weaponSelected].Count;
    }

    //player collects a weapon by physically touching it
    public void EquipNewWeapon(string weapon)
    {
        //update weapon sprite + ammo
        weapons[weapon].sprite = equipped[weapon];
        ammo[weapon].text = "40";
    }

    //player uses up ammo of a certain weapon
    public void usedOneAmmo()
    {
        //decrease ammo by 1 and update ammo text
        int weaponAmmo = Int32.Parse(ammo[weaponSelected].text);
        weaponAmmo--;
        ammo[weaponSelected].text = weaponAmmo.ToString();

        //use diff gameobject bullet next time
        bulletNumber = ++bulletNumber % physicalWeapons[weaponSelected].Count;

        //if weapon is out of ammo, update its sprite
        if (weaponAmmo <= 0)
            weapons[weaponSelected].sprite = notEquipped[weaponSelected];
    }

    //get the weapon bullet from the list in the dictionary + return different bullet next time
    public GameObject getWeapon()
    {
        Transform theWeapon = physicalWeapons[weaponSelected][bulletNumber];
        return theWeapon.gameObject;
    }

    //get the weapon's ammo
    public int getAmmo()
    {
        return Int32.Parse(ammo[weaponSelected].text);
    }
}

