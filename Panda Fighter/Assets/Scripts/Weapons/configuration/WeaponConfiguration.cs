using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class houses all stats about a specific weapon (ex. bullet speed, explosion dmg)
// It also houses physical components of the weapon relevant to using it correctly.

public class WeaponConfiguration : MonoBehaviour
{
    public int Damage { get; private set; }
    public int ExplosionDmg { get; private set; }
    public int StartingAmmo { get; private set; }
    public int Speed { get; private set; }
    public float Range { get; private set; }

    // By default, represents max shots fired per second
    // Exception being for charge-up weapons, where this represents time to charge up shot
    public float FireRateInfo { get; private set; }

    public CombatType CombatType { get; private set; }
    public FiringMode FiringMode { get; private set; }

    public Transform BulletSpawnPoint { get; private set; }
    public GameObject Arms { get; private set; }
    public GameObject PhysicalWeapon { get; private set; }
    public Transform WeaponPivot { get; private set; }
    public Animator Animator { get; private set; }

    // Inverse Kinematics (IK) is used to sync the elbow/shoulder rotation with the direction the weapon's aimed:
    public Transform MainArmIKTracker { get; private set; }
    public List<Vector2> MainArmIKCoordinates { get; private set; }
    public Transform OtherArmIKTracker { get; private set; }
    public List<Vector2> OtherArmIKCoordinates { get; private set; }

    public void Initialize(float fireRateInfo, CombatType combatType, FiringMode firingMode, float weaponRange, int bulletSpeed, int startingAmmo,
        int bulletDmg, int explosionDmg, GameObject arms, GameObject weapon)
    {
        this.FireRateInfo = fireRateInfo;
        this.CombatType = combatType;
        this.FiringMode = firingMode;
        this.Range = weaponRange;
        this.Speed = bulletSpeed;
        this.StartingAmmo = startingAmmo;
        this.Damage = bulletDmg;
        this.ExplosionDmg = explosionDmg;
        this.Arms = arms;
        this.PhysicalWeapon = weapon;

        setup();
    }

    private void setup()
    {
        Transform creature = transform.parent.parent.parent.transform.GetChild(0);
        Animator = creature.GetComponent<Animator>();

        IKArmsHandler armsHandler = creature.GetChild(0).GetChild(0).GetComponent<IKArmsHandler>();

        MainArmIKTracker = armsHandler.GetIKTarget(Arms, true);
        MainArmIKCoordinates = armsHandler.GetIKCoordinates(Arms, true);
        OtherArmIKTracker = armsHandler.GetIKTarget(Arms, false);
        OtherArmIKCoordinates = armsHandler.GetIKCoordinates(Arms, false);

        // if weapon pivot point exists for this weapon, store it
        WeaponPivot = MainArmIKTracker ? MainArmIKTracker.parent.GetChild(1) : null;

        // if bullet spawn point exists for this weapon, store it
        BulletSpawnPoint = (PhysicalWeapon.transform.childCount > 0) ? PhysicalWeapon.transform.GetChild(0) : null;
    }
}
