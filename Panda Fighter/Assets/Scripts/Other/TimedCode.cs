using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Execute a sequence of timed code with full power over the time delays in between
// This class offers an easy-to-use, effecient alternative to IEnumerators when
// a sequence of timed code ges re-executed manually quite often (*trying to avoid the overhead of StartCoroutine*)

// Other features supported:
// - Time delays can be set to UNKNOWN initially and change mid-way during execution
// - Time delays can be set to WAIT if we don't proceed until a condition is met
//   *handle this more effeciently than while loop checks + yield return null in Coroutines*
// - The code's time-based execution can be stopped early, similar to StopCoroutine
// - The code's time-based execution can loop

public class TimedCode
{
    public bool IsRunning { get; private set; } // timed code is currently running/executing
    public float TimeElapsed { get; private set; } // total time elapsed since starting to execute timed code
    private bool allowAutoLooping;

    private List<ExecutionDelay> delays; // n execution delays
    private List<Action> piecesOfCode; // n pieces of code

    private float lastTimeElapsed;
    private int index = 0;

    public TimedCode(List<ExecutionDelay> delays, List<Action> piecesOfCode)
    {
        TimeElapsed = 0f;
        this.delays = delays;
        this.piecesOfCode = piecesOfCode;

        if (delays.Count != piecesOfCode.Count)
            Debug.LogError("Must have the same number of execution delays and actions");
    }

    // start executing the timed code. Takes in an optional parameter for whether to auto loop
    public void Start(bool autoLooping = false)
    {
        IsRunning = true;
        index = 0;
        lastTimeElapsed = 0f;
        TimeElapsed = 0f;

        allowAutoLooping = autoLooping;
    }

    // stop executing the timed code early
    public void StopEarly()
    {
        IsRunning = false;
    }

    public void Update()
    {
        if (IsRunning)
            Tick();
    }

    // core logic to execute pieces of code at the right time
    void Tick()
    {
        if (index >= delays.Count)
        {
            IsRunning = false;
            return;
        }

        while (index < delays.Count && delays[index].status != ETStatus.Waiting)
        {
            // If the current execution delay has passed, execute the corresponding piece of code
            if (TimeElapsed >= delays[index].seconds + lastTimeElapsed)
            {
                piecesOfCode[index]();
                updateIndex();
            }

            // If we're finished waiting for a condition to be met, move to the next execution delay
            else if (delays[index].status == ETStatus.FinishedWaiting)
            {
                delays[index].ReEnableWaiting();
                updateIndex();
            }

            else
                break;
        }

        // Execute a piece of code if we're waiting for a conditional delay to be met 
        if (index < delays.Count && delays[index].status == ETStatus.Waiting)
            piecesOfCode[index]();

        TimeElapsed += Time.deltaTime;
    }

    // move to the next execution delay
    private void updateIndex()
    {
        lastTimeElapsed = TimeElapsed;
        index++;

        if (allowAutoLooping)
        {
            index %= delays.Count;
            if (index == 0)
            {
                TimeElapsed = 0f;
                lastTimeElapsed = 0f;
            }
        }
    }
}

// An execution delay represents the number of seconds to wait before executing
// the next piece of code (action). An execution delay can be Unknown initially and have its seconds property updated
// when known. An execution delay can also be set to Wai if a piece of code needs to run every frame until a condition
// is met. Dont forget to call StopWaiting() once the condition is finally met!

public class ExecutionDelay
{
    public static ExecutionDelay Instant = new ExecutionDelay(0, ETStatus.Constant);
    public static ExecutionDelay Unknown => new ExecutionDelay(float.MaxValue, ETStatus.Constant);
    public static ExecutionDelay Wait => new ExecutionDelay(float.MaxValue, ETStatus.Waiting);

    public float seconds;
    public ETStatus status { get; private set; }

    public ExecutionDelay(float seconds)
    {
        this.seconds = seconds;
        status = ETStatus.Constant;
    }

    public void StopWaiting()
    {
        if (status == ETStatus.Waiting)
            status = ETStatus.FinishedWaiting;
    }

    public void ReEnableWaiting()
    {
        if (status == ETStatus.FinishedWaiting)
        {
            status = ETStatus.Waiting;
            seconds = float.MaxValue;
        }
    }

    private ExecutionDelay(float seconds, ETStatus status)
    {
        this.seconds = seconds;
        this.status = status;
    }
}

// An Execution Time has 3 statuses. It can either be a constant (ex. 4 second delay),
// waiting, or finished waiting
public enum ETStatus
{
    Constant,
    Waiting,
    FinishedWaiting
}