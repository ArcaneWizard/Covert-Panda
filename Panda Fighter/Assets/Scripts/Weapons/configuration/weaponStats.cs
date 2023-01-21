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
            WeaponTypes.gun, FiringModes.singleFire, limbs.PistolGrip, equip.LavaOrbLauncher);

        set(Weapon.PlasmaOrb, 4f, 32,  90, 50,
            0, 140, 
            WeaponTypes.gun, FiringModes.spamFire, limbs.ShortBarrel, equip.PlasmaOrbLauncher);

        set(Weapon.Railgun, 5f, 32, 200, 50,
            30, 110, 
            WeaponTypes.gun, FiringModes.singleFire, limbs.MiddleBarrel, equip.Shielder);

        set(Weapon.LeafScythe, 3f, 10,  -1, 50,
            500, 0, 
            WeaponTypes.meelee, FiringModes.singleFire, limbs.MeeleGrip, equip.LeafScythe);

        set(Weapon.Shotgun, 1.1f,  10,  300, 50,
            600, 0, 
            WeaponTypes.gun, FiringModes.singleFire, limbs.ShortBarrel, equip.GoldenShotgun);

        set(Weapon.ArcticPistol, 6f,  32,  200, 50,  
            90,  0, 
            WeaponTypes.gun, FiringModes.singleFire, limbs.PistolGrip, equip.ArcticCannon);

        set(Weapon.PlasmaSniper, 1.5f,  38,  -1, 50, 
            200, 20, 
            WeaponTypes.gun, FiringModes.singleFire, limbs.LongBarrel, equip.Sniper);

        set(Weapon.Needler, 6.5f, 32,  200, 200, 
            30,  0, 
            WeaponTypes.gun, FiringModes.spamFire, limbs.MiddleBarrel, equip.Needler);

        set(Weapon.FocusBeamer, 0.00f, 34,  -1, 2000,
            7,   0, 
            WeaponTypes.gun, FiringModes.holdFire, limbs.ShortBarrel, equip.FocusBeamer);
            
        set(Weapon.RocketLauncher, 1.2f,  34,  140, 50,
            0, 300, 
            WeaponTypes.gun, FiringModes.singleFire, limbs.ShoulderRest, equip.RocketLauncher);

        set(Weapon.ArcticSprayer, 7f, 32,  100, 50, 
            60,  140, 
            WeaponTypes.gun, FiringModes.singleFire, limbs.ShoulderRest, equip.ArcticSprayer);
    }

    private void set(Weapon weapon, float maxAttacksPerSecond, float weaponRange, int bulletSpeed, int startingAmmo, int bulletDmg,
        int explosionDmg, string combatMode, string weaponType, List<GameObject> limbs, GameObject physicalWeapon)
    {
        weaponSystem.GetConfiguration(weapon).Initialize(maxAttacksPerSecond, combatMode, weaponType, weaponRange, 
            bulletSpeed, startingAmmo, bulletDmg, explosionDmg, limbs, physicalWeapon);
    }
}
