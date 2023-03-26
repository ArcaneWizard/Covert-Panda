using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;

public static class MathX
{
    // Useful threshold for comparing differences between floats
    public static float Epsilon = 0.0001f;

    // Takes in an angle in degrees. Returns an equivalent angle between 0 and 360 degrees
    public static float ClampAngleTo360(float angle)
    {
        return (angle % 360 + 3600) % 360;
    }

    // Takes in an angle in degrees. Returns an equivalent angle between -180 and 180 degrees
    public static float ClampAngleTo180(float angle)
    {
        angle = ClampAngleTo360(angle);
        if (angle > 180)
            angle -= 360;

        return angle;
    }

    //  returns squared distance between 2 vectors
    public static float GetSquaredDistance(Vector2 a, Vector2 b)
    {
        return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);
    }

    // Rotates the vector by specified angle in radians. Can optionally specify pivot point
    public static Vector2 RotateVector(Vector2 vector, float radians, Vector2 pivot = default(Vector2)) 
    {
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        vector -= pivot; 
        vector = new Vector2(cos * vector.x - sin * vector.y, sin * vector.x + cos * vector.y); 
        vector += pivot; 

        return vector; 
    }
}
