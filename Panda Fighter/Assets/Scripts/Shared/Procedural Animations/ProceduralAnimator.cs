using System;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralAnimator : MonoBehaviour
{
    public ProceduralAnimation currentProceduralAnimation { get; private set; }
    private AnimatorHandler animatorHandler;

    //all transitions
    private Dictionary<System.Type, List<Transition>> transitions = new Dictionary<System.Type, List<Transition>>();
    //transitions from our current procedural anim to a specified procedural anim
    private List<Transition> currentAnimationTransitions = new List<Transition>();
    //transitions to a specified procedural anim | called whenever applicable
    private List<Transition> alwaysCalledTransitions = new List<Transition>();

    //empty list of transitions 
    private static List<Transition> EmptyTransitions = new List<Transition>();

    public void PretendConstructor(AnimatorHandler animatorHandler)
    {
        this.animatorHandler = animatorHandler;
    }

    public void Tick()
    {
        var transition = GetTransition();

        if (transition != null && ((currentProceduralAnimation != transition.proceduralAnimation) ||
            (animatorHandler.currentAnimation != transition.animation)))
        {
            String a = (animatorHandler.currentAnimation != null)
                ? animatorHandler.currentAnimation
                : currentProceduralAnimation.GetType().ToString();

            String b = (transition.animation != null)
                ? transition.animation
                : transition.proceduralAnimation.GetType().ToString();

            DebugGUI.debugText7 = "Transitioned to " + b + " from " + a;
            DebugGUI.debugText8 = $"p: {transition.proceduralAnimation} and a: {transition.animation}";
            SetAnimation(transition.proceduralAnimation, transition.animation);
        }

        currentProceduralAnimation?.Tick();
    }

    public void SetAnimation(ProceduralAnimation proceduralAnimation, String animation)
    {
        animatorHandler.SetAnimation(animation);
        if (proceduralAnimation == currentProceduralAnimation)
            return;

        currentProceduralAnimation = proceduralAnimation;

        transitions.TryGetValue(currentProceduralAnimation.GetType(), out currentAnimationTransitions);
        if (currentAnimationTransitions == null)
            currentAnimationTransitions = EmptyTransitions;

        currentProceduralAnimation?.OnEnter();
    }

    public void AddTransition(ProceduralAnimation start, ProceduralAnimation end, String animationEnd,
         Func<bool> condition)
    {
        if (!transitions.TryGetValue(start.GetType(), out var startAnimationTransitions))
        {
            startAnimationTransitions = new List<Transition>();
            transitions[start.GetType()] = startAnimationTransitions;
        }

        startAnimationTransitions.Add(new Transition(end, animationEnd, condition));
    }


    public void AddAlwaysCalledTransition(ProceduralAnimation procedural, String animation, Func<bool> predicate)
    {
        alwaysCalledTransitions.Add(new Transition(procedural, animation, predicate));
    }

    private class Transition
    {
        public Func<bool> Condition { get; }
        public ProceduralAnimation proceduralAnimation { get; }
        public String animation { get; }

        public Transition(ProceduralAnimation proceduralAnimation, String animation, Func<bool> condition)
        {
            this.proceduralAnimation = proceduralAnimation;
            this.animation = animation;
            Condition = condition;
        }
    }

    //returns a transition if the transition condition is met
    private Transition GetTransition()
    {
        foreach (var transition in alwaysCalledTransitions)
            if (transition.Condition())
                return transition;

        foreach (var transition in currentAnimationTransitions)
            if (transition.Condition())
                return transition;

        return null;
    }
}

