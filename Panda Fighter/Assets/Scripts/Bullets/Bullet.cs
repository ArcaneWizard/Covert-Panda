using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector]
    public bool madeContact;

    public virtual void OnEntityEnter(Transform entity) { }
    public virtual void OnMapEnter(Transform map) => gameObject.SetActive(false);

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 11)
            OnMapEnter(col.transform);
    }
}
