using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wRailgun : WeaponBehaviour
{
    private ParticleSystem chargeParticles;

    public override IEnumerator Attack(Vector2 aim)
    {
        StartCoroutine(base.Attack(aim));

        showVisualChargingUp();
        WeaponAction.SpawnAndShootBulletForward(aim,weaponSystem, weaponConfiguration, side, false);

        ConfirmAttackFinished();
        yield return null;
    }

    private void Start()
    {
        chargeParticles = weaponConfiguration.BulletSpawnPoint.transform.GetChild(0).transform.GetComponent<ParticleSystem>();
        chargeParticles.Clear();
    }

    private void showVisualChargingUp() => chargeParticles.Play();
}