using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wRailgun : WeaponImplementation
{
    private float timer;
    private ParticleSystem chargeParticles;

    public override IEnumerator SetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        /*timer = 0f;
        showVisualChargingUp();

        while (timer < configuration.fireRateInfo)
        {
            if (!Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
            {
                attackProgress = "finished";
                chargeParticles.Clear();
                yield break;
            }

            yield return new WaitForSeconds(Time.deltaTime);
            timer += Time.deltaTime;
        }*/

        DoAttack(shooting.GetAim(), bullet, rig);
        yield return null;
    }

    public override void Attack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        reusableWeaponMethods.configureReusedBullet(bullet, rig, config.bulletSpawnPoint, side);
        reusableWeaponMethods.shootBulletInStraightLine(aim, bullet, rig, config.bulletSpeed);
    }

    private void Start()
    {
        chargeParticles = config.bulletSpawnPoint.transform.GetChild(0).transform.GetComponent<ParticleSystem>();
        chargeParticles.Clear();
    }

    private void showVisualChargingUp() => chargeParticles.Play();
}
