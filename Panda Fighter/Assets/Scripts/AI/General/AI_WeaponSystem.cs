
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
        base.InitializeWeaponSystem();
        
        string weapon = "";
        while (weapon == "" || weapon == WeaponTags.Grenades.ToString()) 
            weapon = IWeapons.Keys.ToArray()[UnityEngine.Random.Range(0, IWeapons.Count)];
        
        startWithWeapon(weapon);
    }

    private void startWithWeapon(string weapon)
    {
        collectNewWeapon(weapon);
        selectWeapon(weapon);
    }
}

