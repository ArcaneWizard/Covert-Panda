using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDirectionsAction : AIAction
{
    public ChangeDirectionsAction(AIActionType action, AIActionInfo info) : base(action, info) { }

    public override void Begin(AI_Controller controller)
    {
        base.Begin(controller);
        Finished = true;
    }
}
