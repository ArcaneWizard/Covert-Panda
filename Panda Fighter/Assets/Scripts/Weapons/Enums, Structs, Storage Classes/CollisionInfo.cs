using UnityEngine;

public struct CollisionInfo
{
    public Collider2D Collider { get; set; }
    public Vector3 ContactPoint { get; set; }

    public CollisionInfo(Collider2D collider, Vector3 contactPoint)
    {
        Collider = collider;
        ContactPoint = contactPoint;
    }
}