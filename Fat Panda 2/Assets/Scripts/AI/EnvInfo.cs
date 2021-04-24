using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EnvInfo
{
    Vector2 hitPoint;
    Collider2D collision;

    public EnvInfo(Vector2 hitPoint, Collider2D collision)
    {
        this.hitPoint = hitPoint;
        this.collision = collision;
    }

    public Vector2 getHitPoint()
    {
        return this.hitPoint;
    }

    public GameObject getCollision()
    {
        if (this.collision == null)
            return null;
        else
            return this.collision.gameObject;
    }
}
