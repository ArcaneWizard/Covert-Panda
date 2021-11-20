
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AI_WeaponSystem : CentralWeaponSystem
{
    // Default weapon you start with
    public override void Start()
    {
        base.Start();

        int r = UnityEngine.Random.Range(0, weapons.Count);
        startWithWeapon(weapons.Keys.ToArray()[r]);
        //startWithWeapon("Plasma Orb");
    }

    private void startWithWeapon(string weapon)
    {
        collectNewWeapon(weapon);
        selectWeapon(weapon);
    }
}

