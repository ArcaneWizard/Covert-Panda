using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class houses the actual stats of the weapon (ex. bullet speed).
// It also houses physical components of the weapon relevant to using it correctly.

public class WeaponConfiguration : MonoBehaviour
{
    public string CombatMode { get; private set; }
    public string WeaponType { get; private set; }
    public int BulletDmg { get; private set; }
    public int ExplosionDmg { get; private set; }
    public int StartingAmmo { get; private set; }
    public int BulletSpeed { get; private set; }
    public float WeaponRange { get; private set; }
    public float FireRateInfo { get; private set; }  // max attacks or fired shots per second

    public Transform BulletSpawnPoint { get; private set; }
    public List<GameObject> WeaponSpecificArms { get; private set; }
    public GameObject PhysicalWeapon { get; private set; }
    public Transform WeaponPivot { get; private set; }
    public Animator Animator { get; private set; }

    // Required for configuring a weapon's aim to be very precise when using a mouse
    public List<Vector2> WeaponIKCoordinates { get; private set; } 
    public Transform WeaponAimTracker { get; private set; }  

    public void Initialize(float fireRateInfo, string combatMode, string weaponType, float weaponRange, int bulletSpeed, int startingAmmo,
        int bulletDmg, int explosionDmg, List<GameObject> arms, GameObject weapon)
    {
        this.FireRateInfo = fireRateInfo;
        this.CombatMode = combatMode;
        this.WeaponType = weaponType;
        this.WeaponRange = weaponRange;
        this.BulletSpeed = bulletSpeed;
        this.StartingAmmo = startingAmmo;
        this.BulletDmg = bulletDmg;
        this.ExplosionDmg = explosionDmg;
        this.WeaponSpecificArms = arms;
        this.PhysicalWeapon = weapon;

        setup();
    }

    private void setup()
    {
        Transform creature = transform.parent.parent.parent.transform.GetChild(0);
        Animator = creature.GetComponent<Animator>();

        ArmsHandler armsHandler = creature.GetChild(0).GetChild(0).GetComponent<ArmsHandler>();
        WeaponAimTracker = armsHandler.GetIK_WeaponAimTracker(WeaponSpecificArms);
        WeaponIKCoordinates = armsHandler.GetIK_WeaponCoordinates(WeaponSpecificArms);

        // if weapon pivot point exists for this weapon, store it
        WeaponPivot = WeaponAimTracker ? WeaponAimTracker.parent.GetChild(1) : null;

        // if bullet spawn point exists for this weapon, store it
        BulletSpawnPoint = (PhysicalWeapon.transform.childCount > 0) ? PhysicalWeapon.transform.GetChild(0) : null;

        // update the sprite order of the weapon so it shows in front of / behind the creature's limbs as intended
        Orderer.UpdateSpriteOrder(PhysicalWeapon.transform, transform.parent.parent.parent);
    }
}
