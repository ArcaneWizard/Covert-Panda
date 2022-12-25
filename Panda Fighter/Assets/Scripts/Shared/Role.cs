using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains which side this entity is on, and which creature it is
// Directly updates the entity in the editor if a diff creature is selected

[ExecuteAlways]
public class Role : MonoBehaviour
{
    public Side side;
    public Creatures creature;

    void OnAwake() 
    {
        transform.GetChild(0).GetChild(0).GetComponent<LimbCollection>().creature = creature;
        transform.GetChild(0).GetChild(0).GetComponent<LimbCollection>().updateLimbs();
    }

#if (UNITY_EDITOR)
    void OnValidate() {
        transform.GetChild(0).GetChild(0).GetComponent<LimbCollection>().creature = creature;
        transform.GetChild(0).GetChild(0).GetComponent<LimbCollection>().updateLimbs();

        transform.name = creature.ToString().Split(' ')[0];
    }
#endif
}

public enum Side 
{
    Friendly,
    Enemy
}
 