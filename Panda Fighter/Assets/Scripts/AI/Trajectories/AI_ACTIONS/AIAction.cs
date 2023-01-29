using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An AI Action takes in an Action type (what) and an Action info (how). 
// It contains methods for the AI entering, executing, or exiting out of the action.
// While an action is executing, the variables labeled outputs (ex. speed and dir)
// get updated and can be read by the main AI controller in FixedUpdate.
// Note, all derived classes must call executeCoroutine instead of StartCoroutine,
// so that if the AI exits this action, all running coroutines immediately stop.

[System.Serializable]
public class AIAction
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

    // all coroutines run by this action
    private List<IEnumerator> coroutinesRunning;

    public AIAction(AIActionType action, AIActionInfo info)
    {
        ActionType = action;
        Info = info;
        coroutinesRunning = new List<IEnumerator>();
    }

    public virtual void Begin(AI_Controller controller)
    {
        this.controller = controller;
        creature = controller.transform;
        phaseTracker = creature.GetComponent<CentralPhaseTracker>();

        ExecuteNormalJumpNow = false;
        ExecuteDoubleJumpNow = false;
        ExecuteJumpBoostNow = false;
    }

    public virtual void Run() { }

    // stop all coroutines that were run by this action
    public void Exit()
    {
        foreach (IEnumerator c in coroutinesRunning)
            controller.StopCoroutine(c);

        coroutinesRunning.Clear();
    }

    // returns a simple AI action for changing the IEnumeratorhe creature is heading in
    public static AIAction ChangeDirection(int dir)
    {
        AIActionType type = AIActionType.ChangeDirections;
        AIActionInfo info = new AIActionInfo(dir);
        return new ChangeDirectionsAction(type, info);
    }

    // returns random speed within a range, but ensures this speed is at least the minimum allowed speed
    protected float getRandomSpeed(Vector2 speedRange)
    {
        float randomSpeed = Random.Range(speedRange.x, speedRange.y);
        if (randomSpeed > -2.5f & randomSpeed < 2.5f)
            randomSpeed = 0f;
        else if (randomSpeed < CentralController.MinSpeed && randomSpeed > -CentralController.MinSpeed)
            randomSpeed = CentralController.MinSpeed * Mathf.Sign(randomSpeed);

        return randomSpeed;
    }

    // to be called instead of StartCoroutine by derived classes, so coroutines get stopped midway
    // if the action is exited
    protected void executeCoroutine(IEnumerator c)
    {
        controller.StartCoroutine(c);
        coroutinesRunning.Add(c);
    }

    protected IEnumerator changeVelocityAfterDelay(Vector2 delayRange, Vector2 velocityRange)
    {
        yield return new WaitForSeconds(Random.Range(delayRange.x, delayRange.y));

        float randomSpeed = getRandomSpeed(velocityRange);
        DirX = Info.DirX * (int)Mathf.Sign(randomSpeed);
        Speed = Mathf.Abs(randomSpeed);
    }

}