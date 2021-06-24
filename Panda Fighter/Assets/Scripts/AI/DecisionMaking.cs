using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionMaking : MonoBehaviour
{
    public string botState; //attack, wander, idle and maybe flee
    private Vector2 targetPos;
    public Transform target;

    public Transform player;
    private NewBotAI AI;
    private Animator animator;

    private bool doOnce;

    void Awake()
    {
        AI = transform.GetComponent<NewBotAI>();
    }

    void Start()
    {
        botState = "wander";
        StartCoroutine(repeatedlySetTargetPosition());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator repeatedlySetTargetPosition()
    {
        StartCoroutine(setTargetPosition());
        yield return new WaitForSeconds(3f);
        StartCoroutine(repeatedlySetTargetPosition());
    }

    private IEnumerator setTargetPosition()
    {
        if (botState == "attack")
            targetPos = player.position;
        else if (botState == "wander")
        {
            target.position = new Vector2(transform.position.x, transform.position.y) + Random.insideUnitCircle.normalized * Random.Range(12f, 27f);

            yield return new WaitForSeconds(0.05f);
            if (target.transform.GetComponent<PathCollider>().touchingObstacle)
                StartCoroutine(setTargetPosition());
            else
            {
                targetPos = target.position;
                AI.movementDirX = (int)Mathf.Sign(targetPos.x - transform.position.x);
            }
        }
        else if (botState == "idle")
            targetPos = transform.position;
    }
}
