using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wShotgun : IWeapon {
    
    private float shotgunBulletSpeed = 52;
    private int shotgunBitCounter = 0;
    private Vector2 goldenBitsForceX = new Vector2(-50, 70), goldenBitsForceY = new Vector2(0, 290);
    private float goldenShotgunSpread = 22;
    public List<Transform> goldenShotgunBits = new List<Transform>();

    public override void Awake() {base.Awake(); config.IK_Coordinates = AimingDir.shotgunAiming;}

    public override IEnumerator SetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        DoAttack(aim, bullet, rig);
        yield return null;
    }

    public override void Attack(Vector2 aim, Transform bullet, Rigidbody2D bulletRig) {

        //shoot three golden shotgun bullets with spread
        reusableWeaponMethods.configureReusedBullet(bullet, bulletRig, config.bulletSpawnPoint);
        reusableWeaponMethods.shootBulletInStraightLine(aim, bullet, bulletRig, shotgunBulletSpeed);
        
        bullet = reusableWeaponMethods.retrieveNextBullet(config.weaponSystem);
        bulletRig = bullet.transform.GetComponent<Rigidbody2D>();
        reusableWeaponMethods.configureReusedBullet(bullet, bulletRig, config.bulletSpawnPoint);

        bullet.transform.right = Quaternion.AngleAxis(goldenShotgunSpread, Vector3.forward) * aim;
        bulletRig.velocity = Quaternion.AngleAxis(goldenShotgunSpread, Vector3.forward) * aim * shotgunBulletSpeed;

        bullet = reusableWeaponMethods.retrieveNextBullet(config.weaponSystem);
        bulletRig = bullet.transform.GetComponent<Rigidbody2D>();
        reusableWeaponMethods.configureReusedBullet(bullet, bulletRig, config.bulletSpawnPoint);

        bullet.transform.right = Quaternion.AngleAxis(-goldenShotgunSpread, Vector3.forward) * aim;
        bulletRig.velocity = Quaternion.AngleAxis(-goldenShotgunSpread, Vector3.forward) * aim * shotgunBulletSpeed;

        //explode golden shotgun dust everywhere
        for (int i = shotgunBitCounter; i < shotgunBitCounter + goldenShotgunBits.Count / 2; i++)
        {
            goldenShotgunBits[i].gameObject.SetActive(true);
            goldenShotgunBits[i].transform.right = aim;
            goldenShotgunBits[i].position = config.bulletSpawnPoint.position;
            goldenShotgunBits[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(
                Random.Range(goldenBitsForceX.x, goldenBitsForceY.y) * Mathf.Sign(aim.x),
                Random.Range(goldenBitsForceY.x, goldenBitsForceY.y))
            );
        }

        shotgunBitCounter = (shotgunBitCounter + goldenShotgunBits.Count / 2) % goldenShotgunBits.Count;
    }
}
