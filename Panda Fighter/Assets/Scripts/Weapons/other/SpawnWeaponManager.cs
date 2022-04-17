using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWeaponManager : MonoBehaviour
{
    // collection of all weapons that can be spawned in this level
    public Dictionary<string, int> AvailableWeapons {get; private set;}

    // Initializes the weapons that can be spawned in this level
    // FUTURE TO-DO: make it read off a text file (saves which list of weapons the user has chosen for a level)
    private void Awake() 
    {
        AvailableWeapons = new Dictionary<string, int>();

        if (!transform.GetChild(0))
            Debug.LogError("Need at least one weapon holder");

        Transform spawner = transform.GetChild(0).GetChild(1);
        for (int i = 1; i < spawner.childCount; i++)
            AvailableWeapons.Add(spawner.GetChild(i).GetComponent<WeaponTag>().Tag, i);
    }
}
