using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpamFireGunData : IGunData
{
    public int DmgPerShot { get; set; }
    public int ShotsPerSecond { get; set; }
}
