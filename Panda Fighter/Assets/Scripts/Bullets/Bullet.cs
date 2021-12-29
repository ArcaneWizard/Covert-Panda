using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool madeContact;

    public virtual void OnEntityEnter(Transform entity) { }
    public virtual void OnMapEnter(Transform map) => gameObject.SetActive(false);

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 11)
            OnMapEnter(col.transform);
    }

    // returns a layermask allowing collisions with the map and the hit box of the opposite entity 
    // (player's/friendly AI's hit boxes for enemies and enemies' hit box for the player/friendly bots)
    protected LayerMask mapOrTarget()
    {
        return (gameObject.layer == Layers.friendlyBullet)
            ? (1 << Layers.map | 1 << Layers.enemyHitBox)
            : (1 << Layers.map | 1 << Layers.friendlyHitBox);
    }
}
