using System;

using UnityEngine;

public class AI : MonoBehaviour
{
    private GameObject aiBody;
    private AI_FollowPath pathFollower;
    private AI_LookAround lookAround;
    private AI_WanderAround wanderAround;
    private AI_Controller controller;

    [SerializeField]
    private Transform possibleDestinations, manualDestination;

    private StateMachine stateMachine;

    void Awake()
    {
        aiBody = transform.GetChild(0).gameObject;
        pathFollower = aiBody.AddComponent<AI_FollowPath>();
        wanderAround = aiBody.AddComponent<AI_WanderAround>();
        lookAround = aiBody.GetComponent<AI_LookAround>();
        controller = aiBody.GetComponent<AI_Controller>();

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
        transition(seekDestination, wander, () => Input.GetKeyDown(KeyCode.P) || pathFollower.JourneyStatus == JourneyStatus.Ended);
        transition(wander, seekDestination, () => Input.GetKeyDown(KeyCode.O));
        //  transition(wander, idle, () => idle.GoodTimeToGoIdle);
        //  transition(idle, wander, () => idle.StopBeingIdle);

        stateMachine.SetState(wander);

        void transition(IState from, IState to, Func<bool> condition) =>
            stateMachine.AddTransition(from, to, condition);
    }

    void Update() => stateMachine.Tick();
}
