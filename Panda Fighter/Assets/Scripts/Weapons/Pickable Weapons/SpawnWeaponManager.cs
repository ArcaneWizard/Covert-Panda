using System.Collections.Generic;

using UnityEngine;

public class SpawnWeaponManager : MonoBehaviour
{
    // collection of all weapons that can be spawned in this level
    public List<int> AvailableWeapons { private set; get; }

    // Initializes the weapons that can be spawned in this level
    // FUTURE TO-DO: make it read off a text file (saves which list of weapons the user has chosen for a level)
    void Awake()
    {
        Dictionary<Weapon, int> weaponsToInclude = new Dictionary<Weapon, int>();

        Transform spawner = transform.GetChild(0).GetChild(1);
        for (int i = 0; i < spawner.childCount; i++)
            weaponsToInclude.Add(spawner.GetChild(i).GetComponent<WeaponTag>().Tag, i);

        weaponsToInclude.Remove(Weapon.None);

        AvailableWeapons = new List<int>();
        foreach (int weaponIndex in weaponsToInclude.Values) {
            AvailableWeapons.Add(weaponIndex);
        }
    }
}
