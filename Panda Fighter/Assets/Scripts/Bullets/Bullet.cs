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
        if (col.gameObject.layer == Layers.map)
            OnMapEnter(col.transform);
    }
}
