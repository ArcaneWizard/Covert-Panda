using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plasmaBullet : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 11)
        {
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }
    }
}
