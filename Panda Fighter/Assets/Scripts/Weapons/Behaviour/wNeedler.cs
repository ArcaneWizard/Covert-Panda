using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wNeedler : WeaponBehaviour
{
    public override IEnumerator Attack(Vector2 aim)
    {
        StartCoroutine(base.Attack(aim));

        Transform bullet = WeaponAction.SpawnAndShootBulletForward(aim, weaponSystem, weaponConfiguration, side, false);
        float yOffset = Random.Range(-0.13f, 0.13f);
        bullet.position += new Vector3(0, yOffset, 0f);

        ConfirmAttackFinished();
        yield return null;
    }
}

