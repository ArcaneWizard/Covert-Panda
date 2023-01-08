using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [HideInInspector] public float radius;

    private int explosionDamage;
    private HashSet<int> entitiesHurt;
    private CircleCollider2D collider;

    private void Awake()
    {
        entitiesHurt = new HashSet<int>();

        if (transform.childCount > 0)
            collider = transform.GetChild(0).GetComponent<CircleCollider2D>();
    }

    // initialize explosion's radius and the explosion damage
    private void Start()
    {
        if (collider)
            collider.radius = radius;
        explosionDamage = transform.parent.GetComponent<WeaponConfiguration>().ExplosionDmg;
    }

    // check if an entity was already damaged by an explosion
    public bool wasEntityAlreadyHurt(int id) => entitiesHurt.Contains(id);

    // update which entities were already damaged by an explosion
    public void updateEntitiesHurt(int id) => entitiesHurt.Add(id);

    // updates explosion layer and enables explosion collider to damage nearby entities (their health script 
    // detects the explosion collider). turns off explosion collider after one frame. clears the list of entities 
    // damaged by the explosion
    public IEnumerator damageSurroundingEntities()
    {
        transform.GetChild(0).gameObject.layer = Layer.Explosion;

        collider.enabled = true;
        yield return new WaitForSeconds(Time.deltaTime);
        collider.enabled = false;

        entitiesHurt.Clear();
    }

    // returns the dmg the explosion should do to a given entity based on how far they are from the center of 
    // the explosion
    public int GetDamageBasedOffDistance(Transform entity)
    {
        BoxCollider2D entityCollider = entity.GetChild(0).GetComponent<BoxCollider2D>();
        Vector2 closestCollisionPoint = entityCollider.ClosestPoint(collider.transform.position);

        float squareDistance = Mathf.Pow(closestCollisionPoint.x - collider.transform.position.x, 2)
            + Mathf.Pow(closestCollisionPoint.y - collider.transform.position.y, 2);

        if (squareDistance <= radius * radius * 0.16f)
            return Mathf.RoundToInt((explosionDamage * ((squareDistance/radius/radius/-1f) + 1f)) * UnityEngine.Random.Range(0.9f, 1.0f));
        else if (squareDistance <= radius * radius * 0.64f)    
            return Mathf.RoundToInt(explosionDamage * ((squareDistance/radius/radius/-0.8f) + 1.3f));
        else if (squareDistance <= radius * radius)    
            return  Mathf.RoundToInt(explosionDamage * (-2.5f * (squareDistance/radius/radius) + 2.7f));
        else
            return 0;
    }
}
