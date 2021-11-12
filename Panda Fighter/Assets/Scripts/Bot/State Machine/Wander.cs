using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : IState
{
    private AI_WanderAround wanderAround;

    public Wander(AI_WanderAround wanderAround) => this.wanderAround = wanderAround;

    public void OnEnter() => wanderAround.startWandering();
    public void Tick() => wanderAround.tick();
    public void OnExit() => wanderAround.stopWandering();
}
