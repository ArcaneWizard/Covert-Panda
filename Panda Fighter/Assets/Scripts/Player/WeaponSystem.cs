
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

    private Shooting shooting;

    void Awake()
    {
        //define components
        shooting = transform.GetComponent<Shooting>();

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
    }

    // --------------------------------------------------------------------
    // Default weapon you start with
    // --------------------------------------------------------------------
    void Start()
    {
        EquipNewWeapon("Pistol", 25);
        SelectWeapon("Pistol", "gun");
    }

    void Update()
    {
        if (Input.GetKeyDown("0"))
            SelectWeapon("Grenade", "hands");
        if (Input.GetKeyDown("1"))
            SelectWeapon("Boomerang", "gun");
        if (Input.GetKeyDown("2"))
            SelectWeapon("Pistol", "gun");
        if (Input.GetKeyDown("3"))
            SelectWeapon("Plasma Orb", "hands");
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
        bulletNumber = ++bulletNumber % physicalWeapons[weaponSelected].Count;
        shooting.combatMode = combatMode;

        if (combatMode == "hands")
            shooting.weaponHeld = getWeapon();
    }

    // --------------------------------------------------------------------
    // Player collects a weapon by physically touching it
    // --------------------------------------------------------------------
    public void EquipNewWeapon(string weapon, int bullets)
    {
        //update weapon sprite + ammo
        weapons[weapon].sprite = equipped[weapon];
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

        //if weapon is out of ammo, update its sprite
        if (weaponAmmo <= 0)
            weapons[weaponSelected].sprite = notEquipped[weaponSelected];
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
}

