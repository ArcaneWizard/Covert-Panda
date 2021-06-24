using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCollider : MonoBehaviour
{
    [HideInInspector]
    public bool touchingObstacle;
    private List<GameObject> obstacles = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 11)
        {
            if (!obstacles.Contains(other.gameObject))
                obstacles.Add(other.gameObject);

            touchingObstacle = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 11)
        {
            if (obstacles.Contains(other.gameObject))
                obstacles.Remove(other.gameObject);

            if (obstacles.Count == 0)
                touchingObstacle = false;
        }
    }
}
