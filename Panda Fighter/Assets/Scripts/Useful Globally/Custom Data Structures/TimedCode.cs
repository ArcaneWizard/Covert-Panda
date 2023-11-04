using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> This class offers an effecient alternative to IEnumerators to execute code 
/// after a certain amount of time or after conditions are met </summary>

/*
   Other features supported:
   - Execution can be stopped early, similar to StopCoroutine
   - Execution can loop

   Timed Code constructor takes in list of n execution delays and list of n actions (ie. functions of code to execute)
     - let x refer to the ith execution delay, and y refer to the ith action
     - x = new ExecutionDelay(3) means wait 3 seconds before executing y
     - x = ExecutionDelay.Wait means execute y every frame until until ExecutionDelay.StopWaiting is called
     - x = ExecutionDelay.Unknown means do nothing until x is redefined
*/

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

    // stop repeating the current action every frame (if applicable)
    public void StopRepeatingEveryFrame()
    {
        if (!delays[index].RepeatEveryFrame)
            return;

        updateIndex();
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

        if (delays[index].RepeatEveryFrame)
            piecesOfCode[index]();

        while (index < delays.Count && !delays[index].RepeatEveryFrame)
        { 
            if (TimeElapsed >= delays[index].Seconds + lastTimeElapsed)
            {
                piecesOfCode[index]();
                updateIndex();
            }

            else
                break;
        }

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
// the next piece of code (action).

public class ExecutionDelay
{
    public float Seconds;
    public bool RepeatEveryFrame {get; set;}

    public static ExecutionDelay Instant = new ExecutionDelay(0);
    public static ExecutionDelay Unknown => new ExecutionDelay(float.MaxValue);
    public static ExecutionDelay Repeat => new ExecutionDelay(float.MaxValue, true);

    public ExecutionDelay(float seconds)
    {
        Seconds = seconds;
        RepeatEveryFrame = false;
    }

    private ExecutionDelay(float seconds, bool isCalledEveryFrame)
    {
        Seconds = seconds;
        RepeatEveryFrame = isCalledEveryFrame;
    }
}