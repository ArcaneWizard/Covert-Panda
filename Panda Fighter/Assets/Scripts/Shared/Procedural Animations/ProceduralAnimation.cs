using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProceduralAnimation : MonoBehaviour
{
    public bool slipped { get; protected set; }

    public virtual void OnEnter() { }
    public abstract void Tick();

    protected int directionFacing => transform.localEulerAngles.y == 0 ? 1 : -1;
    protected int directionAngle => transform.localEulerAngles.y == 0 ? 0 : 180;
    protected bool facingRight => transform.localEulerAngles.y == 0;
}
