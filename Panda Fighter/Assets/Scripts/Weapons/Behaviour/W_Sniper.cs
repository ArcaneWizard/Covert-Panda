using UnityEngine;

public class W_Sniper : WeaponBehaviour
{
    protected override void attack(Vector2 aim)
    {
        PhysicalBullet bullet = CommonWeaponBehaviours.SpawnBullet(aim, weaponSystem, weaponConfiguration, side);
        bullet.Transform.GetComponent<SniperBeam>().ShowBeam(aim);

        AttackProgress = AttackProgress.Finished;
    }
}
