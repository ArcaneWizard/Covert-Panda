using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CentralWeaponSystem : MonoBehaviour
{
    protected Dictionary<string, Text> ammo = new Dictionary<string, Text>();

    protected Dictionary<string, List<Transform>> weaponAmmoPools = new Dictionary<string, List<Transform>>();
    protected Dictionary<string, IWeapon> weapons = new Dictionary<string, IWeapon>();

    public Transform inventory;
    public Transform physicalWeapon;

    public string weaponSelected;
    protected int bulletNumber = 0;

    protected CentralShooting shooting;
    protected CentralLookAround lookAround;


    public virtual void Awake()
    {
        shooting = transform.GetComponent<CentralShooting>();
        lookAround = transform.GetComponent<CentralLookAround>();

        //add each weapon's image + ammo text to a dictionary, accessible by weapon tag
        foreach (Transform weapon in inventory)
            ammo.Add(weapon.tag, weapon.GetChild(1).transform.GetComponent<Text>());

        foreach (Transform weaponType in physicalWeapon)
        {
            //add each weapon's pool of ammo to a dictionary, accessible by weapon tag
            List<Transform> ammoPool = new List<Transform>();
            foreach (Transform ammo in weaponType)
                ammoPool.Add(ammo);
            weaponAmmoPools.Add(weaponType.tag, ammoPool);

            //add each weapon's shootingInstructions to a dictionary, accessible by weapon tag
            IWeapon weapon = weaponType.transform.GetComponent<IWeapon>();
            weapons.Add(weaponType.tag, weapon);
        }

    }

    public virtual void selectWeapon(string weapon, string combatMode)
    {
        //if the weapon is already selected, no need to do anything
        if (weapon == weaponSelected)
            return;

        //if a weapon/grenade is currently held by the player but not "thrown", hide it before selecting the new weapon
        if (weapon != weaponSelected && shooting.newWeaponHeld != null)
            shooting.newWeaponHeld.gameObject.SetActive(false);

        int weaponAmmo = Int32.Parse(ammo[weapon].text);

        //if the selected weapon has ammo, equip it
        if (weaponAmmo > 0)
            weaponSelected = weapon;
        else
            return;

        //use the next bullet in the bullet pool next time you fire
        if (combatMode != "meelee" || weaponAmmoPools.ContainsKey(weaponSelected))
            bulletNumber = ++bulletNumber % weaponAmmoPools[weaponSelected].Count;

        //switch combat mode for this specific weapon (update arm limb animations)
        shooting.combatMode = combatMode;
        shooting.configureWeaponAndArms();

        if (combatMode == "handheld")
            shooting.newWeaponHeld = getWeapon();

        weapons[weaponSelected].SetDefaultAnimation();
    }

    public virtual void EquipNewWeapon(string weapon, int bullets) => ammo[weapon].text = bullets.ToString();

    public virtual void useOneAmmo()
    {
        //decrease ammo by 1 and update ammo text
        int weaponAmmo = Int32.Parse(ammo[weaponSelected].text);
        weaponAmmo--;
        ammo[weaponSelected].text = weaponAmmo.ToString();

        //use diff gameobject bullet next time
        bulletNumber = ++bulletNumber % weaponAmmoPools[weaponSelected].Count;
    }

    public GameObject getWeapon() => weaponAmmoPools[weaponSelected][bulletNumber].gameObject;
    public int getAmmo() => Int32.Parse(ammo[weaponSelected].text);
    public IWeapon getWeaponConfig() => weapons[weaponSelected];

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
