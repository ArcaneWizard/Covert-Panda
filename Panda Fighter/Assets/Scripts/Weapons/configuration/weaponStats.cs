using UnityEngine;

public class WeaponStats
{
    private IKArmsHandler arms;
    private PhysicalWeapons equip;

    private CentralWeaponSystem weaponSystem;
    private const int INSTANT_SPEED = -1;
    private const int NO_SPEED = -1;
    //private const int NO_AMMO = -1;

    public WeaponStats(CentralWeaponSystem weaponSystem)
    {
        Transform options = weaponSystem.transform.GetChild(0).GetChild(0);
        arms = options.GetComponent<IKArmsHandler>();
        equip = options.GetComponent<PhysicalWeapons>();

        this.weaponSystem = weaponSystem;
    }

    public void Initialize()
    {
        set(Weapon.LavaPistol, 7f, 32, 200, 50,
            90, 30,
            CombatType.Gun, FiringMode.SingleFire, arms.PistolGrip, equip.LavaOrbLauncher);

        set(Weapon.PlasmaOrb, 4f, 32, 90, 50,
            0, 140,
            CombatType.Gun, FiringMode.SpamFire, arms.ShortBarrel, equip.PlasmaOrbLauncher);

        set(Weapon.Railgun, 0.6f, 32, 1000, 50,
            800, 110,
            CombatType.Gun, FiringMode.ChargeUpFire, arms.MiddleBarrel, equip.Shielder);

        set(Weapon.LeafScythe, 3f, 10, NO_SPEED, 50,
            500, 0,
            CombatType.Meelee, FiringMode.SingleFire, arms.MeeleGrip, equip.LeafScythe);

        set(Weapon.Shotgun, 1.15f, 10, 300, 50,
            600, 0,
            CombatType.Gun, FiringMode.SingleFire, arms.ShortBarrel, equip.GoldenShotgun);

        set(Weapon.ArcticPistol, 10f, 32, 350, 50,
            90, 0,
            CombatType.Gun, FiringMode.SingleFire, arms.PistolGrip, equip.ArcticCannon);

        set(Weapon.PlasmaSniper, 1.42f, 200f, INSTANT_SPEED, 50,
            200, 20,
            CombatType.Gun, FiringMode.SingleFire, arms.LongBarrel, equip.Sniper);

        set(Weapon.Needler, 6.5f, 32, 200, 200,
            50, 0,
            CombatType.Gun, FiringMode.SpamFire, arms.MiddleBarrel, equip.Needler);

        set(Weapon.FocusBeamer, 0.00f, 34, INSTANT_SPEED, 2000,
            210, 0,
            CombatType.Gun, FiringMode.ContinousBeam, arms.ShortBarrel, equip.FocusBeamer);

        set(Weapon.RocketLauncher, 1.2f, 34, 140, 50,
            0, 300,
            CombatType.Gun, FiringMode.SingleFire, arms.ShoulderRest, equip.RocketLauncher);

        set(Weapon.ArcticSprayer, 7f, 32, 100, 50,
            60, 140,
            CombatType.Gun, FiringMode.SingleFire, arms.ShoulderRest, equip.ArcticSprayer);
    }

    private void set(Weapon weapon, float fireRateInfo, float weaponRange, int speed, int startingAmmo, int dmg,
        int explosionDmg, CombatType combatType, FiringMode firingMode, GameObject limbs, GameObject physicalWeapon)
    {
        weaponSystem.GetConfiguration(weapon).Initialize(fireRateInfo, combatType, firingMode, weaponRange,
            speed, startingAmmo, dmg, explosionDmg, limbs, physicalWeapon);
    }
}
