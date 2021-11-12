using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CentralWeaponSystem : MonoBehaviour
{
    protected Dictionary<string, int> ammo = new Dictionary<string, int>();
    protected Dictionary<string, List<Transform>> weaponAmmoPools = new Dictionary<string, List<Transform>>();
    protected Dictionary<string, IWeapon> weapons = new Dictionary<string, IWeapon>();

    protected Transform physicalWeapon;

    public string weaponSelected;
    protected int bulletNumber = 0;

    protected CentralShooting shooting;
    protected CentralLookAround lookAround;

    public virtual void Awake()
    {
        shooting = transform.GetComponent<CentralShooting>();
        lookAround = transform.GetComponent<CentralLookAround>();
        physicalWeapon = transform.parent.GetChild(1).transform.GetChild(0);
    }

    public virtual void Start()
    {
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

            //define each weapon's ammo in a dictionary, accessible by weapon tag
            ammo.Add(weaponType.tag, 0);
        }
    }

    public virtual void selectWeapon(string weapon)
    {
        //if the weapon is already selected, no need to do anything
        if (weapon == weaponSelected)
            return;

        //if a weapon/grenade is currently held by the player but not "thrown", hide it before selecting the new weapon
        if (weapon != weaponSelected && shooting.weaponHeld != null)
            shooting.weaponHeld.gameObject.SetActive(false);

        //if the selected weapon has ammo, equip it
        if (ammo[weapon] > 0)
            weaponSelected = weapon;
        else
            return;

        //use the next bullet in the bullet pool next time you fire
        string combatMode = weapons[weapon].config.combatMode;
        if (combatMode != "meelee" || weaponAmmoPools.ContainsKey(weaponSelected))
            bulletNumber = ++bulletNumber % weaponAmmoPools[weaponSelected].Count;

        //switch combat mode for this specific weapon (update arm limb animations)
        shooting.combatMode = combatMode;
        shooting.configureWeaponAndArms();

        if (combatMode == "handheld")
            shooting.weaponHeld = getWeapon();

        weapons[weaponSelected].SetDefaultAnimation();
    }

    public virtual void collectNewWeapon(string weapon) => ammo[weapon] = weapons[weapon].config.startingAmmo;

    public virtual void useOneAmmo()
    {
        ammo[weaponSelected] -= 1;
        bulletNumber = ++bulletNumber % weaponAmmoPools[weaponSelected].Count;
    }

    public GameObject getWeapon() => weaponAmmoPools[weaponSelected][bulletNumber].gameObject;
    public int getAmmo() => ammo[weaponSelected];
    public IWeapon getWeaponConfig() => weapons[weaponSelected];

    //useful for special attacks (right click)
    public GameObject getLastWeapon()
    {
        int totalBullets = weaponAmmoPools[weaponSelected].Count;
        return weaponAmmoPools[weaponSelected][(bulletNumber + totalBullets - 1) % totalBullets].gameObject;
    }
}
