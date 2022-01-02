using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float radius;

    private int explosionDamage;
    private HashSet<int> entitiesHurt;
    private CircleCollider2D collider;
    
    private void Awake() 
    {
        entitiesHurt = new HashSet<int>();

        if (transform.childCount > 0) 
            collider = transform.GetChild(0).GetComponent<CircleCollider2D>();
    }
    
    private void Start() 
    {
        if (collider)
            collider.radius = radius;
        explosionDamage = transform.parent.GetComponent<WeaponConfiguration>().explosionDmg;
    }

    public bool wasEntityAlreadyHurt(int id) => entitiesHurt.Contains(id);
    public void updateEntitiesHurt(int id) => entitiesHurt.Add(id);

    // enables explosion collider to damage nearby entities (their health script detects the explosion collider)
    // turns off explosion collider after one frame. clears the list of entities damaged by the explosion
    public IEnumerator damageSurroundingEntities() 
    {
        collider.enabled = true;
        yield return new WaitForSeconds(Time.deltaTime);
        collider.enabled = false;

        entitiesHurt.Clear();
    }

    // returns the dmg the explosion should do to a given entity based on how far they are from the center of 
    // the explosion
    public int damageBasedOffDistance(Transform entity) 
    {
        BoxCollider2D entityCollider = entity.GetChild(0).GetComponent<BoxCollider2D>();
        Vector2 closestCollisionPoint = entityCollider.ClosestPoint(collider.transform.position);

        float squareDistance = Mathf.Pow(closestCollisionPoint.x - collider.transform.position.x, 2) 
            + Mathf.Pow(closestCollisionPoint.y - collider.transform.position.y, 2);

        DebugGUI.debugText7 = (squareDistance).ToString();

        if (squareDistance <= 5.5f)
            return explosionDamage;
        else if (squareDistance < 10f)
            return Mathf.RoundToInt(explosionDamage * (-0.1f * squareDistance + 1.55f));
        else
            return Mathf.RoundToInt(explosionDamage * 5.4f / squareDistance);
    }

    // editor auto sets the correct explosion layers for all enemies, friendly AI and the player 
    // enemies -> enemy explosion layer
    // player/friendly ai -> friendly explosion layer
    /*private void OnValidate() {
        if (transform.childCount > 0) {
            if (gameObject.layer == Layers.enemyBullet)
                transform.GetChild(0).gameObject.layer = Layers.enemyExplosion;
            else
                transform.GetChild(0).gameObject.layer = Layers.friendlyExplosion;
        }
    }*/

}
