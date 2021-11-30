using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProceduralAnimation : MonoBehaviour
{
    public bool slipped { get; protected set; }

    public virtual void OnEnter() { }
    public abstract void Tick();
}
