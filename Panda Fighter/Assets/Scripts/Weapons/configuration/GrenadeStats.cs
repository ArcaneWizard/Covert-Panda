using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeStats
{
    private IKArmsHandler armsHandler;
    private PhysicalWeapons equip;

    private CentralGrenadeSystem grenadeSystem;

    public GrenadeStats(CentralGrenadeSystem grenadeSystem)
    {
        Transform options = grenadeSystem.transform.GetChild(0).GetChild(0);
        armsHandler = options.GetComponent<IKArmsHandler>();
        equip = options.GetComponent<PhysicalWeapons>();

        this.grenadeSystem = grenadeSystem;
    }

    public void Initialize()
    {
        initialize(Grenade.Frag, 0.3f, 32, 52, 5000,
            0, 600,
            WeaponTypes.handheld, FiringModes.singleFire, armsHandler.Hands, equip.GrenadeHands);
    }

    private void initialize(Grenade grenade, float fireRateInfo, float weaponRange, int bulletSpeed, int startingAmmo, int bulletDmg,
        int explosionDmg, string combatMode, string weaponType, List<GameObject> limbs, GameObject physicalWeapon)
    {
        grenadeSystem.GetConfiguration(grenade).Initialize(fireRateInfo, combatMode, weaponType, weaponRange,
            bulletSpeed, startingAmmo, bulletDmg, explosionDmg, limbs, physicalWeapon);
    }
}
