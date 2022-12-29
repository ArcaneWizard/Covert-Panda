using System.Collections;
using UnityEngine;

// This class houses methods to shoot a bullet for almost any weapon and throw almost any type of grenade.
// Since bullets and grenades are recycled, they first need to be configured correcty (kill existing velocity,
// spawn at gun's bullet spawnpoint, re-enable bullet collider, etc.) and then a physics force or velocity can
// be applied to move them as desired 

public static class WeaponAction
{
    // spawns in a new bullet and shoots it in straight line forward
    public static Transform ShootBulletForward(Vector2 aim, CentralWeaponSystem weaponSystem,
        WeaponConfiguration configuration, Side side, bool isBulletSticky)
    {
        Transform bullet = weaponSystem.UseOneBullet().transform;
        Rigidbody2D bulletRig = bullet.GetComponent<Rigidbody2D>();
        ConfigureBullet(bullet, bulletRig, configuration.BulletSpawnPoint, side);

        bullet.right = aim;
        bulletRig.velocity = aim * configuration.BulletSpeed;

        predictTrajectoryOfFastBullets(bullet, aim, false, isBulletSticky);
        return bullet;
    }

    // spawns in a new bullet and shoots it a specified angle off from the specified aim direction
    public static Transform ShootBulletDiagonally(Vector2 aim, float angleOffset,
        Vector2 verticalOffsetRange, CentralWeaponSystem weaponSystem,
        WeaponConfiguration configuration, Side side, bool isBulletSticky)
    {
        Transform bullet = weaponSystem.UseOneBullet().transform;
        Rigidbody2D bulletRig = bullet.GetComponent<Rigidbody2D>();
        ConfigureBullet(bullet, bulletRig, configuration.BulletSpawnPoint, side);

        Vector2 newAim = Quaternion.AngleAxis(angleOffset, Vector3.forward) * aim;
        bullet.position += Vector3.up * UnityEngine.Random.Range(verticalOffsetRange.x, verticalOffsetRange.y);
        bullet.right = newAim;
        bulletRig.velocity = newAim * configuration.BulletSpeed;

        predictTrajectoryOfFastBullets(bullet, newAim, false, isBulletSticky);
        return bullet;
    }

    // spawns in a new bullet and shoots it in an arc
    public static Transform ShootBulletInArc(Vector2 aim, CentralWeaponSystem weaponSystem,
        WeaponConfiguration configuration, Side side, bool isBulletSticky)
    {
        Transform bullet = weaponSystem.UseOneBullet().transform;
        Rigidbody2D bulletRig = bullet.GetComponent<Rigidbody2D>();
        ConfigureBullet(bullet, bulletRig, configuration.BulletSpawnPoint, side);

        bullet.right = aim;
        Vector2 unadjustedForce = configuration.BulletSpeed * 40 * aim;
        bulletRig.AddForce(unadjustedForce * bulletRig.mass);

        predictTrajectoryOfFastBullets(bullet, aim, true, isBulletSticky);
        return bullet;
    }

    // spawns in a new bullet that is still (doesn't shoot it)
    public static Transform SpawnBullet(Vector2 aim, CentralWeaponSystem weaponSystem,
        WeaponConfiguration configuration, Side side, bool isBulletSticky)

    {
        Transform bullet = weaponSystem.UseOneBullet().transform;
        Rigidbody2D bulletRig = bullet.GetComponent<Rigidbody2D>();
        ConfigureBullet(bullet, bulletRig, configuration.BulletSpawnPoint, side);

        bullet.right = aim;
        return bullet;
    }

    // spawns in a new grenade and throws it in an arc
    public static Transform ThrowGrenade(Vector2 aim, CentralGrenadeSystem grenadeSystem,
        WeaponConfiguration configuration, Side side, bool isGrenadeSticky)
    {
        Transform grenade = grenadeSystem.UseOneGrenade();
        Rigidbody2D grenadeRig = grenade.GetComponent<Rigidbody2D>();
        ConfigureBullet(grenade, grenadeRig, configuration.BulletSpawnPoint, side);

        grenade.transform.right = aim;
        Vector2 unadjustedForce = configuration.BulletSpeed * 40 * aim;
        grenadeRig.AddForce(unadjustedForce * grenadeRig.mass);

        predictTrajectoryOfFastBullets(grenade, aim, true, isGrenadeSticky);
        return grenade;
    }

