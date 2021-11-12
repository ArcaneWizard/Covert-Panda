using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponConfig : MonoBehaviour
{
    public string combatMode { get; private set; }
    public int startingAmmo { get; private set; }
    public List<GameObject> limbs { get; private set; }
    public Transform bulletSpawnPoint { get; private set; }
    public GameObject weapon { get; private set; }
    public Transform aimTarget { get; private set; }  //required when combat mode isn't handheld
    public float ratePerSecond { get; private set; }  //required for spam fire weapons

    [HideInInspector] public List<Vector2> IK_Coordinates = AimingDir.defaultAiming;
    public CentralWeaponSystem weaponSystem { get; private set; }
    public CentralShooting shooting { get; private set; }
    public Animator animator { get; private set; }

    private Transform entity;


    public void update(string combatMode, int startingAmmo, List<GameObject> limbs, GameObject weapon,
            Transform aimTarget, float ratePerSecond)
    {
        this.combatMode = combatMode;
        this.startingAmmo = startingAmmo;
        this.limbs = limbs;
        this.weapon = weapon;
        this.aimTarget = aimTarget;
        this.ratePerSecond = ratePerSecond;

        setup();
    }

    private void setup()
    {
        entity = transform.parent.parent.parent.transform.GetChild(0);
        weaponSystem = entity.GetComponent<CentralWeaponSystem>();
        shooting = entity.GetComponent<CentralShooting>();
        animator = entity.GetComponent<Animator>();

        if (weapon.transform.childCount > 0) bulletSpawnPoint = weapon.transform.GetChild(0);
    }
}
