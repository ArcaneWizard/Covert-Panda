using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChangeDirectionsAction : AIAction
{
    public ChangeDirectionsAction(AIActionType action, AIActionInfo info) : base(action, info) { }

    public override void StartExecution(AI_Controller controller)
    {
        base.StartExecution(controller);
        executeCoroutine(changeDirections());
    }

    // head towards a random position within the Action info bounds, then change directions once there
    private IEnumerator changeDirections()
    {
        float randomXPos = UnityEngine.Random.Range(Info.Bounds.x, Info.Bounds.y);
        DirX = Math.Sign(randomXPos - creature.position.x);
        Speed = CentralController.MaxSpeed;

        while ((DirX == 1 && creature.position.x < randomXPos) || (DirX == -1 && creature.position.x > randomXPos))
            yield return null;

        DirX = Info.DirX;
        Speed = CentralController.MaxSpeed;
        Finished = true;
    }
}
