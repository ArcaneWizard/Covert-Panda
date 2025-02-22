using UnityEngine;

public class W_Railgun : WeaponBehaviour
{
    private ParticleSystem chargeParticles;

    public override void StartChargingUp()
    {
        chargeParticles.Stop();
        chargeParticles.Play();
    }
    public override void StopChargingUp() => chargeParticles.Clear();

    protected override void attack(Vector2 aim)
    {
        CommonWeaponBehaviours.SpawnAndShootBulletForward(aim, weaponSystem, weaponConfiguration, side);
        AttackProgress = AttackProgress.Finished;
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
