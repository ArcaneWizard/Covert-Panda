using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CreatureSizes
{
    public static ColliderSize Amphibow = new ColliderSize(0.092f, 1.355f, 1.001f, 3.172f);
    public static ColliderSize Angelfish = new ColliderSize(0.092f, 1.355f, 1.001f, 3.172f);
}

public class ColliderSize
{
    public float offsetX, offsetY, sizeX, sizeY;

    public ColliderSize(float offsetX, float offsetY, float sizeX, float sizeY)
    {
        this.offsetX = offsetX;
        this.offsetY = offsetY;
        this.sizeX = sizeX;
        this.sizeY = sizeY;
    }
}