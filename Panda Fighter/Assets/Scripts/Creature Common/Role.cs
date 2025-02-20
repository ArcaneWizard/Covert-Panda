using UnityEngine;

/// Contains which side this entity is on, and which creature it is
/// Directly updates the entity in the editor if a diff creature is selected

[ExecuteAlways]
public class Role : MonoBehaviour
{
    public Side Side;

    [field: SerializeField] public Creatures Sprites { get; private set; }
    [field: SerializeField] public CreatureColliders Colliders { get; private set; }
}

public enum Side
{
    Friendly,
    Enemy
}
