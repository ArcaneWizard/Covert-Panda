
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponSystem : MonoBehaviour
{
    private Dictionary<string, Image> weaponIcon = new Dictionary<string, Image>();
    private Dictionary<string, Image> weaponSlot = new Dictionary<string, Image>();
    private Dictionary<string, Text> ammo = new Dictionary<string, Text>();
    private Dictionary<string, Sprite> equipped = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> notEquipped = new Dictionary<string, Sprite>();

    private Dictionary<string, List<Transform>> physicalWeapons = new Dictionary<string, List<Transform>>();
    public Dictionary<string, WeaponConfig> weaponConfigurations = new Dictionary<string, WeaponConfig>();

    public Transform inventory;
    public Transform physicalWeapon;
    public Transform equippedWeaponSprites;
    public Transform unequippedWeaponSprites;

    public Sprite slotSelected, slotNotSelected;

    public string weaponSelected;
    private int bulletNumber = 0;

    private Shooting shooting;
    private HoldingTheWeapon holdTheWeapon;
    private Sideview_Controller player_Controller;
    private IKTracking iKTracking;

    void Awake()
    {
        //define components
        shooting = transform.GetComponent<Shooting>();
        holdTheWeapon = transform.GetComponent<HoldingTheWeapon>();
        iKTracking = transform.GetComponent<IKTracking>();
        player_Controller = transform.GetComponent<Sideview_Controller>();

        //add each weapon's image + ammo text to a dictionary, accessible by weapon tag
        foreach (Transform weapon in inventory)
        {
            weaponSlot.Add(weapon.tag, weapon.GetChild(0).transform.GetComponent<Image>());
            ammo.Add(weapon.tag, weapon.GetChild(1).transform.GetComponent<Text>());
            weaponIcon.Add(weapon.tag, weapon.GetChild(2).transform.GetComponent<Image>());
        }

        //add each weapon sprite (equipped vs not equipped) to a dictionary, accessible by weapon tag
        foreach (Transform weapon in equippedWeaponSprites)
            equipped.Add(weapon.tag, weapon.transform.GetComponent<SpriteRenderer>().sprite);
        foreach (Transform weapon in unequippedWeaponSprites)
            notEquipped.Add(weapon.tag, weapon.transform.GetComponent<SpriteRenderer>().sprite);

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
        EquipNewWeapon("Shielder", 25);
        SelectWeapon("Shielder", "gun");
        List<Vector2> aiming = iKTracking.setIKCoordinates("Shielder");
        player_Controller.calculateShoulderAngles(aiming);
    }

    // --------------------------------------------------------------------
    // Associate each weapon with a different number key
    // --------------------------------------------------------------------
    void Update()
    {
        //Note for the combat mode
        //If you're literally holding the "bullet" you have  to throw, like with a grenade, use the string "handheld"
        //If it's a gun that has ammo and shoots bullets, use the string "gun"
        //If it's a meelee based weapon with "infinite ammo" while hitting bots with it, use the string "meelee"

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
    public void EquipNewWeapon(string weapon, int bullets)
    {
        //update weapon sprite + ammo
        weaponIcon[weapon].sprite = equipped[weapon];
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
            weaponIcon[weaponSelected].sprite = notEquipped[weaponSelected];
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

