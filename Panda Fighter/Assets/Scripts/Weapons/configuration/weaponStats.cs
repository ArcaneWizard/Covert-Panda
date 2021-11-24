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

        set(WeaponTags.Grenades, Mode.handheld, Type.singleFire, 0, 52, 300, l.Hands, w.GrenadeHands);
        set(WeaponTags.LavaPistol, Mode.gun, Type.singleFire, 0, 92, 300, l.Pistol_grip, w.LavaOrbLauncher);
        set(WeaponTags.PlasmaOrb, Mode.gun, Type.singleFire, 0, 50, 300, l.Short_barrel, w.PlasmaOrbLauncher);

        set(WeaponTags.Railgun, Mode.gun, Type.chargeUpFire, 0.5f, 120, 300, l.Middle_barrel, w.Shielder);
        set(WeaponTags.LeafScythe, Mode.meelee, Type.singleFire, 0, 60, 300, l.Meelee_grip, w.LeafScythe);
        set(WeaponTags.Shotgun, Mode.gun, Type.singleFire, 0, 52, 300, l.Short_barrel, w.GoldenShotgun);

        set(WeaponTags.ArcticPistol, Mode.gun, Type.singleFire, 0, 72, 300, l.Pistol_grip, w.ArcticCannon);
        set(WeaponTags.PlasmaSniper, Mode.gun, Type.singleFire, 0, 1000, 300, l.Long_barrel, w.Sniper);
        set(WeaponTags.Needler, Mode.gun, Type.spamFire, 9, 120, 300, l.Middle_barrel, w.Needler);

        set(WeaponTags.FocusBeamer, Mode.gun, Type.holdFire, 0, 72, 3000, l.Short_barrel, w.FocusBeamer);
        set(WeaponTags.RocketLauncher, Mode.gun, Type.singleFire, 0, 100, 300, l.Shoulder_rest, w.RocketLauncher);
        set(WeaponTags.ArcticSprayer, Mode.gun, Type.spamFire, 4, 42, 300, l.Shoulder_rest, w.ArcticSprayer);
    }

    private void set(WeaponTags tag, string combatMode, string weaponType, float fireRateInfo, int bulletSpeed,
        int startingAmmo, List<GameObject> limbs, GameObject weapon)
    {
        weaponSystem.getWeaponConfiguration(tag.ToString()).update(
            combatMode, weaponType, fireRateInfo, bulletSpeed, startingAmmo, limbs, weapon
        );
    }
}
