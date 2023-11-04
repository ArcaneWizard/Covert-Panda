using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMeeleeWeaponData : IWeaponData
{
    public int DmgPerSwing { get; set; }
    public int SwingsPerSecond { get; set; }
}