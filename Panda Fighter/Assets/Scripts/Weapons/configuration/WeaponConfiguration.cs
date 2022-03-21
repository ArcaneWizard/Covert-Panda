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
    public List<GameObject> limbs { get; private set; }
    public Transform bulletSpawnPoint { get; private set; }
    public GameObject weapon { get; private set; }
    public List<Vector2> IK_Coordinates { get; private set; } //required for configuring aiming
    public Transform aimTarget { get; private set; }  //required when combat mode isn't handheld
    public float fireRateInfo { get; private set; }  //required for spam fire weapons

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
        aimTarget = Limbs.getAimTarget(limbs);
        IK_Coordinates = Limbs.getIKCoordinates(limbs);

        if (weapon.transform.childCount > 0) 
            bulletSpawnPoint = weapon.transform.GetChild(0);
        
        Orderer.updateOrder(weapon.transform, transform.parent.parent.parent);
    }
}
