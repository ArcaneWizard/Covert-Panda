using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotAI : MonoBehaviour
{
    private Rigidbody2D rig;
    private Animator animator;
    private SpriteRenderer sR;

    private EnvironmentDetection environmentInputs = new EnvironmentDetection();

    void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        animator = transform.GetChild(0).transform.GetComponent<Animator>();
        sR = transform.GetChild(0).transform.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        //GENERAL PROCESS FOR DOING STUFF:

        //The following line rescans the environment 
        environmentInputs.scanEnvironment(transform);

        //The following line requests info about the scanned environment
        Dictionary<string, EnvInfo> info = environmentInputs.requestEnvironmentInfo();

        //To access info about a particular raycast
         info["C"].getHit();            
         info["C"].getHitPoint(); 
    }

    void Update()
    {
        //orient the bot to face the direction they're moving in
        botOrientation();

        //set bot's idle or move animation
        botAnimation();

        if (Input.GetMouseButtonDown(0))
            environmentInputs.scanEnvironment(transform);
    }

    private void botAnimation()
    {
        if (Mathf.Abs(rig.velocity.x) > 0)
            animator.SetInteger("State", 1);
        else
            animator.SetInteger("State", 0);
    }

    private void botOrientation()
    {
        if (rig.velocity.x > 0)
            sR.flipX = true;
        else if (rig.velocity.x < 0)
            sR.flipX = false;
    }
}
