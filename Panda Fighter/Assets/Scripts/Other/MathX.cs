using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathX
{
    // Takes in an angle in degrees. Returns an equivalent angle between -180 and 180 degrees
    public static float StandardizeAngle(float angle)
    {
        angle = (angle + 3600) % 360;
        if (angle > 180)
            angle -= 360;

        return angle;
    }

    //  returns squared distance between 2 vectors
    public static float GetSquaredDistance(Vector2 a, Vector2 b)
    {
        return (a.x - b.x) * (a.x - b.x) + (b.x - b.y) * (b.y - a.y);
    }

}
