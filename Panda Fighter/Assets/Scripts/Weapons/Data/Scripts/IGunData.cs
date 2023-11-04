using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGunData : IWeaponData
{
    HowGunIsHeld HowGunIsHeld { get; set; }
    int StartingAmmo { get; set; }
    int ReloadDuration { get; set; }
    int BulletSpeed { get; set; }
    int MaxRangeByAI { get; set; }
}
