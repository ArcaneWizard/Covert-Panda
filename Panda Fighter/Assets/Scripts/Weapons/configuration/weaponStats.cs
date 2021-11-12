using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class weaponStats : MonoBehaviour
{
    public WeaponConfig Grenade;
    public WeaponConfig Boomerang;
    public WeaponConfig PlasmaOrb;
    public WeaponConfig Shielder;
    public WeaponConfig LeafScythe;
    public WeaponConfig GoldenShotgun;
    public WeaponConfig ArcticCannon;
    public WeaponConfig Needler;
    public WeaponConfig Sniper;

    private Limbs l;
    private Weapons w;
    private AimTargets aT;

    private void Awake()
    {
        Transform options = transform.parent.GetChild(0).GetChild(0).GetChild(2);
        l = options.GetComponent<Limbs>();
        w = options.GetComponent<Weapons>();
        aT = options.GetComponent<AimTargets>();

        Grenade.update(cM.handheld, 30, l.Hands, w.GrenadeHands, null, 0);
        Boomerang.update(cM.gun, 40, l.Short_barrel, w.BoomerangLauncher, aT.ShortBarrelAim, 0);
        PlasmaOrb.update(cM.gun, 25, l.Short_barrel, w.PlasmaOrbLauncher, aT.ShortBarrelAim, 0);
        Shielder.update(cM.gun, 25, l.Middle_barrel, w.Shielder, aT.MediumBarrelAim, 0);
        LeafScythe.update(cM.meelee, 1, l.Meelee_pole, w.LeafScythe, aT.MeeleePoleAim, 0);
        GoldenShotgun.update(cM.gun, 15, l.Short_barrel, w.GoldenShotgun, aT.ShortBarrelAim, 0);
        ArcticCannon.update(cM.gun, 80, l.Long_barrel, w.ArcticCannon, aT.LongBarrelAim, 0);
        Sniper.update(cM.gun, 100, l.Long_barrel, w.Sniper, aT.LongBarrelAim, 0);
        Needler.update(cM.gun, 100, l.Middle_barrel, w.Needler, aT.MediumBarrelAim, 9);
    }

}
