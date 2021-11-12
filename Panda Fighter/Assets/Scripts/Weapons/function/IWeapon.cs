using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IWeapon : MonoBehaviour
{
    public virtual void SetDefaultAnimation() { return; }
    public abstract IEnumerator SetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig);
    public abstract void Attack(Vector2 aim, Transform bullet, Rigidbody2D rig);
    public virtual IEnumerator BonusSetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig) { yield return null; }
    public virtual void BonusAttack(Vector2 aim, Transform bullet, Rigidbody2D rig) { return; }

    [HideInInspector] public string attackProgress { get; private set; }
    [HideInInspector] public WeaponConfig config;

    public void DoSetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        attackProgress = "started";
        StartCoroutine(SetupAttack(aim, bullet, rig));
    }

    public void DoAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        Attack(aim, bullet, rig);
        config.shooting.updateWeaponHeldForHandheldWeapons();
        attackProgress = "finished";
    }

    public void DoBonusSetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        attackProgress = "started";
        StartCoroutine(BonusSetupAttack(aim, bullet, rig));
    }

    public void DoBonusAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        BonusAttack(aim, bullet, rig);
        attackProgress = "finished";
    }

    public virtual void Awake()
    {
        config = transform.GetComponent<WeaponConfig>();
        attackProgress = "finished";
    }
}

//reusable weapon methods
public static class reusableWeaponMethods
{

    public static void shootBulletInStraightLine(Vector2 aim, Transform bullet, Rigidbody2D rig, float speed)
    {
        bullet.transform.right = aim;
        rig.velocity = aim * speed;
    }

    public static void configureReusedBullet(Transform bullet, Rigidbody2D bulletRig, Transform bulletSpawnPoint)
    {
        //spawn bullet at the right place + default velocity and rotation
        bullet.position = bulletSpawnPoint.position;
        bullet.GetComponent<Collider2D>().isTrigger = false;

        bullet.localEulerAngles = new Vector3(0, 0, bullet.transform.localEulerAngles.z);
        bulletRig.velocity = new Vector2(0, 0);
        bulletRig.angularVelocity = 0;

        //reinitiate the OnEnable method of the bullet (where explosions/effects/traits get reset)
        bullet.gameObject.SetActive(false);
        bullet.gameObject.SetActive(true);
    }

    public static Transform retrieveNextBullet(CentralWeaponSystem weaponSystem)
    {
        Transform bullet = weaponSystem.getWeapon().transform;
        weaponSystem.useOneAmmo();
        return bullet;
    }

    public static float calculateTimeB4ReleasingWeapon(float trackingMultiplier, float trackingOffset, Vector2 aimDir) => ((-aimDir.y + 1) * trackingMultiplier + trackingOffset);

}




