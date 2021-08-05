
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
        EquipNewWeapon("Scythe", 1);
        SelectWeapon("Scythe", "meelee");
        List<Vector2> aiming = iKTracking.setIKCoordinates("Scythe");
        controller.calculateShoulderAngles(aiming);
    }
}

