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
    private Dictionary<String, List<Transition>> transitions = new Dictionary<String, List<Transition>>();

    //transitions for our current state
    private List<Transition> currentStateTransitions = new List<Transition>();

    //transitions always called
    private List<Transition> alwaysCalledTransitions = new List<Transition>();

    //empty list of transitions 
    private static List<Transition> EmptyTransitions = new List<Transition>();

    private string IStateName;

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

        transitions.TryGetValue(currentState.GetType().ToString(), out currentStateTransitions);
        if (currentStateTransitions == null)
            currentStateTransitions = EmptyTransitions;

        currentState.OnEnter();
    }

    public void AddTransition(IState from, IState to, Func<bool> condition)
    {
        IStateName = from.GetType().ToString();

        if (!transitions.TryGetValue(IStateName, out var IStateTransitions))
        {
            IStateTransitions = new List<Transition>();
            transitions[IStateName] = IStateTransitions;
        }

        IStateTransitions.Add(new Transition(to, condition));
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
