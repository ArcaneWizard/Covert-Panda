using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class weaponStats : MonoBehaviour
{
    public WeaponConfig Grenade;
    public WeaponConfig LavaOrb;
    public WeaponConfig PlasmaOrb;
    public WeaponConfig Shielder;
    public WeaponConfig LeafScythe;
    public WeaponConfig GoldenShotgun;
    public WeaponConfig ArcticCannon;
    public WeaponConfig Needler;
    public WeaponConfig Sniper;

    private Limbs l;
    private Weapons w;

    private void Awake()
    {
        Transform options = transform.parent.GetChild(0).GetChild(0).GetChild(0);
        l = options.GetComponent<Limbs>();
        w = options.GetComponent<Weapons>();

        Grenade.update(Mode.handheld, Type.singleFire, 0, 52, 300, l.Hands, w.GrenadeHands);
        LavaOrb.update(Mode.gun, Type.singleFire, 0, 92, 300, l.Pistol_grip, w.LavaOrbLauncher);
        PlasmaOrb.update(Mode.gun, Type.singleFire, 0, 52, 300, l.Short_barrel, w.PlasmaOrbLauncher);
        
        Shielder.update(Mode.gun, Type.holdFire, 0.01f, 120, 300, l.Middle_barrel, w.Shielder);
        LeafScythe.update(Mode.meelee, Type.singleFire, 0, 60, 300, l.Meelee_grip, w.LeafScythe);
        GoldenShotgun.update(Mode.gun, Type.singleFire, 0, 52, 300, l.Short_barrel, w.GoldenShotgun);

        ArcticCannon.update(Mode.gun, Type.singleFire, 0, 72, 300, l.Pistol_grip, w.ArcticCannon);
        Sniper.update(Mode.gun, Type.singleFire, 0, 1000, 300, l.Long_barrel, w.Sniper);
        Needler.update(Mode.gun, Type.spamFire, 9, 120, 300, l.Middle_barrel, w.Needler);
    }

}
