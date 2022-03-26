using System;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine;

public class AI : MonoBehaviour
{
    private GameObject AI_body;
    private AI_FollowPath pathFollower;
    private PathFinding pathFinding;
    private AI_LookAround lookAround;
    private AI_WanderAround wanderAround;
    private AI_Controller controller;

    [SerializeField]
    private Transform possibleDestinations, manualDestination;
    private bool occasionallyGrounded;

    private StateMachine stateMachine;

    void Awake()
    {
        pathFinding = transform.GetComponent<PathFinding>();

        AI_body = transform.GetChild(0).gameObject;
        pathFollower = AI_body.AddComponent<AI_FollowPath>();
        wanderAround = AI_body.AddComponent<AI_WanderAround>();
        lookAround = AI_body.GetComponent<AI_LookAround>();
        controller = AI_body.GetComponent<AI_Controller>();

        stateMachine = new StateMachine();
    }

    void Start()
    {
        var wander = new Wander(wanderAround, lookAround);
        var seekDestination = new SeekDestination(this, pathFollower, possibleDestinations, manualDestination, lookAround);
        var attack = new AttackAggressively();
        var flee = new Flee();
        var idle = new Idle(controller);

        IState a = attack;
        transition(seekDestination, wander, () => Input.GetKeyDown(KeyCode.P) || pathFollower.journey == "ended");
        transition(wander, seekDestination, () => Input.GetKeyDown(KeyCode.O));
        transition(wander, idle, () => idle.GoodTimeToGoIdle);
        transition(idle, wander, () => idle.StopBeingIdle);

        stateMachine.SetState(wander);

        void transition(IState from, IState to, Func<bool> condition) =>
            stateMachine.AddTransition(from, to, condition);
    }

    void Update() => stateMachine.Tick();
}