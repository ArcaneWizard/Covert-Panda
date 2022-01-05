using UnityEngine;
using System.Collections;

//reusable weapon methods
public static class reusableWeaponMethods
{

    public static void shootBulletInStraightLine(Vector2 aim, Transform bullet, Rigidbody2D rig, float speed)
    {
        bullet.transform.right = aim;
        rig.velocity = aim * speed;
    }

    public static void configureReusedBullet(Transform bullet, Rigidbody2D bulletRig, Transform bulletSpawnPoint, Side side)
    {
        // spawn bullet at the right place + default velocity and rotation
        bullet.position = bulletSpawnPoint.position;
        bullet.localEulerAngles = new Vector3(0, 0, bullet.localEulerAngles.z);
        bulletRig.velocity = new Vector2(0, 0);
        bulletRig.angularVelocity = 0;

        // set the layer of the bullet (does it damage the player/friendly creatures or enemy creatures)
        if (side == Side.Friendly)
            bullet.gameObject.layer = Layers.friendlyBullet;
        else
            bullet.gameObject.layer = Layers.enemyBullet;

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
        bullet.GetComponent<Bullet>().madeContact = false;
    }

    public static Transform retrieveNextBullet(CentralWeaponSystem weaponSystem)
    {
        Transform bullet = weaponSystem.GetBullet.transform;
        weaponSystem.useOneAmmo();
        return bullet;
    }


    public static void fadeOutBullet(Transform bullet, float delay, float duration, MonoBehaviour mB) =>
        mB.StartCoroutine(fadeBullet(bullet, delay, duration));

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

    public static float calculateTimeB4ReleasingWeapon(float trackingMultiplier, float trackingOffset, Vector2 aimDir) => ((-aimDir.y + 1) * trackingMultiplier + trackingOffset);

}

