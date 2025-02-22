using System.Collections.Generic;

using MEC;

using UnityEngine;

// Slow bullets have the option of using predictive raycast logic or physical collider collisions

public abstract class Bullet : MonoBehaviour
{
    protected WeaponConfiguration weaponConfiguration;

    /// <summary> Invoked when a bullet is fired. </summary>
    public virtual void OnFire(Vector2 aim) { }

    protected virtual void Awake()
    {
        weaponConfiguration = gameObject.transform.parent.GetComponent<WeaponConfiguration>();
    }

    /// <summary> Invoked whenever the bullet hits a creature. By default, deactivates the bullet </summary>
    protected virtual void OnCreatureCollision(CollisionInfo collisionInfo, Transform t) => Timing.RunSafeCoroutine(deactivateBullet(), gameObject);

    ///<summary> Invoked whenever the bullet collides with the map. By default, deactivates the bullet. </summary>
    protected virtual void OnMapCollision(CollisionInfo collisionInfo) => Timing.RunSafeCoroutine(deactivateBullet(), gameObject);

    /// <summary>  Multiplier applied to the bullet's damage. By default, set to 1 </summary>
    protected virtual float DamageMultiplier() => 1;

    /// <summary> Runtime bullet damage after applying damage multiplier </summary>
    protected int runtimeBulletDamage => (int)(weaponConfiguration.Damage * Mathf.Clamp(Mathf.Abs(DamageMultiplier()), 0, 5f));

    private IEnumerator<float> deactivateBullet()
    {
        var rig = transform.GetComponent<Rigidbody>();

        if (rig != null) {
            rig.linearVelocity = Vector2.zero;
        }

        yield return Timing.WaitForOneFrame;
        gameObject.SetActive(false);
    }
}