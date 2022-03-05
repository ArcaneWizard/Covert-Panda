using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
