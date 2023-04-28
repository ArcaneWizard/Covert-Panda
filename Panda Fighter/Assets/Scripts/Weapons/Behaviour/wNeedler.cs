using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wNeedler : WeaponBehaviour
{
    protected override IEnumerator attack(Vector2 aim)
    {
        StartCoroutine(base.attack(aim));

        Transform bullet = WeaponBehaviourHelper.SpawnAndShootBulletForward(aim, weaponSystem, weaponConfiguration, side);
        float yOffset = Random.Range(-0.13f, 0.13f);
        bullet.position += new Vector3(0, yOffset, 0f);

        confirmAttackFinished();
        yield return null;
    }
}

