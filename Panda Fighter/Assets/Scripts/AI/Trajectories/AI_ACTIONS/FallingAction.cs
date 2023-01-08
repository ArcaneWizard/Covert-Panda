using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingAction : AIAction
{
    public FallingAction(AIActionType action, int direction, Vector2 speed, Vector2 timeB4Change, Vector2 changedSpeed,
        Vector2 timeB4SecondChange, Vector2 secondChangedSpeed, Vector2 jumpBounds, Vector3 trajectoryPos) :
        base(action, direction, speed, timeB4Change, changedSpeed, timeB4SecondChange, secondChangedSpeed, jumpBounds, trajectoryPos) { }

    public FallingAction(int dir) : base(dir) { }
}
