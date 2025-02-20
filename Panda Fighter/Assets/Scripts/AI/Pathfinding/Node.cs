using UnityEngine;

public class Node
{
    public Transform Transform { get; private set; }
    public Node Parent;

    public float GCost;
    public float HCost;

    public float PathId;

    public Node(Transform transform)
    {
        Transform = transform;
    }

    public float FCost => GCost + HCost;
}
