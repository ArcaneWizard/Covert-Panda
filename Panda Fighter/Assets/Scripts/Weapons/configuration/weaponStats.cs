using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats 
{
    private Limbs l;
    private Weapons w;

    private CentralWeaponSystem weaponSystem;

    public WeaponStats(CentralWeaponSystem weaponSystem)
    {
        Transform options = weaponSystem.transform.GetChild(0).GetChild(0);
        l = options.GetComponent<Limbs>();
        w = options.GetComponent<Weapons>();

        this.weaponSystem = weaponSystem;

        set(0.3f, 32, 52, 5000, 0, 600, WeaponTags.Grenades, Mode.handheld, Type.singleFire, l.Hands, w.GrenadeHands);
        set(0.15f, 32, 200, 5000, 90, 30, WeaponTags.LavaPistol, Mode.gun, Type.singleFire, l.Pistol_grip, w.LavaOrbLauncher);
        set(0.25f, 32, 90, 5000, 0, 140, WeaponTags.PlasmaOrb, Mode.gun, Type.spamFire, l.Short_barrel, w.PlasmaOrbLauncher);

        set(0.2f, 32, 200, 30, 30, 110, WeaponTags.Railgun, Mode.gun, Type.singleFire, l.Middle_barrel, w.Shielder);
        set(0.33f, 10, -1, 5000, 500, 0, WeaponTags.LeafScythe, Mode.meelee, Type.singleFire, l.Meelee_grip, w.LeafScythe);
        set(0.6f, 10, 300, 5000, 600, 0, WeaponTags.Shotgun, Mode.gun, Type.singleFire, l.Short_barrel, w.GoldenShotgun);

        set(0.2f, 32, 200, 5000, 90, 0, WeaponTags.ArcticPistol, Mode.gun, Type.singleFire, l.Pistol_grip, w.ArcticCannon);
        set(0.8f, 38, -1, 15, 200, 20, WeaponTags.PlasmaSniper, Mode.gun, Type.singleFire, l.Long_barrel, w.Sniper);
        set(0.15f, 32, 200, 5000, 30, 0, WeaponTags.Needler, Mode.gun, Type.spamFire, l.Middle_barrel, w.Needler);

        set(0.00f, 34, -1, 5000, 7, 0, WeaponTags.FocusBeamer, Mode.gun, Type.holdFire, l.Short_barrel, w.FocusBeamer);
        set(0.8f, 34, 140, 5000, 0, 300, WeaponTags.RocketLauncher, Mode.gun, Type.singleFire, l.Shoulder_rest, w.RocketLauncher);
        set(0.07f, 32, 100, 5000, 60, 140, WeaponTags.ArcticSprayer, Mode.gun, Type.spamFire, l.Shoulder_rest, w.ArcticSprayer);
    }

    private void set(float fireRateInfo, float weaponRange, int bulletSpeed, int startingAmmo, int bulletDmg,
        int explosionDmg, WeaponTags tag, string combatMode, string weaponType, List<GameObject> limbs, GameObject weapon)
    {
        weaponSystem.GetWeaponConfiguration(tag).update(fireRateInfo, combatMode, weaponType, weaponRange, 
            bulletSpeed, startingAmmo, bulletDmg, explosionDmg, limbs, weapon);
    }
}
