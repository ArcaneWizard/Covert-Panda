using UnityEngine;

public class W_FocusBeamer : WeaponBehaviour
{
    protected override void attack(Vector2 aim)
    {
        PhysicalBullet bullet = CommonWeaponBehaviours.SpawnBullet(aim, weaponSystem, weaponConfiguration, side);

        bullet.Transform.GetComponent<FocusBeam>().ShootBeam(weaponConfiguration.BulletSpawnPoint,
            weaponConfiguration.PhysicalWeapon.transform, phaseTracker.IsDoingSomersault);

        bullet.Transform.GetComponent<Bullet>().OnFire(aim);
    }
}