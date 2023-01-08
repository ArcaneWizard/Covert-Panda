using UnityEngine;

[System.Serializable]

// An AI Action contains what the AI should do and how the AI should do it. The
// Action Type is the what, such as a double jump. The Action Info is the how,
// such as information to execute a double jump properly

public abstract class AIAction
{
    public AIActionType ActionType { get; private set; }
    public AIActionInfo Info { get; private set; }

    protected AI_Controller controller;
    protected Transform creature;

    public AIAction(AIActionType action, AIActionInfo info, AI_Controller controller)
    {
        ActionType = action;
        Info = info;

        this.controller = controller;
        creature = controller.transform;
    }

    // invoked when starting to execute this action
    public abstract void StartExecution();

    // invoked every frame when executing this action
    public abstract void Execute();

    // returns whether the action is finished executing
    public bool FinishedExecuting;

    protected float getRandomSpeed(Vector2 speedRange)
    {
        float randomSpeed = Random.Range(speedRange.x, speedRange.y);
        if (randomSpeed > -2.5f & randomSpeed < 2.5f)
            randomSpeed = 0f;
        else if (randomSpeed < 16f && randomSpeed > -16f)
            randomSpeed = 16f * Mathf.Sign(randomSpeed);

        return randomSpeed;
    }
}