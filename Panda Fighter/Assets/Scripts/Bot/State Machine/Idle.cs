using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : IState
{
    private AI_Controller controller;
    private float unfreezeTimer;
    private int initialDirection;

    public bool GoodTimeToGoIdle;
    public bool StopBeingIdle;

    public Idle(AI_Controller controller)
    {
        this.controller = controller;

        GoodTimeToGoIdle = false;
        StopBeingIdle = false;
        controller.StartCoroutine(groundedDuringRandomChecks());
    }

    public void OnEnter()
    {
        unfreezeTimer = UnityEngine.Random.Range(1f, 3.2f);
        initialDirection = controller.dirX;
    }

    public void Tick()
    {
        controller.setDirection(0);

        if (unfreezeTimer > 0)
            unfreezeTimer -= Time.deltaTime;
        else
            StopBeingIdle = true;
    }

    public void OnExit()
    {
        GoodTimeToGoIdle = false;
        StopBeingIdle = false;

        controller.setDirection(initialDirection);
        controller.setSpeed(CentralController.maxSpeed);
    }

    private IEnumerator groundedDuringRandomChecks()
    {
        float delay = UnityEngine.Random.Range(0, 6f);
        yield return new WaitForSeconds(delay);
        GoodTimeToGoIdle = controller.isGrounded && controller.isTouchingMap;
        controller.StartCoroutine(groundedDuringRandomChecks());
    }
}
