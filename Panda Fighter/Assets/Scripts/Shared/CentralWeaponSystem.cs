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
        if (weapon == weaponSelected || ammo[weapon] <= 0)
            return;

        if (shooting.weaponHeld != null)
            shooting.weaponHeld.gameObject.SetActive(false);

        weaponSelected = weapon;
        weapons[weaponSelected].SetDefaultAnimation();
        shooting.combatMode = weaponConfig.combatMode;
        shooting.configureWeaponAndArms();
        this.weapon.resetAttackProgress();

        if (weaponAmmoPools.ContainsKey(weaponSelected))
            bulletNumber = ++bulletNumber % weaponAmmoPools[weaponSelected].Count;

        if (weaponConfig.combatMode == "handheld")
            shooting.weaponHeld = getBullet;
    }

    public virtual void collectNewWeapon(string weapon) => ammo[weapon] = weapons[weapon].config.startingAmmo;

    public virtual void useOneAmmo()
    {
        ammo[weaponSelected] -= 1;
        bulletNumber = ++bulletNumber % weaponAmmoPools[weaponSelected].Count;
    }

    public GameObject getBullet => weaponAmmoPools[weaponSelected][bulletNumber].gameObject;
    public int getAmmo => ammo[weaponSelected];
    public IWeapon weapon => weapons[weaponSelected];
    public WeaponConfig weaponConfig => weapons[weaponSelected].config;

    //useful for special attacks (right click)
    public GameObject getLastBullet()
    {
        int totalBullets = weaponAmmoPools[weaponSelected].Count;
        return weaponAmmoPools[weaponSelected][(bulletNumber + totalBullets - 1) % totalBullets].gameObject;
    }
}
