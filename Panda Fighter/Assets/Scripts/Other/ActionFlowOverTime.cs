using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Execute a bunch of code sequentially with full power over the time delays in between
// This class offers an easy-to-use, more effecient alternative to IEnumerators when
// 1) a sequence of timed actions needs to be executed with time delays in between, and
// 2) this sequence OFTEN needs to be re-executed MANUALLY (trying to avoid the overhead of StartCoroutine)

// Other features supported:
// - Time delays can set to UNKNOWN and depend on the results of computations later on
// - Code execution can be stalled by waiting until a specific condition is met
//   *handle this more effeciently than while loop checks + yield return null in Coroutines*
// - Code execution may be stopped early before completion, similar to StopCoroutine
// - Code execution may be put on auto loop

public class ActionFlowOverTime
{
    public bool isRunning { get; private set; } // timed sequence is still being executed
    private bool loopAfterFinishing;

    // the ith entry represents the time in seconds since the (i-1)th entry that must elapse
    // before executing the ith action. Assumes a starting time of 0 seconds
    private List<ExecutionDelay> times; 
    private List<Action> actions;

    private float lastTimeElapsed;
    public float timeElapsed { get; private set; }
    private int index = 0;

    public ActionFlowOverTime(List<ExecutionDelay> times, List<Action> actions)
    {
        timeElapsed = 0f;
        this.times = times;
        this.actions = actions;

        if (times.Count != actions.Count)
            Debug.LogError("Invalid Timed Actions Sequence Constructed");
    }

    public void Start(bool loopAfterFinishing = false)
    {
        isRunning = true;
        index = 0;
        lastTimeElapsed = 0f;
        timeElapsed = 0f;

        this.loopAfterFinishing = loopAfterFinishing;
    }

    public void StopEarly()
    {
        isRunning = false;
    }

    public void Update()
    {
        if (isRunning)
            Tick();
    }

    void Tick()
    {
        if (index >= times.Count)
        {
            isRunning = false;
            return;
        }

        while (index < times.Count && times[index].status != ETStatus.Waiting)
        {
            // if time elapsed goes beyond the next execution time in the sequence, execute the next corresponding action
            if (timeElapsed >= times[index].seconds + lastTimeElapsed)
            {
                actions[index]();
                updateIndex();
            }

            // if a waiting execution time is finished
            else if (times[index].status == ETStatus.FinishedWaiting)
            {
                times[index].ReEnableWaiting();
                updateIndex();
            }

            else
                break;
        }

        // Execute the action corresponding to the waiting execution time 
        if (index < times.Count && times[index].status == ETStatus.Waiting)
            actions[index]();

        timeElapsed += Time.deltaTime;
    }

    private void updateIndex()
    {
        lastTimeElapsed = timeElapsed;
        index++;

        if (loopAfterFinishing)
        {
            index %= times.Count;
            if (index == 0)
            {
                timeElapsed = 0f;
                lastTimeElapsed = 0f;
            }
        }
    }
}

// An execution time represents a 
public class ExecutionDelay
{
    public static ExecutionDelay Zero = new ExecutionDelay(0, ETStatus.Constant);
    public static ExecutionDelay Unknown => new ExecutionDelay(float.MaxValue, ETStatus.Constant);
    public static ExecutionDelay Waiting => new ExecutionDelay(float.MaxValue, ETStatus.Waiting);

    public float seconds;
    public ETStatus status { get; private set; }

    public ExecutionDelay(float seconds)
    {
        this.seconds = seconds;
        status = ETStatus.Constant;
    }

    public void StopWaiting() => status = ETStatus.FinishedWaiting;
    public void ReEnableWaiting()
    {
        status = ETStatus.Waiting;
        seconds = float.MaxValue;
    }

    private ExecutionDelay(float seconds, ETStatus status)
    {
        this.seconds = seconds;
        this.status = status;
    }
}

// An Execution Time has 3 statuses. It can either be a constant (ex. 4 second delay),
// waiting (till its finished waiting), or finished waiting
public enum ETStatus
{
    Constant,
    Waiting,
    FinishedWaiting
}