using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.Rendering;

public class StateMachine
{
    private IState currentState;

    //all transitions
    private Dictionary<System.Type, List<Transition>> transitions = new Dictionary<System.Type, List<Transition>>();
    //transitions from our current state
    private List<Transition> currentStateTransitions = new List<Transition>();
    //transitions always called when applicable
    private List<Transition> alwaysCalledTransitions = new List<Transition>();

    //universal empty list of transitions (that a list above will be set to by default)
    private static List<Transition> EmptyTransitions = new List<Transition>();

    private string IStateName;

    public void Tick()
    {
        var transition = GetTransition();
        if (transition != null)
            SetState(transition.To);

        currentState?.Tick();

        Debug.Log("STATE: " + currentState);
    }

    public void SetState(IState state)
    {
        if (state == currentState)
            return;

        currentState?.OnExit();
        currentState = state;

        transitions.TryGetValue(currentState.GetType(), out currentStateTransitions);
        if (currentStateTransitions == null)
            currentStateTransitions = EmptyTransitions;

        currentState.OnEnter();
    }

    public void AddTransition(IState start, IState end, Func<bool> condition)
    {
        if (!transitions.TryGetValue(start.GetType(), out var startStateTransitions))
        {
            startStateTransitions = new List<Transition>();
            transitions[start.GetType()] = startStateTransitions;
        }

        startStateTransitions.Add(new Transition(end, condition));
    }

    public void AddAlwaysCalledTransition(IState state, Func<bool> predicate)
    {
        alwaysCalledTransitions.Add(new Transition(state, predicate));
    }

    private class Transition
    {
        public Func<bool> Condition { get; }
        public IState To { get; }

        public Transition(IState to, Func<bool> condition)
        {
            To = to;
            Condition = condition;
        }
    }

    //returns a transition if the transition condition is met
    private Transition GetTransition()
    {
        foreach (var transition in alwaysCalledTransitions)
            if (transition.Condition())
                return transition;

        foreach (var transition in currentStateTransitions)
            if (transition.Condition())
                return transition;

        return null;
    }
}
