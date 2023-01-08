using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : IState
{
    private AI_WanderAround wanderAround;
    private AI_LookAround lookAround;

    public Wander(AI_WanderAround wanderAround, AI_LookAround lookAround)
    {
        this.wanderAround = wanderAround;
        this.lookAround = lookAround;
    }

    public void OnEnter() => wanderAround.StartWandering();
    public void Tick() => wanderAround.Tick();
    public void OnExit() => wanderAround.StopWandering();
}
