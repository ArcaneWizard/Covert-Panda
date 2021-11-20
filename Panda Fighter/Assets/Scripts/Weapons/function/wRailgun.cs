using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wRailgun : IWeapon
{
    private float timer;
    private ParticleSystem chargeParticles;

    public override IEnumerator SetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        timer = 0f;
        showVisualChargingUp();

        while (timer < config.fireRateInfo)
        {
            if (!Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
            {
                attackProgress = "finished";
                chargeParticles.Clear();
                yield return null;
            }

            yield return new WaitForSeconds(Time.deltaTime);
            timer += Time.deltaTime;
        }

        if (Input.GetMouseButton(0))
            DoAttack(shooting.getAim(), bullet, rig);
    }

    public override void Attack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        reusableWeaponMethods.configureReusedBullet(bullet, rig, config.bulletSpawnPoint);
        reusableWeaponMethods.shootBulletInStraightLine(aim, bullet, rig, config.bulletSpeed);
    }

    private void Start()
    {
        chargeParticles = config.bulletSpawnPoint.transform.GetChild(0).transform.GetComponent<ParticleSystem>();
        chargeParticles.Clear();
    }

    private void showVisualChargingUp() => chargeParticles.Play();
}
