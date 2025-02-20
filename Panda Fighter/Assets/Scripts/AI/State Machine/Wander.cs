public class Wander : IState
{
    private AI_WanderAround wanderAround;
#pragma warning disable IDE0052 // Remove unread private members
    private AI_LookAround lookAround;
#pragma warning restore IDE0052 // Remove unread private members

    public Wander(AI_WanderAround wanderAround, AI_LookAround lookAround)
    {
        this.wanderAround = wanderAround;
        this.lookAround = lookAround;
    }

    public void OnEnter() => wanderAround.StartWandering();
    public void Tick() => wanderAround.Tick();
    public void OnExit() => wanderAround.StopWandering();
}
