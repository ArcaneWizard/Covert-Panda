using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProceduralAnimation : MonoBehaviour
{
    public virtual void OnEnter() { }
    public abstract void Tick();
}
