/*using UnityEngine;

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

    *//* public void Initialize()
     {
         initialize(Grenade.Frag, 0.3f, 32, 52, 5000,
             0, 600,
             CombatType.Handheld, FiringMode.SingleFire, armsHandler., equip.GrenadeHands);
     }

     private void initialize(Grenade grenade, float fireRateInfo, float weaponRange, int bulletSpeed, int startingAmmo, int bulletDmg,
         int explosionDmg, CombatType combatType, FiringMode firingMode, List<GameObject> limbs, GameObject physicalWeapon)
     {
         grenadeSystem.GetConfiguration(grenade).Initialize(fireRateInfo, combatType, firingMode, weaponRange,
             bulletSpeed, startingAmmo, bulletDmg, explosionDmg, limbs, physicalWeapon);
     }*//*
}
*/