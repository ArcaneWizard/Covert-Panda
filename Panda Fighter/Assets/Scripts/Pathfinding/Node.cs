using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Node
{
    public Transform transform { get; private set; }
    public Node parent;

    public float gCost;
    public float hCost;

    public float pathID;

    public Node(Transform transform)
    {
        this.transform = transform;
    }

    public float fCost => gCost + hCost;
}
