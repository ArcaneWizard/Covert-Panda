using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wShotgun : IWeapon
{
    private float goldenShotgunSpread = 12;

    private float fadeOutDelay = 0.8f;
    private float fadeOutDuration = 0.2f;

    public List<Transform> goldenShotgunBits = new List<Transform>();
    private Vector2 goldenBitsForceX = new Vector2(300, 1200);
    private Vector2 goldenBitsForceY = new Vector2(-50, 400);
    private int shotgunBitCounter = 0;

    public override IEnumerator SetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        DoAttack(aim, bullet, rig);
        yield return null;
    }

    public override void Attack(Vector2 aim, Transform bullet, Rigidbody2D bulletRig)
    {
        reusableWeaponMethods.configureReusedBullet(bullet, bulletRig, configuration.bulletSpawnPoint);
        reusableWeaponMethods.shootBulletInStraightLine(aim, bullet, bulletRig, configuration.bulletSpeed);
        shootNewBulletAtAngle(goldenShotgunSpread, aim, bullet, bulletRig);
        shootNewBulletAtAngle(-goldenShotgunSpread, aim, bullet, bulletRig);
        configuration.bulletSpawnPoint.GetComponent<ParticleSystem>().Play();
    }

    private void shootNewBulletAtAngle(float angle, Vector2 aim, Transform bullet, Rigidbody2D bulletRig)
    {
        bullet = reusableWeaponMethods.retrieveNextBullet(configuration.weaponSystem);
        bulletRig = bullet.transform.GetComponent<Rigidbody2D>();

        reusableWeaponMethods.configureReusedBullet(bullet, bulletRig, configuration.bulletSpawnPoint);
        bullet.transform.right = Quaternion.AngleAxis(angle, Vector3.forward) * aim;
        bulletRig.velocity = Quaternion.AngleAxis(angle, Vector3.forward) * aim * configuration.bulletSpeed;
    }

    /*Not being used anymore
    private void shotgunParticleExplosion(Vector2 aim)
    {
        for (int i = shotgunBitCounter; i < shotgunBitCounter + goldenShotgunBits.Count / 2; i++)
        {
            goldenShotgunBits[i].gameObject.SetActive(true);
            goldenShotgunBits[i].transform.right = aim;
            goldenShotgunBits[i].position = config.bulletSpawnPoint.position;
            goldenShotgunBits[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(
                Random.Range(goldenBitsForceX.x, goldenBitsForceX.y) * Mathf.Sign(aim.x),
                Random.Range(goldenBitsForceY.x, goldenBitsForceY.y) * Mathf.Sign(aim.y))
            );
        }

        shotgunBitCounter = (shotgunBitCounter + goldenShotgunBits.Count / 2) % goldenShotgunBits.Count;
    }*/
}
