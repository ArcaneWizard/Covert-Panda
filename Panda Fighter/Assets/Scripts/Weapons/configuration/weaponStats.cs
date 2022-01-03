using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats : MonoBehaviour
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

        set(0.33f, 32, 52, 300, 0, 120, WeaponTags.Grenades, Mode.handheld, Type.singleFire, l.Hands, w.GrenadeHands);
        set(0.33f, 32, 92, 300, 25, 20, WeaponTags.LavaPistol, Mode.gun, Type.singleFire, l.Pistol_grip, w.LavaOrbLauncher);
        set(0.15f, 32, 50, 300, 0, 42, WeaponTags.PlasmaOrb, Mode.gun, Type.singleFire, l.Short_barrel, w.PlasmaOrbLauncher);

        set(0.3f, 32, 99, 300, 40, 50, WeaponTags.Railgun, Mode.gun, Type.singleFire, l.Middle_barrel, w.Shielder);
        set(0.33f, 10, 60, 300, 200, 0, WeaponTags.LeafScythe, Mode.meelee, Type.singleFire, l.Meelee_grip, w.LeafScythe);
        set(0.6f, 10, 52, 300, 200, 0, WeaponTags.Shotgun, Mode.gun, Type.singleFire, l.Short_barrel, w.GoldenShotgun);

        set(0.11f, 32, 150, 300, 25, 0, WeaponTags.ArcticPistol, Mode.gun, Type.singleFire, l.Pistol_grip, w.ArcticCannon);
        set(0.8f, 38, 99, 300, 80, 0, WeaponTags.PlasmaSniper, Mode.gun, Type.singleFire, l.Long_barrel, w.Sniper);
        set(0.12f, 32, 99, 300, 14, 0, WeaponTags.Needler, Mode.gun, Type.spamFire, l.Middle_barrel, w.Needler);

        set(0.00f, 34, 72, 3000, 1, 0, WeaponTags.FocusBeamer, Mode.gun, Type.holdFire, l.Short_barrel, w.FocusBeamer);
        set(0.8f, 34, 99, 300, 20, 205, WeaponTags.RocketLauncher, Mode.gun, Type.singleFire, l.Shoulder_rest, w.RocketLauncher);
        set(0.25f, 32, 42, 300, 50, 80, WeaponTags.ArcticSprayer, Mode.gun, Type.spamFire, l.Shoulder_rest, w.ArcticSprayer);
    }

    private void set(float fireRateInfo, float weaponRange, int bulletSpeed, int startingAmmo, int bulletDmg, 
        int explosionDmg, WeaponTags tag, string combatMode, string weaponType, List<GameObject> limbs, GameObject weapon)
    {
        weaponSystem.getWeaponConfiguration(tag.ToString()).update(
            fireRateInfo, combatMode, weaponType, weaponRange, bulletSpeed, startingAmmo, bulletDmg, explosionDmg, limbs, weapon
        );
    }
}
