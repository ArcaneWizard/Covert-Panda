using System.Collections;
using UnityEngine;

[System.Serializable]

// An AI Action takes in an Action type (what type of action the AI should do)
// and an Action info (specifics about how the AI should execute that action). 
// There are several methods to then simulate the execution of the Action,
// which update the variables registered "outputs" below. 

public abstract class AIAction
{
    public AIActionType ActionType { get; private set; }
    public AIActionInfo Info { get; private set; }

    // returns whether the action is finished 
    public bool Finished { get; protected set; }

    // outputs
    public float Speed { get; protected set; }
    public int DirX { get; protected set; }
    public bool ExecuteNormalJumpNow;
    public bool ExecuteDoubleJumpNow;
    public bool ExecuteJumpBoostNow;

    protected AI_Controller controller;
    protected Transform creature;
    protected CentralPhaseTracker phaseTracker;

    public AIAction(AIActionType action, AIActionInfo info)
    {
        ActionType = action;
        Info = info;

        phaseTracker = creature.GetComponent<CentralPhaseTracker>();
    }

    public virtual void Begin(AI_Controller controller)
    {
        this.controller = controller;
        creature = controller.transform;

        ExecuteNormalJumpNow = false;
        ExecuteDoubleJumpNow = false;
        ExecuteJumpBoostNow = false;
    }

    public virtual void Run() { }
    
    // returns a simple AI action for changing the direction the creature is heading in
    public static AIAction ChangeDirection(int dir)
    {
        AIActionType type = AIActionType.ChangeDirections;
        AIActionInfo info = new AIActionInfo(dir);
        return new ChangeDirectionsAction(type, info);
    }

    protected float getRandomSpeed(Vector2 speedRange)
    {
        float randomSpeed = Random.Range(speedRange.x, speedRange.y);
        if (randomSpeed > -2.5f & randomSpeed < 2.5f)
            randomSpeed = 0f;
        else if (randomSpeed < 16f && randomSpeed > -16f)
            randomSpeed = 16f * Mathf.Sign(randomSpeed);

        return randomSpeed;
    }

    protected IEnumerator changeVelocityAfterDelay(Vector2 delay, Vector2 velocity)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(delay.x, delay.y));
        if (!controller.IsActionBeingExecuted(this))
            yield break;

        float randomSpeed = getRandomSpeed(velocity);
        DirX = Info.DirX * (int)Mathf.Sign(randomSpeed);
        Speed = Mathf.Abs(randomSpeed);
    }

}