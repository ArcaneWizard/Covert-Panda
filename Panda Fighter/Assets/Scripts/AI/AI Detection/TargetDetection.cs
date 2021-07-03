using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetection : MonoBehaviour
{
    public List<GameObject> obstacles = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        //touching a possible target
        if (other.gameObject.layer == 18)
        {
            if (!obstacles.Contains(other.gameObject))
                obstacles.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //leaving a potential target
        if (other.gameObject.layer == 18)
        {
            if (obstacles.Contains(other.gameObject))
                obstacles.Remove(other.gameObject);
        }
    }
}
