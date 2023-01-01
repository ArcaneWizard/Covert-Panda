using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats 
{
    private ArmsHandler limbs;
    private PhysicalWeapons equip;

    private CentralWeaponSystem weaponSystem;

    public WeaponStats(CentralWeaponSystem weaponSystem)
    {
        Transform options = weaponSystem.transform.GetChild(0).GetChild(0);
        limbs = options.GetComponent<ArmsHandler>();
        equip = options.GetComponent<PhysicalWeapons>();

        this.weaponSystem = weaponSystem;
    }

    public void Initialize()
    {
        set(Weapon.LavaPistol, 0.15f, 32,  200, 20,
            90,  30,
            WeaponTypes.gun, FiringModes.singleFire, limbs.Pistol_grip, equip.LavaOrbLauncher);

        set(Weapon.PlasmaOrb, 0.25f, 32,  90, 20,
            0, 140, 
            WeaponTypes.gun, FiringModes.spamFire, limbs.Short_barrel, equip.PlasmaOrbLauncher);

        set(Weapon.Railgun, 0.2f, 32, 200, 30,
            30, 110, 
            WeaponTypes.gun, FiringModes.singleFire, limbs.Middle_barrel, equip.Shielder);

        set(Weapon.LeafScythe, 0.33f, 10,  -1, 20,
            500, 0, 
            WeaponTypes.meelee, FiringModes.singleFire, limbs.Meelee_grip, equip.LeafScythe);

        set(Weapon.Shotgun, 0.6f,  10,  300, 20,
            600, 0, 
            WeaponTypes.gun, FiringModes.singleFire, limbs.Short_barrel, equip.GoldenShotgun);

        set(Weapon.ArcticPistol, 0.2f,  32,  200, 20,  
            90,  0, 
            WeaponTypes.gun, FiringModes.singleFire, limbs.Pistol_grip, equip.ArcticCannon);

        set(Weapon.PlasmaSniper, 0.8f,  38,  -1,  15, 
            200, 20, 
            WeaponTypes.gun, FiringModes.singleFire, limbs.Long_barrel, equip.Sniper);

        set(Weapon.Needler, 0.15f, 32,  200, 50, 
            30,  0, 
            WeaponTypes.gun, FiringModes.spamFire, limbs.Middle_barrel, equip.Needler);

        set(Weapon.FocusBeamer, 0.00f, 34,  -1,  20,
            7,   0, 
            WeaponTypes.gun, FiringModes.holdFire, limbs.Short_barrel, equip.FocusBeamer);
            
        set(Weapon.RocketLauncher, 0.8f,  34,  140, 7,
            0, 300, 
            WeaponTypes.gun, FiringModes.singleFire, limbs.Shoulder_rest, equip.RocketLauncher);

        set(Weapon.ArcticSprayer, 0.07f, 32,  100, 15, 
            60,  140, 
            WeaponTypes.gun, FiringModes.singleFire, limbs.Shoulder_rest, equip.ArcticSprayer);
    }

    private void set(Weapon weapon, float maxAttacksPerSecond, float weaponRange, int bulletSpeed, int startingAmmo, int bulletDmg,
        int explosionDmg, string combatMode, string weaponType, List<GameObject> limbs, GameObject physicalWeapon)
    {
        weaponSystem.GetConfiguration(weapon).Initialize(maxAttacksPerSecond, combatMode, weaponType, weaponRange, 
            bulletSpeed, startingAmmo, bulletDmg, explosionDmg, limbs, physicalWeapon);
    }
}
