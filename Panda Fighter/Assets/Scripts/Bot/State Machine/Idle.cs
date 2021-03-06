using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : IState
{
    private AI_Controller controller;
    private float unfreezeTimer;

    // how long (in seconds) the AI goes idle for. provided as a range
    private Vector2 freezeTime = new Vector2(0.4f, 3f);

    // how often (in seconds) the AI goes idle. provided as a range
    private Vector2 delayGoingIdle = new Vector2(1.5f, 5.5f);

    public bool GoodTimeToGoIdle;
    public bool StopBeingIdle;

    public Idle(AI_Controller controller)
    {
        this.controller = controller;

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

        int x = 1;
        if (!GoodTimeToGoIdle && x <= 2) 
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 1.2f));
            GoodTimeToGoIdle = controller.isGrounded && controller.isTouchingMap;
            ++x;
        }
            
        controller.StartCoroutine(updateWhenAiShouldGoIdle());
    }
}
