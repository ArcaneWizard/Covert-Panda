using UnityEngine;

public class W_Needler : WeaponBehaviour
{
    protected override void attack(Vector2 aim)
    {
        float yOffset = Random.Range(-0.13f, 0.13f);
        Transform bullet = CommonWeaponBehaviours.SpawnAndShootBulletForward(aim, weaponSystem,
            weaponConfiguration, side, extraSettings);

        AttackProgress = AttackProgress.Finished;

        void extraSettings(Transform bullet)
        {
            bullet.position += new Vector3(0, yOffset, 0f);
        }
    }
}
