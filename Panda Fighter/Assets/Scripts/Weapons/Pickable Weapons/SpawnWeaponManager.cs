using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWeaponManager : MonoBehaviour
{
    // collection of all weapons that can be spawned in this level
    public List<int> AvailableWeapons {private set; get;}

    // Initializes the weapons that can be spawned in this level
    // FUTURE TO-DO: make it read off a text file (saves which list of weapons the user has chosen for a level)
    private void Awake()
    {
        Dictionary<Weapon, int> WeaponsToInclude = new Dictionary<Weapon, int>();

        Transform spawner = transform.GetChild(0).GetChild(1);
        for (int i = 0; i < spawner.childCount; i++)
            WeaponsToInclude.Add(spawner.GetChild(i).GetComponent<WeaponTag>().Tag, i);
        WeaponsToInclude.Remove(Weapon.None);

        AvailableWeapons = new List<int>();
        foreach (int weaponIndex in WeaponsToInclude.Values)
            AvailableWeapons.Add(weaponIndex);
    }
}
