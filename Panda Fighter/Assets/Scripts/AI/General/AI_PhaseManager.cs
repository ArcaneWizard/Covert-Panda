using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_PhaseManager : CentralPhaseManager
{
    protected override bool facingRight() => body.localEulerAngles.y == 0;
}