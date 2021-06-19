using System.Collections;
using System.Collections.Generic;
using System.Numerics;
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

    public void print()
    {
        Debug.LogFormat("{0}: {1}, {2}", this.direction, this.x, this.y);
    }

    public void printColored()
    {
        Debug.LogFormat("<color=cyan>{0}: {1}, {2}</color>", this.direction, this.x, this.y);
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