using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : IState
{
    // how long (in seconds) the AI goes idle for. provided as a range
    private Vector2 duration = new Vector2(0.3f, 2f);

    // how often (in seconds) the AI goes idle. provided as a range
    private Vector2 frequency = new Vector2(4f, 9.8f);

    private AI_Controller controller;

    private float timer;
    public bool NotIdle;


    public Idle(AI_Controller controller)
    {
        this.controller = controller;

        NotIdle = false;
        controller.StartCoroutine(updateWhenAiShouldGoIdle());
    }

    public void OnEnter()
    {
        timer = UnityEngine.Random.Range(duration.x, duration.y);
        controller.SetDirection(0);
    }

    public void Tick()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
        else
            NotIdle = true;
    }

    public void OnExit()
    { 
        NotIdle = false;
        controller.SetDirection(UnityEngine.Random.Range(0, 2) * 2 - 1);
    } 

    private IEnumerator updateWhenAiShouldGoIdle()
    {
        float delay = UnityEngine.Random.Range(frequency.x, frequency.y);
        yield return new WaitForSeconds(delay);
        bool canGoIdle = controller.isGrounded && controller.isTouchingMap;

        int x = 1;
        if (!canGoIdle && x <= 1) 
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 1.2f));
            canGoIdle = controller.isGrounded && controller.isTouchingMap;
            ++x;
        }
            
        controller.StartCoroutine(updateWhenAiShouldGoIdle());
    }
}
