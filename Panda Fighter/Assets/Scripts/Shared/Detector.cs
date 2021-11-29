using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    public bool detected { get; private set; }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 11)
            detected = true;
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.layer == 11)
            detected = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == 11)
            detected = false;
    }
}
