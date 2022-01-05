
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
    public override void InitializeWeaponSystem()
    {
        int r = UnityEngine.Random.Range(0, IWeapons.Count);
        startWithWeapon(IWeapons.Keys.ToArray()[r]);
        
        //startWithWeapon(WeaponTags.Shielder.ToString());
    }

    private void startWithWeapon(string weapon)
    {
        collectNewWeapon(weapon);
        selectWeapon(weapon);
    }
}

