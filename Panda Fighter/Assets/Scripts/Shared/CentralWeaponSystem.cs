using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CentralWeaponSystem : MonoBehaviour
{
    protected Dictionary<string, int> ammo = new Dictionary<string, int>();
    protected Dictionary<string, List<Transform>> bulletPools = new Dictionary<string, List<Transform>>();
    protected Dictionary<string, IWeapon> IWeapons = new Dictionary<string, IWeapon>();
    protected Dictionary<string, WeaponConfiguration> weaponConfigurations = new Dictionary<string, WeaponConfiguration>();
    protected Transform allBulletPools;

    public string weaponSelected { private set; get; }
    protected int bulletNumber = 0;

    protected CentralShooting shooting;
    protected CentralLookAround lookAround;
    private List<GameObject> Limbs_And_Weapons;

    public virtual void Awake()
    {
        shooting = transform.GetComponent<CentralShooting>();
        lookAround = transform.GetComponent<CentralLookAround>();

        Limbs_And_Weapons = new List<GameObject>();
        allBulletPools = transform.parent.GetChild(1).transform.GetChild(0);
        weaponSelected = "";

        foreach (Transform bulletPool in allBulletPools)
        {
            string tag = bulletPool.GetComponent<WeaponTag>().Tag;

            //add each weapon's pool of ammo to a dictionary
            List<Transform> tempBulletPool = new List<Transform>();
            foreach (Transform bullet in bulletPool)
            {
                tempBulletPool.Add(bullet);
            }
            bulletPools.Add(tag, tempBulletPool);

            //add each weapon's shooting instructions and weapon configuration to a dictionary
            IWeapon weapon = bulletPool.GetComponent<IWeapon>();
            IWeapons.Add(tag, weapon);
            WeaponConfiguration config = bulletPool.GetComponent<WeaponConfiguration>();
            weaponConfigurations.Add(tag, config);
            weapon.configuration = config;

            //add each weapon's ammo in a dictionary
            ammo.Add(tag, 0);
        }

        WeaponStats weaponStats = new WeaponStats(this);
    }

    public virtual void selectWeapon(string weapon)
    {
        //if the weapon is already selected, no need to do anything
        if (weapon == weaponSelected || ammo[weapon] <= 0)
            return;

        if (shooting.weaponHeld != null)
            shooting.weaponHeld.gameObject.SetActive(false);

        weaponSelected = weapon;
        IWeapons[weaponSelected].SetDefaultAnimation();
        IWeapons[weaponSelected].resetAttackProgress();
        shooting.updateCombatMode(weaponConfiguration.combatMode);
        lookAround.setAimTarget(weaponConfiguration.aimTarget);
        lookAround.calculateShoulderAngles(weaponConfiguration.IK_Coordinates);

        if (bulletPools.ContainsKey(weaponSelected))
            bulletNumber = ++bulletNumber % bulletPools[weaponSelected].Count;

        if (weaponConfiguration.combatMode == "handheld")
            shooting.updateWeaponHeldForHandheldWeapons();

        //deactivate the previous animated arm limbs + enable new ones
        foreach (GameObject limb_or_weapon in Limbs_And_Weapons)
            limb_or_weapon.SetActive(false);
        Limbs_And_Weapons.Clear();

        Limbs_And_Weapons.Add(weaponConfiguration.weapon);
        weaponConfiguration.weapon.SetActive(true);

        foreach (GameObject limb_or_weapon in weaponConfiguration.limbs)
        {
            Limbs_And_Weapons.Add(limb_or_weapon);
            limb_or_weapon.SetActive(true);
        }
    }

    public virtual void collectNewWeapon(string weapon) => ammo[weapon] = IWeapons[weapon].configuration.startingAmmo;

    public virtual void useOneAmmo()
    {
        ammo[weaponSelected] -= 1;
        bulletNumber = ++bulletNumber % bulletPools[weaponSelected].Count;
    }

    public GameObject GetBullet => bulletPools[weaponSelected][bulletNumber].gameObject;
    public int GetAmmo => ammo[weaponSelected];
    public IWeapon IWeapon => IWeapons[weaponSelected];
    public WeaponConfiguration weaponConfiguration => weaponConfigurations[weaponSelected];
    public WeaponConfiguration getWeaponConfiguration(String weapon) => weaponConfigurations[weapon];

    //useful for special attacks (right click)
    public GameObject getLastBullet()
    {
        int totalBullets = bulletPools[weaponSelected].Count;
        return bulletPools[weaponSelected][(bulletNumber + totalBullets - 1) % totalBullets].gameObject;
    }
}
