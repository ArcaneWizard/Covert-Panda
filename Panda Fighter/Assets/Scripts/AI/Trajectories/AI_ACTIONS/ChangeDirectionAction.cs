using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDirectionAction : AIAction
{
    public ChangeDirectionAction(AIActionType action, int direction, Vector2 speed, Vector2 timeB4Change, Vector2 changedSpeed,
        Vector2 timeB4SecondChange, Vector2 secondChangedSpeed, Vector2 jumpBounds, Vector3 trajectoryPos) :
        base(action, direction, speed, timeB4Change, changedSpeed, timeB4SecondChange, secondChangedSpeed, jumpBounds, trajectoryPos) { }

    public ChangeDirectionAction(int dir) : base(dir) { }

    
}
