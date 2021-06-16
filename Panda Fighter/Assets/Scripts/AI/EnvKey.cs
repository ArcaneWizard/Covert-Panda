using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EnvKey  
{
    public int x;
    public int y;
    public char direction;

    public EnvKey(char direction, int x, int y)
    {
        this.x = x;
        this.y = y;
        this.direction = direction;
    }
}

public struct WallChecker
{
    public bool wallNearby;
    public string floorOpening;
    public string ceilingOpening;

    public WallChecker(bool wallNearby, string floorOpening, string ceilingOpening)
    {
        this.wallNearby = wallNearby;
        this.floorOpening = floorOpening;
        this.ceilingOpening = ceilingOpening;
    }
}