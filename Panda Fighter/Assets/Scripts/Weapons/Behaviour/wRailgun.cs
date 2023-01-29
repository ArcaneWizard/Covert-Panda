using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wRailgun : WeaponBehaviour
{
    private ParticleSystem chargeParticles;

    public override void StartChargingUp() => chargeParticles.Play();
    public override void StopChargingUp() => chargeParticles.Clear();

    protected override IEnumerator attack(Vector2 aim)
    {
        StartCoroutine(base.attack(aim));

        WeaponAction.SpawnAndShootBulletForward(aim, weaponSystem, weaponConfiguration, side, false);

        confirmAttackFinished();
        yield return null;
    }

    void Start()
    {
        chargeParticles = weaponConfiguration.BulletSpawnPoint.transform.GetChild(0).transform.GetComponent<ParticleSystem>();
        chargeParticles.Clear();
    }
}