    // spawns in a new greande that is still (doesn't thorw it)
    public static Transform SpawnGrenade(Vector2 aim, CentralGrenadeSystem grenadeSystem,
        WeaponConfiguration configuration, Side side, bool isBulletSticky)

    {
        Transform grenade = grenadeSystem.UseOneGrenade();
        Rigidbody2D grenadeRig = grenade.GetComponent<Rigidbody2D>();
        ConfigureBullet(grenade, grenadeRig, configuration.BulletSpawnPoint, side);

        grenade.right = aim;
        return grenade;
    }

    // fades out a bullet after a specified delay. The fading lasts a specified duration. 
    public static void FadeOutBullet(Transform bullet, float delay, float duration, MonoBehaviour mB) =>
        mB.StartCoroutine(fadeBullet(bullet, delay, duration));

    // calculate how long the creature's grenade throwing animation should play before the grenade is released.
    // Models a slope-intercept equation based off the specified aim direction 
    public static float CalculateTimeB4ReleasingGrenade(float trackingMultiplier, float trackingOffset, Vector2 aim)
        => ((-aim.y + 1) * trackingMultiplier + trackingOffset);

    private static void ConfigureBullet(Transform bullet, Rigidbody2D bulletRig, Transform bulletSpawnPoint, Side side)
    {
        // spawn bullet at the right place + default velocity and rotation
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.localEulerAngles = new Vector3(0, 0, bullet.localEulerAngles.z);
        bulletRig.velocity = new Vector2(0, 0);
        bulletRig.angularVelocity = 0;

        // set the layer of the bullet (does it damage the player/friendly creatures or enemy creatures)
        if (side == Side.Friendly)
            bullet.gameObject.layer = Layer.FriendlyBullet;
        else
            bullet.gameObject.layer = Layer.EnemyBullet;

        // reenable collider
        bullet.GetComponent<Collider2D>().enabled = true;

        // reset the bullet sprite to be opaque (ie. alpha = 1)
        if (bullet.GetComponent<SpriteRenderer>())
        {
            Color color = bullet.GetComponent<SpriteRenderer>().color;
            bullet.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 1);
        }

        // reinitiate the OnEnable method of the bullet (where variables get reset)
        bullet.gameObject.SetActive(false);
        bullet.gameObject.SetActive(true);

        // reset the trail renderer and particle effect, if any
        if (bullet.GetComponent<TrailRenderer>())
            bullet.GetComponent<TrailRenderer>().Clear();

        if (bullet.GetComponent<ParticleSystem>())
            bullet.GetComponent<ParticleSystem>().Clear();

        // reset that the bullet can do damage
        bullet.GetComponent<Bullet>().disableCollisionDetection = false;
    }

    // predicative logic ensures that super fast bullets don't pass through matter!
    private static void predictTrajectoryOfFastBullets(Transform bullet, Vector2 aim, bool updateBulletDirContinuously, bool stickyBullet) 
    {
        if (bullet.parent.GetComponent<WeaponConfiguration>().BulletSpeed > 55f)
            bullet.transform.GetComponent<Bullet>().RunPredictiveLogic(aim, bullet.position, updateBulletDirContinuously, stickyBullet);
    }

    private static IEnumerator fadeBullet(Transform bullet, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        float alphaDissipateRate = 0.1f / duration;
        SpriteRenderer sR = bullet.transform.GetComponent<SpriteRenderer>();

        while (duration > 0 && bullet.gameObject.activeSelf)
        {
            duration -= 0.1f;
            sR.color = new Color(sR.color.r, sR.color.g, sR.color.b, sR.color.a - alphaDissipateRate);
            yield return new WaitForSeconds(0.1f);
        }
    }
}

