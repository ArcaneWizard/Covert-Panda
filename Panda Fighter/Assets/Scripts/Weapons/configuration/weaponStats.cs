using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats 
{
    private IKArmsHandler limbs;
    private PhysicalWeapons equip;

    private CentralWeaponSystem weaponSystem;

    public WeaponStats(CentralWeaponSystem weaponSystem)
    {
        Transform options = weaponSystem.transform.GetChild(0).GetChild(0);
        limbs = options.GetComponent<IKArmsHandler>();
        equip = options.GetComponent<PhysicalWeapons>();

        this.weaponSystem = weaponSystem;
    }

    public void Initialize()
    {
        set(Weapon.LavaPistol, 7f, 32,  200, 50,
            90,  30,
            CombatType.Gun, FiringMode.SingleFire, limbs.PistolGrip, equip.LavaOrbLauncher);

        set(Weapon.PlasmaOrb, 4f, 32,  90, 50,
            0, 140, 
            CombatType.Gun, FiringMode.SpamFire, limbs.ShortBarrel, equip.PlasmaOrbLauncher);

        set(Weapon.Railgun, 1.0f, 32, 200, 50,
            30, 110, 
            CombatType.Gun, FiringMode.ChargeUpFire, limbs.MiddleBarrel, equip.Shielder);

        set(Weapon.LeafScythe, 3f, 10,  -1, 50,
            500, 0, 
            CombatType.Meelee, FiringMode.SingleFire, limbs.MeeleGrip, equip.LeafScythe);

        set(Weapon.Shotgun, 1.15f,  10,  300, 50,
            600, 0, 
            CombatType.Gun, FiringMode.SingleFire, limbs.ShortBarrel, equip.GoldenShotgun);

        set(Weapon.ArcticPistol, 6f,  32,  200, 50,  
            90,  0, 
            CombatType.Gun, FiringMode.SingleFire, limbs.PistolGrip, equip.ArcticCannon);

        set(Weapon.PlasmaSniper, 1.42f,  38,  -1, 50, 
            200, 20, 
            CombatType.Gun, FiringMode.SingleFire, limbs.LongBarrel, equip.Sniper);

        set(Weapon.Needler, 6.5f, 32,  200, 200, 
            30,  0, 
            CombatType.Gun, FiringMode.SpamFire, limbs.MiddleBarrel, equip.Needler);

        set(Weapon.FocusBeamer, 0.00f, 34,  -1, 2000,
            7,   0, 
            CombatType.Gun, FiringMode.ContinousBeam, limbs.ShortBarrel, equip.FocusBeamer);
            
        set(Weapon.RocketLauncher, 1.2f,  34,  140, 50,
            0, 300, 
            CombatType.Gun, FiringMode.SingleFire, limbs.ShoulderRest, equip.RocketLauncher);

        set(Weapon.ArcticSprayer, 7f, 32,  100, 50, 
            60,  140, 
            CombatType.Gun, FiringMode.SingleFire, limbs.ShoulderRest, equip.ArcticSprayer);
    }

    private void set(Weapon weapon, float maxAttacksPerSecond, float weaponRange, int bulletSpeed, int startingAmmo, int bulletDmg,
        int explosionDmg, CombatType combatType, FiringMode firingMode, List<GameObject> limbs, GameObject physicalWeapon)
    {
        weaponSystem.GetConfiguration(weapon).Initialize(maxAttacksPerSecond, combatType, firingMode, weaponRange, 
            bulletSpeed, startingAmmo, bulletDmg, explosionDmg, limbs, physicalWeapon);
    }
}
