using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.Rendering;

public class StateMachine : MonoBehaviour
{
    private IState currentState;

    //all transitions
    private Dictionary<Type, List<Transition>> transitions = new Dictionary<Type, List<Transition>>();

    //transitions for our current state
    private List<Transition> currentStateTransitions = new List<Transition>();

    //transitions always called
    private List<Transition> alwaysCalledTransitions = new List<Transition>();

    //empty list of transitions 
    private static List<Transition> EmptyTransitions = new List<Transition>();

    public void DoStuff()
    {
        var transition = GetTransition();
        if (transition != null)
            SetState(transition.To);

        currentState.DoStuff();
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

    public void AddTransition(IState from, IState to, Func<bool> predicate)
    {
        if (!transitions.TryGetValue(from.GetType(), out var newTransitions))
        {
            newTransitions = new List<Transition>();
            transitions[from.GetType()] = newTransitions;
        }

        newTransitions.Add(new Transition(to, predicate));
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
