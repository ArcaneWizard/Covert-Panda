using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.Rendering;

public class StateMachine : MonoBehaviour
{
    private IState currentState;
    private IState lastState;

    //all transitions
    private Dictionary<System.Type, List<Transition>> transitions = new Dictionary<System.Type, List<Transition>>();

    //transitions from our current state to a specified state | called when conditions' met
    private List<Transition> currentStateTransitions = new List<Transition>();
    //transitions to a specified state | called whenever applicable
    private List<Transition> alwaysCalledTransitions = new List<Transition>();
    //empty list of transitions 
    private static List<Transition> EmptyTransitions = new List<Transition>();

    public void Tick()
    {
        var transition = GetTransition();
        if (transition != null)
        {
            Debug.Log("Transitioning from " + currentState + " to " + transition.To);
            SetState(transition.To);
        }

        currentState?.Tick();
    }

    public void SetState(IState state)
    {
        if (state == currentState)
            return;

        currentState?.OnExit();
        lastState = currentState;
        currentState = state;

        transitions.TryGetValue(currentState.GetType(), out currentStateTransitions);
        if (currentStateTransitions == null)
            currentStateTransitions = EmptyTransitions;

        currentState.OnEnter();
    }

    public IState getLastState => lastState;

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
