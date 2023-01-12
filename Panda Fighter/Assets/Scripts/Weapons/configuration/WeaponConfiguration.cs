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
    public List<GameObject> Arms { get; private set; }
    public GameObject PhysicalWeapon { get; private set; }
    public Transform WeaponPivot { get; private set; }
    public Animator Animator { get; private set; }

    // Inverse Kinematics (IK) is used to sync the elbow/shoulder rotation with the direction the weapon's aimed:
    public Transform MainArmIKTracker { get; private set; }
    public List<Vector2> MainArmIKCoordinates { get; private set; }
    public Transform OtherArmIKTracker { get; private set; }
    public List<Vector2> OtherArmIKCoordinates { get; private set; }

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

        // update the sprite order of the weapon so it shows in front of / behind the creature's limbs as intended
        Orderer.UpdateSpriteOrder(PhysicalWeapon.transform.GetComponent<SpriteRenderer>(), transform.parent.parent.parent);
    }
}
