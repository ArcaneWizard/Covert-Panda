using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == Layer.DefaultGround || col.gameObject.layer == Layer.OneWayGround)
            gameObject.SetActive(false);
    }
}
