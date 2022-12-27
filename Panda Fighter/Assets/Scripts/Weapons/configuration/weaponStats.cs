using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats 
{
    private Limbs limbs;
    private PhysicalWeapons equip;

    private CentralWeaponSystem weaponSystem;

    public WeaponStats(CentralWeaponSystem weaponSystem)
    {
        Transform options = weaponSystem.transform.GetChild(0).GetChild(0);
        limbs = options.GetComponent<Limbs>();
        equip = options.GetComponent<PhysicalWeapons>();

        this.weaponSystem = weaponSystem;


        set(Weapon.Grenades, 0.3f, 32, 52, 5000,
            0, 600, 
            Mode.handheld, Type.singleFire, limbs.Hands, equip.GrenadeHands);

        set(Weapon.LavaPistol, 0.15f, 32,  200, 5000,
            90,  30,
            Mode.gun, Type.singleFire, limbs.Pistol_grip, equip.LavaOrbLauncher);

        set(Weapon.PlasmaOrb, 0.25f, 32,  90, 5000,
            0, 140, 
            Mode.gun, Type.spamFire, limbs.Short_barrel, equip.PlasmaOrbLauncher);

        set(Weapon.Railgun, 0.2f, 32, 200, 30,
            30, 110, 
            Mode.gun, Type.singleFire, limbs.Middle_barrel, equip.Shielder);

        set(Weapon.LeafScythe, 0.33f, 10,  -1, 5000,
            500, 0, 
            Mode.meelee, Type.singleFire, limbs.Meelee_grip, equip.LeafScythe);

        set(Weapon.Shotgun, 0.6f,  10,  300, 5000,
            600, 0, 
            Mode.gun, Type.singleFire, limbs.Short_barrel, equip.GoldenShotgun);

        set(Weapon.ArcticPistol, 0.2f,  32,  200, 5000,  
            90,  0, 
            Mode.gun, Type.singleFire, limbs.Pistol_grip, equip.ArcticCannon);

        set(Weapon.PlasmaSniper, 0.8f,  38,  -1,  15, 
            200, 20, 
            Mode.gun, Type.singleFire, limbs.Long_barrel, equip.Sniper);

        set(Weapon.Needler, 0.15f, 32,  200, 5000, 
            30,  0, 
            Mode.gun, Type.spamFire, limbs.Middle_barrel, equip.Needler);

        set(Weapon.FocusBeamer, 0.00f, 34,  -1,  5000,
            7,   0, 
            Mode.gun, Type.holdFire, limbs.Short_barrel, equip.FocusBeamer);
            
        set(Weapon.RocketLauncher, 0.8f,  34,  140, 5000,
            0, 300, 
            Mode.gun, Type.singleFire, limbs.Shoulder_rest, equip.RocketLauncher);

        set(Weapon.ArcticSprayer, 0.07f, 32,  100, 5000, 
            60,  140, 
            Mode.gun, Type.spamFire, limbs.Shoulder_rest, equip.ArcticSprayer);
    }

    private void set(Weapon weapon, float fireRateInfo, float weaponRange, int bulletSpeed, int startingAmmo, int bulletDmg,
        int explosionDmg, string combatMode, string weaponType, List<GameObject> limbs, GameObject physicalWeapon)
    {
        weaponSystem.GetConfiguration(weapon).update(fireRateInfo, combatMode, weaponType, weaponRange, 
            bulletSpeed, startingAmmo, bulletDmg, explosionDmg, limbs, physicalWeapon);
    }
}
