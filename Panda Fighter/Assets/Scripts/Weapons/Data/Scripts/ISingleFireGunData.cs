using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISingleFireGunData : IGunData
{
    public int DmgPerShot { get; set; }
    public int ShotsPerSecond { get; set; }
}
