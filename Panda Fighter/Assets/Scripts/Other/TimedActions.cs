using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedActions
{
    private float timer;

    private float[] times;
    private Action[] actions;
    public int index = 0;

    public bool isOver { get; private set; }

    public TimedActions(float[] times, Action[] actions)
    {
        timer = 0f;
        this.times = times;
        this.actions = actions;
    }

    public void StopEarly()
    {
        isOver = true;
    }

    public void Update()
    {
        if (!isOver)
            Tick();
    }

    void Tick()
    {
        if (index >= times.Length)
        {
            isOver = true;
            return;
        }

        if (timer >= times[index])
        {
            actions[index]();

            index = (index + 1) % times.Length;
            if (index == 0)
                timer = 0f;
        }

        timer += Time.deltaTime;
    }
}
