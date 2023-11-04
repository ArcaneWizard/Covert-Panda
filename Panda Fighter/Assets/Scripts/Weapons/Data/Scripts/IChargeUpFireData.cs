using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChargeUpFireData : IGunData
{
    public int DmgPerShot { get; set; }
    public int ChargeUpDuration { get; set; }
    public int ShotDuration { get; set; }
}
