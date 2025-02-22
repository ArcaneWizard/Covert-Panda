using System.Collections.Generic;

using MEC;

using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Explosion : MonoBehaviour
{
    [HideInInspector] public float Radius;

    private HashSet<Transform> entitiesHurt;
    private CircleCollider2D collider;

    private int defaultDmg; // default explosion dmg
    private Transform creator; // who created the explosion 

    /// <summary> Requires the creature who created the explosion and the explosion dmg </summary>
    public IEnumerator<float> EnableExplosion(Transform creator, int dmg)
    {
        this.defaultDmg = dmg;
        this.creator = creator;

        collider.enabled = true;
        yield return Timing.WaitForSeconds(Time.deltaTime);
        collider.enabled = false;
        entitiesHurt.Clear();
    }

    void Awake()
    {
        entitiesHurt = new HashSet<Transform>();
        collider = transform.GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        collider.gameObject.layer = Layer.Explosion;
        collider.radius = Radius;
        collider.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        int layer = col.gameObject.layer;
        if (layer == Layer.FriendlyHitBox || layer == Layer.EnemyHitBox) {
            var entity = col.transform;
            if (entitiesHurt.Contains(entity))
                return;

            var health = entity.GetComponent<Health>();
            int explosionDmg = getDamageByDistance(entity);
            health?.InflictDamage(explosionDmg, creator);
            entitiesHurt.Add(entity);
        }
    }

    // the further the entity is from the explosion's center, the lower the explosion dmg returned
    private int getDamageByDistance(Transform entity)
    {
        Collider2D hitBoxCollider = entity.GetComponent<Collider2D>();
        Vector2 closestCollisionPoint = hitBoxCollider.ClosestPoint(transform.position);

        // calculate d: the distance between the creature and the center of the explosion,
        // in units of explosion radius, squared
        float xDiff = closestCollisionPoint.x - transform.position.x;
        float yDiff = closestCollisionPoint.y - transform.position.y;
        float d = (xDiff * xDiff + yDiff * yDiff) / (Radius * Radius);

        float dmgMultiplier;
        if (d <= 0.16f)
            dmgMultiplier = -d + 1f;
        else if (d <= 0.64f)
            dmgMultiplier = -0.5f * d + 0.92f;
        else if (d <= 1)
            dmgMultiplier = -0.83f * d + 1.13f;
        else
            dmgMultiplier = 0f;

        // Self-note on math above:
        // let e = the distance between the creature and the center of the explosion in units of explosion radius
        // for e btwn [0, 0.4], dmg multiplier ranges btwn [1, 0.84]
        // for e btwn [0.4, 0.8], dmg multiplier ranges btwn [0.84, 0.60]
        // for e btwn [0.8, 1], dmg multiplier ranges btwn [0.60, 0.30]
        // for e > 1, dmg multiplier is 0

        return Mathf.RoundToInt(defaultDmg * dmgMultiplier * Random.Range(0.93f, 1.07f));
    }
}
