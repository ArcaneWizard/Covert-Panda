using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponConfiguration : MonoBehaviour
{
    public string combatMode { get; private set; }
    public string weaponType { get; private set; }
    public int bulletDmg { get; private set; }
    public int explosionDmg { get; private set; }
    public int startingAmmo { get; private set; }
    public int bulletSpeed { get; private set; }
    public float weaponRange { get; private set; }
    public float fireRateInfo { get; private set; }  //required for spam fire weapons

    public Transform bulletSpawnPoint { get; private set; }
    public List<GameObject> limbs { get; private set; }
    public GameObject weapon { get; private set; }
    public Transform weaponPivot { get; private set; }
    public List<Vector2> weaponIKCoordinates { get; private set; } //required for configuring a weapon's aim to be very precise when using a mouse
    public Transform weaponAimTracker { get; private set; }  //required for configuring a weapon's aim to be very precise when using a mouse

    public CentralWeaponSystem weaponSystem { get; private set; }
    public CentralShooting shooting { get; private set; }
    public Animator animator { get; private set; }

    private Transform entity;
    private Limbs Limbs;

    public void update(float fireRateInfo, string combatMode, string weaponType, float weaponRange, int bulletSpeed, int startingAmmo,
        int bulletDmg, int explosionDmg, List<GameObject> limbs, GameObject weapon)
    {
        this.fireRateInfo = fireRateInfo;
        this.combatMode = combatMode;
        this.weaponType = weaponType;
        this.weaponRange = weaponRange;
        this.bulletSpeed = bulletSpeed;
        this.startingAmmo = startingAmmo;
        this.bulletDmg = bulletDmg;
        this.explosionDmg = explosionDmg;
        this.limbs = limbs;
        this.weapon = weapon;

        setup();
    }

    private void setup()
    {
        entity = transform.parent.parent.parent.transform.GetChild(0);
        weaponSystem = entity.GetComponent<CentralWeaponSystem>();
        shooting = entity.GetComponent<CentralShooting>();
        animator = entity.GetComponent<Animator>();

        Limbs = entity.transform.GetChild(0).GetChild(0).transform.GetComponent<Limbs>();
        weaponAimTracker = Limbs.GetIK_WeaponAimTracker(limbs);
        weaponIKCoordinates = Limbs.GetIK_WeaponCoordinates(limbs);

        weaponPivot = (weaponAimTracker) ? weaponAimTracker.parent.GetChild(1) : null;

        if (weapon.transform.childCount > 0) 
            bulletSpawnPoint = weapon.transform.GetChild(0);
        
        Orderer.updateSpriteOrder(weapon.transform, transform.parent.parent.parent);
    }
}
