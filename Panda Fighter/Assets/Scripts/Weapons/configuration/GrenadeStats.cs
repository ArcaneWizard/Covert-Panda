using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeStats
{
    private Limbs limbs;
    private PhysicalWeapons equip;

    private CentralGrenadeSystem grenadeSystem;

    public GrenadeStats(CentralGrenadeSystem grenadeSystem)
    {
        Transform options = grenadeSystem.transform.GetChild(0).GetChild(0);
        limbs = options.GetComponent<Limbs>();
        equip = options.GetComponent<PhysicalWeapons>();

        this.grenadeSystem = grenadeSystem;
    }

    public void Initialize()
    {
        set(Grenade.Frag, 0.3f, 32, 52, 5000,
            0, 600,
            Mode.handheld, WeaponType.singleFire, limbs.Hands, equip.GrenadeHands);
    }

    private void set(Grenade grenade, float fireRateInfo, float weaponRange, int bulletSpeed, int startingAmmo, int bulletDmg,
        int explosionDmg, string combatMode, string weaponType, List<GameObject> limbs, GameObject physicalWeapon)
    {
        grenadeSystem.GetConfiguration(grenade).Initialize(fireRateInfo, combatMode, weaponType, weaponRange,
            bulletSpeed, startingAmmo, bulletDmg, explosionDmg, limbs, physicalWeapon);
    }
}
