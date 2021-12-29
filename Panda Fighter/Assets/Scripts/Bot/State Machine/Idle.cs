using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : IState
{
    private AI_Controller controller;
    private float unfreezeTimer;
    private Transform player;

    // how long (in seconds) the AI goes idle for. provided as a range
    private Vector2 freezeTime = new Vector2(0.2f, 2f);

    // how often (in seconds) the AI goes idle. provided as a range
    private Vector2 delayGoingIdle = new Vector2(2f, 6.7f);

    public bool GoodTimeToGoIdle;
    public bool StopBeingIdle;

    public Idle(AI_Controller controller, Transform player)
    {
        this.controller = controller;
        this.player = player;

        GoodTimeToGoIdle = false;
        StopBeingIdle = false;
        controller.StartCoroutine(updateWhenAiShouldGoIdle());
    }

    public void OnEnter()
    {
        unfreezeTimer = UnityEngine.Random.Range(freezeTime.x, freezeTime.y);
        controller.setDirection(0);
    }

    public void Tick()
    {
        if (unfreezeTimer > 0)
            unfreezeTimer -= Time.deltaTime;
        else
            StopBeingIdle = true;
    }

    public void OnExit()
    {
        GoodTimeToGoIdle = false;
        StopBeingIdle = false;
        controller.setDirection(UnityEngine.Random.Range(0, 2) * 2 - 1);
    }

    private IEnumerator updateWhenAiShouldGoIdle()
    {
        float delay = UnityEngine.Random.Range(delayGoingIdle.x, delayGoingIdle.y);
        yield return new WaitForSeconds(delay);
        GoodTimeToGoIdle = controller.isGrounded && controller.isTouchingMap;
        controller.StartCoroutine(updateWhenAiShouldGoIdle());
    }
}
