using UnityEngine;

public static class Forces
{
    ///<summary> returns component of vector a on vector b </summary>
    public static float CalculateComponent2D(this Vector2 a, Vector2 b) => Vector2.Dot(a, b) / b.magnitude;

    ///<summary> returns component of vector a on vector b </summary>
    public static float CalculateComponent3D(this Vector3 a, Vector3 b) => Vector3.Dot(a, b) / b.magnitude;

    ///<summary> returns projection of vector a on vector b </summary>
    public static Vector2 CalculateProjection2D(this Vector2 a, Vector2 b) => b * CalculateComponent2D(a, b);

    ///<summary> returns projection of vector a on vector b </summary>
    public static Vector3 CalculateProjection3D(this Vector3 a, Vector3 b) => b * CalculateComponent3D(a, b);

    ///<summary> returns torque generated by a force (at the forceOrigin) on an infinite axis (rotating around the pivotPoint). </summary>
    public static float CalculateTorqueAroundInfiniteAxis2D(Vector2 force, Vector2 forceOrigin, Vector2 axisSlope, Vector2 pivotPoint)
    {
        Vector2 intersection = MathX.GetIntersectionOfTwoLines(force, axisSlope, forceOrigin, pivotPoint);
        float perpendicularForce = CalculateComponent2D(force, MathX.GetPerpendicular1(axisSlope));
        return perpendicularForce * Vector2.Distance(intersection, pivotPoint) * (intersection.x > pivotPoint.x ? 1 : -1);
    }

    ///<summary> returns torque generated by a force (at the forcePoint) on a a seesaw (specified by two endpoints and pivot point).
    /// Throws error if pivot point or forcepoint don't lie on the seesaw. </summary>
    public static float CalculateTorqueAround2DSeesaw(Vector2 force, Vector2 forcePoint, Vector2 pivotPoint, Vector2 endPoint1, Vector2 endPoint2)
    {
        if (pivotPoint.IsOnLineSegment2D(endPoint1, endPoint2))
            Debug.LogError("pivot point must lie between endpoints");

        else if (forcePoint.IsOnLineSegment2D(endPoint1, endPoint2))
            Debug.LogError("pivot point must lie between endpoints");

        return -1;
    }

    // throws error upon failing
    /*
    private static void passTestCases()
    {
        float a = CalculateTorqueAroundInfiniteAxis2D(new Vector2(3, 2).normalized, new Vector2(0, 0), Vector2.left, new Vector2(8, 2));
        if (!a.EqualTo(-2.7735f))
            Debug.LogError("Torque not precisely calculated within one thousandth of a decimal");

        float b = CalculateTorqueAroundInfiniteAxis2D(new Vector2(3, 2).normalized, new Vector2(0, 0), Vector2.left, new Vector2(8, 2));
        float c = CalculateTorqueAroundInfiniteAxis2D(new Vector2(-3, -2).normalized, new Vector2(16, 4), Vector2.left, new Vector2(8, 2));
        if (!b.EqualTo(c))
            Debug.LogError("Torque with equal, opposite diagonal forces around pivot point don't match");

        b = CalculateTorqueAroundInfiniteAxis2D(new Vector2(5, -1).normalized, new Vector2(0, 6), Vector2.right, new Vector2(10, 3));
        c = CalculateTorqueAroundInfiniteAxis2D(new Vector2(-5, 1).normalized, new Vector2(20, 0), Vector2.right, new Vector2(10, 3));
        if (!b.EqualTo(c))
            Debug.LogError("Torque with equal, opposite diagonal forces around pivot point don't match");
    }*/
}
