
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AI_WeaponSystem : CentralWeaponSystem
{
    // Default weapon you start with
    void Start()
    {
        int r = UnityEngine.Random.Range(1, 8);

        if (r == 1)
            startWithWeapon("Grenade", "handheld");
        else if (r == 2)
            startWithWeapon("Shielder", "gun");
        else if (r == 3)
            startWithWeapon("Boomerang", "handheld");
        else if (r == 4)
            startWithWeapon("Plasma Orb", "handheld");
        else if (r == 5)
            startWithWeapon("Scythe", "meelee");
        else if (r == 6)
            startWithWeapon("Sniper", "gun");
        else if (r == 7)
            startWithWeapon("Shotgun", "gun");
    }

    private void startWithWeapon(string weapon, string combatMode)
    {
        EquipNewWeapon(weapon, 20);
        selectWeapon(weapon, combatMode);
        List<Vector2> aiming = getWeaponConfig().config.IK_Coordinates;
        lookAround.calculateShoulderAngles(aiming);
    }
}

