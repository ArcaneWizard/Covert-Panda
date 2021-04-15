using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EnvInfo 
{
    Vector2 hitPoint;
    bool hit;

    public EnvInfo(Vector2 hitPoint, bool hit)
    {
        this.hitPoint = hitPoint;
        this.hit = hit;
    }

    public Vector2 getHitPoint()
    {
        return this.hitPoint;
    }

    public bool getHit()
    {
        return this.hit;
    }
}
