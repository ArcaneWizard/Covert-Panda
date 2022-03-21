using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orderer : MonoBehaviour 
{
    public static void updateOrder(Transform sprite, Transform entity) 
    {
        if (sprite.GetComponent<SpriteRenderer>() == null)
            return;

        int order = sprite.GetComponent<SpriteRenderer>().sortingOrder;
        if (entity.tag == "Player")
            sprite.GetComponent<SpriteRenderer>().sortingOrder = order % 100 + 10000;
        else
            sprite.GetComponent<SpriteRenderer>().sortingOrder = order % 100 + 100 * entity.GetSiblingIndex() + 100;
    }
}