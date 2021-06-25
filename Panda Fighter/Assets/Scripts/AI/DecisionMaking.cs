using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionMaking : MonoBehaviour
{
    public string botState; //attack, wander, idle and maybe flee
    public Transform target;

    public Jump rightJump;
    public Jump leftJump;

    public Transform player;
    private Animator animator;
    private NewBotAI AI;

    private Vector2 targetPos;
    private bool canSetNewTarget;
    public bool testMode;

    private float timeElapsed; //time elapsed since last updating target
    private float speed;
    private float jumpDelay;
    private float newSpeed;

    private float reconsiderRightJumpTimer = 0f;
    private float reconsiderLeftJumpTimer = 0f;
    private float jumpAgainTimer = 0f;

    void Awake()
    {
        AI = transform.GetComponent<NewBotAI>();
        animator = transform.GetChild(0).transform.GetComponent<Animator>();

        canSetNewTarget = true;
        botState = "wander";

        rightJump = new Jump("null", 0f, 0f, 0f);
        leftJump = new Jump("null", 0f, 0f, 0f);
    }

    void Start()
    {
        StartCoroutine(setNewTargetPosition());
    }

    void Update()
    {
        if (botState == "wander")
            wanderMode();
    }

    private void wanderMode()
    {
        float targetDistance = Vector2.Distance(targetPos, transform.position);

        if (targetDistance < 4f)
            StartCoroutine(setNewTargetPosition());

        else if (timeElapsed > 8.0f)
            StartCoroutine(setNewTargetPosition());


        timeElapsed += Time.deltaTime;

        if (reconsiderRightJumpTimer >= 0)
            reconsiderRightJumpTimer -= Time.deltaTime;
        if (reconsiderLeftJumpTimer >= 0)
            reconsiderLeftJumpTimer -= Time.deltaTime;
        if (jumpAgainTimer >= 0)
            jumpAgainTimer -= Time.deltaTime;
    }

    public void decideWhetherToJump()
    {
        //if a right jump is possible
        if (AI.movementDirX == 1 && rightJump.getType() != "null" && AI.grounded && jumpAgainTimer <= 0f)
        {
            //the bot must jump if there is a wall in front and no hole in front to drop down in
            if (AI.wallToTheRight && Mathf.Abs(transform.position.x - AI.obstacleToTheRight.x) < 5f && AI.rightHole == Vector2.zero)
            {
                Debug.Log("must right jump");
                StartCoroutine(executeJump(rightJump));
            }

            //the bot must jump if its target position is above it and there is either a hole or wall in front
            else if (targetPos.y > transform.position.y && ((AI.wallToTheRight && Mathf.Abs(transform.position.x - AI.obstacleToTheRight.x) < 4f) || AI.rightHole != Vector2.zero))
            {
                Debug.Log("must right jump to get to target");
                StartCoroutine(executeJump(rightJump));
            }

            //the bot may choose to jump if the target position is above it but it could continue forward (no hole)
            else if (targetPos.y > transform.position.y && AI.rightHole == Vector2.zero && reconsiderRightJumpTimer <= 0f)
            {
                int r = UnityEngine.Random.Range(0, 10);

                if (r >= 5)
                {
                    Debug.Log("chose to jump");
                    StartCoroutine(executeJump(rightJump));
                }
                else
                {
                    reconsiderRightJumpTimer = 1f;

                    Debug.LogFormat("{0}, {1}, {2}, {3}, {4}", "jump it skipped on: ", rightJump.getType(), rightJump.getJumpSpeed(),
                    rightJump.getDelay(), rightJump.getMidAirSpeed());
                }
            }
        }

        //if a left jump is possible
        if (AI.movementDirX == -1 && leftJump.getType() != "null" && AI.grounded && jumpAgainTimer <= 0f)
        {
            Debug.Log("considering left jump");

            //the bot must jump if there is a wall in front and no hole in front to drop down in
            if (AI.wallToTheLeft && Mathf.Abs(transform.position.x - AI.obstacleToTheLeft.x) < 5f && AI.leftHole == Vector2.zero)
            {
                Debug.Log("must left jump");
                StartCoroutine(executeJump(leftJump));
            }

            //the bot must jump if its target position is above it and there is either a hole or wall in front
            else if (targetPos.y > transform.position.y && ((Mathf.Abs(transform.position.x - AI.obstacleToTheLeft.x) < 4f && AI.wallToTheLeft) || AI.leftHole != Vector2.zero))
            {
                Debug.Log("must left jump to get to target");
                StartCoroutine(executeJump(leftJump));
            }

            //the bot may choose to jump if the target position is above it but it could continue forward (no hole)
            else if (targetPos.y > transform.position.y && AI.leftHole == Vector2.zero && reconsiderLeftJumpTimer <= 0f)
            {
                int r = UnityEngine.Random.Range(0, 10);

                if (r >= 5)
                {
                    Debug.Log("chose to left jump");
                    StartCoroutine(executeJump(leftJump));
                }
                else
                {
                    reconsiderLeftJumpTimer = 1f;
                    Debug.LogFormat("{0}, {1}, {2}, {3}, {4}", "jump it skipped on: ", leftJump.getType(), leftJump.getJumpSpeed(),
                    leftJump.getDelay(), leftJump.getMidAirSpeed());
                }
            }
        }
    }

    //the alien executes a jump (as deemed to work with certain configuration settings)
    private IEnumerator executeJump(Jump jump)
    {
        Debug.LogFormat("{0}, {1}, {2}, {3}, {4}", "jump executed: ", jump.getType(), jump.getJumpSpeed(),
        jump.getDelay(), jump.getMidAirSpeed());

        jumpAgainTimer = 0.3f;

        speed = jump.getJumpSpeed();
        jumpDelay = jump.getDelay();
        newSpeed = jump.getMidAirSpeed();

        if (jump.getType() == "right jump" || jump.getType() == "left jump")
            AI.jump(speed);

        else if (jump.getType() == "right double jump" || jump.getType() == "left double jump")
        {
            AI.jump(speed);
            yield return new WaitForSeconds(jumpDelay);
            AI.doublejump(newSpeed, AI.movementDirX);
        }

        else
            Debug.LogError(jump.getType() + " is not a known type of jump");
    }

    //alien either jumped or fell onto some ground surface, so now determine whether to head left or right
    public IEnumerator decideMovementAfterFallingDown()
    {
        float targetHorizontalDistance = targetPos.x - transform.position.x;
        float dir = Mathf.Sign(targetHorizontalDistance);

        //if target isn't clearly left or right, pick a random direction to head in
        if (Mathf.Abs(targetHorizontalDistance) < 2f)
            dir = UnityEngine.Random.Range(0, 2) * 2 - 1;

        //if target is clearly left or right, head left or right for most of the hallways 70% of the time 
        else
        {
            int r = UnityEngine.Random.Range(0, 10);

            if (r >= 9)
                dir *= -1;
        }

        Debug.Log("deciding movement after falling down");
        yield return new WaitForSeconds(0.05f);

        //if there isn't a wall blocking the way (exception being if there's a drop down hole), head in the determined direction
        if (dir == -1 && (Mathf.Abs(transform.position.x - AI.obstacleToTheLeft.x) > 4f || !AI.wallToTheLeft || AI.leftHole != Vector2.zero))
            AI.movementDirX = -1;
        else if (Mathf.Abs(transform.position.x - AI.obstacleToTheRight.x) > 4f || !AI.wallToTheRight || AI.rightHole != Vector2.zero)
            AI.movementDirX = 1;
        else if ((Mathf.Abs(transform.position.x - AI.obstacleToTheLeft.x) > 4f || !AI.wallToTheLeft || AI.leftHole != Vector2.zero))
            AI.movementDirX = -1;
        else
            Debug.LogError("walls are blocking alien in on both sides????");
    }

    public IEnumerator setNewTargetPosition()
    {
        if (botState == "attack")
            targetPos = player.position;

        else if (botState == "wander" && !testMode)
        {
            //calculate random nearby target position
            target.position = new Vector2(transform.position.x, transform.position.y) + UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(12f, 27f);
            timeElapsed = 0;

            yield return new WaitForSeconds(0.05f);

            //regardless of target viability, bot is currently in its jump state so redo search after a short delay
            if (animator.GetBool("jumped"))
            {
                yield return new WaitForSeconds(0.3f);
                StartCoroutine(setNewTargetPosition());
            }

            //this target isn't viable -> redo search
            else if (target.transform.GetComponent<PathCollider>().touchingObstacle && canSetNewTarget)
                StartCoroutine(setNewTargetPosition());

            //this target is viable but a new target was already set very recently -> don't do anything
            else if (!canSetNewTarget)
                yield return null;

            //this target is viable -> set it as the new target
            else
            {
                Debug.Log("set new movement");
                targetPos = target.position;
                AI.movementDirX = (int)Mathf.Sign(targetPos.x - transform.position.x);

                canSetNewTarget = false;
                yield return new WaitForSeconds(0.5f);
                canSetNewTarget = true;
            }
        }

        else if (botState == "wander" && testMode)
        {
            testMode = false;
            targetPos = target.position;
            AI.movementDirX = (int)Mathf.Sign(targetPos.x - transform.position.x);
        }

        else if (botState == "idle")
            targetPos = transform.position;
    }
}
