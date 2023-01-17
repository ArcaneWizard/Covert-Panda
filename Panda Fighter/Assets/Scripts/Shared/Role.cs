using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains which side this entity is on, and which creature it is
// Directly updates the entity in the editor if a diff creature is selected

[ExecuteAlways]
public class Role : MonoBehaviour
{
    public Side side;

    [field: SerializeField] public Creatures sprites { get; private set; }
    [field: SerializeField] public CreatureColliders colliders { get; private set; }
}

public enum Side 
{
    Friendly,
    Enemy
}
 