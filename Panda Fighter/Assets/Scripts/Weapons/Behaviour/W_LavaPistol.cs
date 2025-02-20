using UnityEngine;

public class W_LavaPistol : WeaponBehaviour
{
    protected override void attack(Vector2 aim)
    {
        CommonWeaponBehaviours.SpawnAndShootBulletForward(aim, weaponSystem,
            weaponConfiguration, side, extraSettings);

        void extraSettings(Transform bullet) => bullet.localEulerAngles = Vector3.zero;
    }
}
