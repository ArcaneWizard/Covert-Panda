using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Use this if transforming weapons stats from code to scriptable objects
*/

[CreateAssetMenu(fileName = "New Weapon", menuName = "WeaponStats")]
public class NewWeaponStat : ScriptableObject
{
    public float fireRateInfo;
    public Mode combatMode;
    public Type weaponType;
    public float weaponRange;
    public int bulletSpeed;
    public int startingAmmo;
    public int bulletDmg;
    public int explosionDmg;
    public List<GameObject> limbs;
    public GameObject weapon;
}