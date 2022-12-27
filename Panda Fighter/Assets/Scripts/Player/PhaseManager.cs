using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : CentralPhaseManager
{
    protected override bool facingRight() => 
        Input.mousePosition.x >= camera.WorldToScreenPoint(controller.shootingArm.parent.position).x;
}
