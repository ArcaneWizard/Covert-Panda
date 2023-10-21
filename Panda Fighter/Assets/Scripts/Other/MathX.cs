using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;

///<summary> Math Library with useful functions on floats, angles, vectors, line intersections, modulo, and more. </summary>
public static class MathX
{
    private const float EPSILON = 0.00001f;

    ///<summary> Returns whether two floats are equal (difference below the default threshold or both NaN). </summary>
    public static bool IsEqual(float x, float y, float threshold = EPSILON)
    {
        if (x == y)
            return true;

        return Mathf.Abs(x - y) < threshold;
    }

    ///<summary> Returns whether the two vectors are equal. Note: floats are considered equal if their difference
    /// is below the threshold. </summary>
    public static bool IsEqual(Vector2 a, Vector2 b, float threshold = EPSILON) =>
        IsEqual(a.x, b.x, threshold) && IsEqual(a.y, b.y, threshold); 

    ///<summary> Returns the non-negative answer for a % b </summary>
    public static int Modulo(int a, int b) => ((a % b) + b) % b;

    ///<summary> Takes in an angle and returns an equivalent angle between 0 and 360 degrees </summary>
    public static float ClampAngleTo360(float angle) => (angle % 360 + 360) % 360;

    ///<summary> Takes in an angle and returns an equivalent angle between -180 and 180 degrees </summary>
    public static float ClampAngleTo180(float angle)
    {
        angle = ClampAngleTo360(angle);
        if (angle >= 180)
            angle -= 360;

        return angle;
    }

    ///<summary> Returns the squared distance between 2 vectors </summary>
    public static float GetSquaredDistance(Vector2 a, Vector2 b)
    {
        Vector2 diff = a - b;
        return diff.x * diff.x + diff.y * diff.y;
    }

    ///<summary> Rotates the vector by specified angle in radians. Can optionally set the pivot point (relative to the tail of the vector) </summary> 
    public static Vector2 RotateVector(Vector2 vector, float radians, Vector2 pivot = default(Vector2)) 
    {
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        vector -= pivot; 
        vector = new Vector2(cos * vector.x - sin * vector.y, sin * vector.x + cos * vector.y); 
        vector += pivot; 
        
        return vector; 
    }

    ///<summary> Get perpendicular vector by rotating the specified vector 90 degrees clockwise </summary> 
    public static Vector2 GetPerpendicular1(Vector2 vector) => new Vector2(vector.y, -vector.x);

    ///<summary> Get perpendicular vector by rotating specified vector 90 degrees counter-clockwise </summary> 
    public static Vector2 GetPerpendicular2(Vector2 vector) => new Vector2(-vector.y, vector.x);

    ///<summary> Returns whether a/b = c/d. Will return true if both ratios are undefined </summary>
    public static bool IsEqualRatio(float a, float b, float c, float d)
    {
        if (IsEqual(b, 0) || IsEqual(d, 0))
            return IsEqual(b, d);

        return IsEqual(a / b, c / d);
    }

    ///<summary> Returns intersection point of two lines. Each line is defined by it's slope and a single point it passes through. </summary>
    public static Vector2 GetIntersectionOfTwoLines(Vector2 line1Slope, Vector2 line2Slope, Vector2 line1Point, Vector2 line2Point)
    {
        Matrix4x4 a = Matrix4x4.identity;
        a[0, 0] = -line1Slope.y;
        a[0, 1] = line1Slope.x;
        a[1, 0] = -line2Slope.y;
        a[1, 1] = line2Slope.x;
        Vector4 b = new Vector4(-line1Point.x * line1Slope.y + line1Point.y * line1Slope.x, -line2Point.x * line2Slope.y + line2Point.y * line2Slope.x);
        var solution = a.inverse * b;

        return new Vector2(solution.x, solution.y);
    }

    ///<summary> Returns whether the specified point lies on the line segment between the two specified endpoints </summary>
    public static bool IsPointOnLineSegment2D(Vector2 point, Vector2 endPoint1, Vector2 endPoint2)
    {
        float x1 = point.x - endPoint1.x;
        float y1 = point.y - endPoint1.y;
        float x2 = point.x - endPoint2.x;
        float y2 = point.y - endPoint2.y;

        return IsEqualRatio(x1, y1, x2, y2);
    }

    ///<summary> Returns whether the specified point lies on the line segment between the two specified endpoints </summary>
    public static bool IsPointOnLineSegment3D(Vector3 point, Vector3 endPoint1, Vector3 endPoint2)
    {
        float x1 = point.x - endPoint1.x;
        float y1 = point.y - endPoint1.y;
        float z1 = point.z - endPoint1.z;
        float x2 = point.x - endPoint2.x;
        float y2 = point.y - endPoint2.y;
        float z2 = point.z - endPoint2.z;

        return IsEqualRatio(x1, y1, x2, y2) && IsEqualRatio(x1, z1, x2, z2) && IsEqualRatio(y1, z1, y2, z2);
    }
}
