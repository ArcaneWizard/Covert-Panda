using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IContinousFireData : IGunData
{
   public int DmgPerSecond { get; set; }
}
