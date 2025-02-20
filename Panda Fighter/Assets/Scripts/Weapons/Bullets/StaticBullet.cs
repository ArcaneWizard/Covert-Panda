using UnityEngine;

/// <summary>
/// Static bullets are still bullets (ex. instantaneous beams) which use physical colliders to detect collisions.
/// For fast bullets, inherit from RigidbodyBullets instead.
/// </summary>

[RequireComponent(typeof(Collider2D))]
public class StaticBullet : Bullet
{
    private bool isDetectingCollisions;

    public override void OnFire(Vector2 aim)
    {
        base.OnFire(aim);
        isDetectingCollisions = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!isDetectingCollisions)
            return;

        int layer = col.gameObject.layer;

        if (layer == Layer.GetHitBoxOfOpposingSide(gameObject)) {
            isDetectingCollisions = false;

            Health creatureHealth = col.transform.parent.GetComponent<Health>();
            creatureHealth.InflictDamage(runtimeBulletDamage, creature);

            var contacts = new ContactPoint2D[1];
            col.GetContacts(contacts);
            var collisionInfo = new CollisionInfo(col, contacts[0].point);
            OnCreatureCollision(collisionInfo, col.transform);
        } else if (layer == Layer.DefaultPlatform || layer == Layer.OneSidedPlatform) {
            isDetectingCollisions = false;

            var contacts = new ContactPoint2D[1];
            col.GetContacts(contacts);
            var collisionInfo = new CollisionInfo(col, contacts[0].point);
            OnMapCollision(collisionInfo);
        }
    }
}
