using System;
using System.Collections;

using UnityEngine;

// This class houses methods to spawn and shoot a bullet for almost any type of weapon.
// Since bullets and grenades are recycled, they first need to be configured correcty (kill existing velocity,
// spawn at gun's bullet spawnpoint, re-enable bullet collider, etc.) and then a physics force or velocity can
// be applied to move them as desired 

public static class CommonWeaponBehaviours
{
    // spawns in a new bullet and shoots it in straight line forward.
    public static Transform SpawnAndShootBulletForward(Vector2 aim, CentralWeaponSystem weaponSystem,
        WeaponConfiguration configuration, Side side, Action<Transform> extraSettings = null)
    {
        // spawn bullet and shoot in straight line in specified aim direction
        PhysicalBullet bullet = SpawnBullet(aim, weaponSystem, configuration, side);
        bullet.Rig.linearVelocity = aim * configuration.Speed;

        // apply extra settings to bullet, if any are specified
        if (extraSettings != null)
            extraSettings(bullet.Transform);

        // confirm bullet was fired
        Debug.Log(bullet.Transform.name);
        bullet.Transform.GetComponent<Bullet>().OnFire(aim);

        return bullet.Transform;
    }

    // spawns in a new bullet and with a random specified vertical offset from the default bullet spawn position,
    // and shoots it a specified angle off from the default aim direction
    public static Transform SpawnAndShootBulletDiagonally(Vector2 aim, float angleOffset,
        Vector2 verticalOffsetRange, CentralWeaponSystem weaponSystem,
        WeaponConfiguration configuration, Side side, Action<Transform> extraSettings = null)
    {
        // spawn bullet and shoot in a diagonal line based on specified aim direction, angle offset, and vertical offset
        PhysicalBullet bullet = SpawnBullet(aim, weaponSystem, configuration, side);
        Vector2 newAim = Quaternion.AngleAxis(angleOffset, Vector3.forward) * aim;
        bullet.Transform.position += Vector3.up * UnityEngine.Random.Range(verticalOffsetRange.x, verticalOffsetRange.y);
        bullet.Transform.right = newAim;
        bullet.Rig.linearVelocity = newAim * configuration.Speed;

        // apply extra settings to bullet, if any are specified
        if (extraSettings != null)
            extraSettings(bullet.Transform);

        // confirm bullet was fired
        bullet.Transform.GetComponent<Bullet>().OnFire(aim);
        return bullet.Transform;
    }

    // spawns in a new bullet and shoots it in an arc
    public static Transform SpawnAndShootBulletInArc(Vector2 aim, Vector2 forceMultiplier, Vector2 forceOffset,
        CentralWeaponSystem weaponSystem, WeaponConfiguration configuration, Side side,
        Action<Transform> extraSettings = null)
    {
        // spawn bullet and shoot with specified force
        PhysicalBullet bullet = SpawnBullet(aim, weaponSystem, configuration, side);
        Vector2 unadjustedForce = configuration.Speed * 40 * aim * forceMultiplier + forceOffset;
        bullet.Rig.AddForce(unadjustedForce * bullet.Rig.mass);

        // apply extra settings to bullet, if any are specified
        if (extraSettings != null)
            extraSettings(bullet.Transform);

        // confirm bullet was fired 
        bullet.Transform.GetComponent<Bullet>().OnFire(aim);
        return bullet.Transform;
    }
    // spawns in a new bullet that is still (doesn't shoot it)
    public static PhysicalBullet SpawnBullet(Vector2 aim, CentralWeaponSystem weaponSystem,
        WeaponConfiguration configuration, Side side)

    {
        Transform bullet = weaponSystem.UseOneBullet().transform;
        Rigidbody2D bulletRig = bullet.GetComponent<Rigidbody2D>();

        // spawn bullet at the right place. it should be still with default rotation
        bullet.position = configuration.BulletSpawnPoint.position;
        bullet.localEulerAngles = new Vector3(0, 0, bullet.localEulerAngles.z);
        bulletRig.linearVelocity = new Vector2(0, 0);
        bulletRig.angularVelocity = 0;
        bullet.right = aim;

        // set the layer of the bullet (does it damage the player/friendly creatures or enemy creatures)
        if (side == Side.Friendly)
            bullet.gameObject.layer = Layer.FriendlyBullet;
        else
            bullet.gameObject.layer = Layer.EnemyBullet;

        // re-enable collider
        bullet.GetComponent<Collider2D>().enabled = true;

        // reset the bullet sprite to be opaque (ie. alpha = 1)
        if (bullet.GetComponent<SpriteRenderer>()) {
            Color color = bullet.GetComponent<SpriteRenderer>().color;
            bullet.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 1);
        }

        // reset the trail renderer and particle effect, if any
        if (bullet.GetComponent<TrailRenderer>())
            bullet.GetComponent<TrailRenderer>().Clear();

        if (bullet.GetComponent<ParticleSystem>())
            bullet.GetComponent<ParticleSystem>().Clear();

        bullet.gameObject.SetActive(true);
        return new PhysicalBullet(bullet, bulletRig);
    }

    // fades out a bullet after a specified delay. The fading lasts a specified duration. 
    public static void FadeOutBullet(Transform bullet, float delay, float duration, MonoBehaviour mB) =>
        mB.StartCoroutine(fadeBullet(bullet, delay, duration));

    // calculate how long the creature's grenade throwing animation should play before the grenade is released.
    // Models a slope-intercept equation based off the specified aim direction 
    public static float CalculateTimeB4ReleasingGrenade(float trackingMultiplier, float trackingOffset, Vector2 aim)
        => ((-aim.y + 1) * trackingMultiplier + trackingOffset);

    private static IEnumerator fadeBullet(Transform bullet, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        float alphaDissipateRate = 0.1f / duration;
        SpriteRenderer sR = bullet.GetComponent<SpriteRenderer>();

        while (duration > 0 && bullet.gameObject.activeSelf) {
            duration -= 0.1f;
            sR.color = new Color(sR.color.r, sR.color.g, sR.color.b, sR.color.a - alphaDissipateRate);
            yield return new WaitForSeconds(0.1f);
        }
    }
}

public struct PhysicalBullet
{
    public Transform Transform;
    public Rigidbody2D Rig;

    public PhysicalBullet(Transform transform, Rigidbody2D rig)
    {
        this.Transform = transform;
        this.Rig = rig;
    }
}