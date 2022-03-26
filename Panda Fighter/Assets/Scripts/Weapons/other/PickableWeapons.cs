using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableWeapons : MonoBehaviour
{
    public Dictionary<string, int> Weapons {get; private set;}

    private void Awake() 
    {
        Weapons = new Dictionary<string, int>();

        if (!transform.GetChild(0))
            Debug.LogError("Need at least one weapon holder");

        Transform weapons = transform.GetChild(0).GetChild(1);
        for (int index = 0; index < weapons.childCount; index++)
            Weapons.Add(weapons.GetChild(index).GetComponent<WeaponTag>().Tag, index);
    }
}
