using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wRailgun : WeaponBehaviour
{
    private ParticleSystem chargeParticles;

    public override void StartChargingUp()
    {
        chargeParticles.Stop();
        chargeParticles.Play();
    }
    public override void StopChargingUp() => chargeParticles.Clear();

    protected override void startAttack()
    {
        WeaponBehaviourHelper.SpawnAndShootBulletForward(aim, weaponSystem, weaponConfiguration, side);
    }

    void Start()
    {
        chargeParticles = weaponConfiguration.BulletSpawnPoint.transform.GetChild(0).transform.GetComponent<ParticleSystem>();
        chargeParticles.Clear();
        chargeParticles.Stop();

        var main = chargeParticles.main;
        main.playOnAwake = false;
    }
}
