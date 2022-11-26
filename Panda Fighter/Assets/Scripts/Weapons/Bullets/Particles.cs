using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == Layers.DefaultGround || col.gameObject.layer == Layers.OneWayGround)
            gameObject.SetActive(false);
    }
}
