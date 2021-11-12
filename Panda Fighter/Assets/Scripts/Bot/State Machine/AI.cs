using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    private GameObject AI_body;
    private AI_FollowPath pathFollower;
    private PathFinding pathFinding;
    private AI_WanderAround wanderAround;

    [SerializeField]
    private Transform possibleDestinations, manualDestination;

    private StateMachine stateMachine;

    void Awake()
    {
        pathFinding = transform.GetComponent<PathFinding>();

        AI_body = transform.GetChild(0).gameObject;
        pathFollower = AI_body.AddComponent<AI_FollowPath>();
        wanderAround = AI_body.AddComponent<AI_WanderAround>();

        stateMachine = new StateMachine();
    }

    void Start()
    {
        var wander = new Wander(wanderAround);
        var seekDestination = new SeekDestination(this, pathFollower, possibleDestinations, manualDestination);
        var attack = new AttackAggressively();
        var flee = new Flee();
        var idle = new Idle();

        transition(seekDestination, wander, () => Input.GetKeyDown(KeyCode.P) || pathFollower.journey == "ended");
        transition(wander, seekDestination, () => Input.GetKeyDown(KeyCode.O));

        stateMachine.SetState(seekDestination);

        void transition(IState from, IState to, Func<bool> condition) =>
            stateMachine.AddTransition(from, to, condition);
    }

    void Update()
    {
        stateMachine.Tick();
    }
}