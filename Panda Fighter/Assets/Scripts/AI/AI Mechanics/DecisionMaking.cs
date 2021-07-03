using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class DecisionMaking : MonoBehaviour
{
    public string botState; //attack, wander, idle and maybe flee
    public Transform target;

    public List<Jump> rightJumps = new List<Jump>();
    public List<Jump> leftJumps = new List<Jump>();

    public Transform player;
    private Animator animator;
    private NewBotAI AI;

    //target setting configurations
    private bool canSetNewTarget;
    public bool testMode;
    public Transform Targets;
    private float timeElapsed; //time elapsed since last updating target
    private int targetNumber = 0;
    private int prevTargetNumber = 0;

    //jump configurations
    private float speed;
    private float jumpDelay;
    private float newSpeed;

    //jump variables
    private float reconsiderJumpTimer = 0f;
    private float jumpAgainTimer = 0f;
    private bool foundHigherPlatform;

    //inputs for making decisions
    private List<Jump> availableJumps = new List<Jump>();
    private Vector2 hole;
    private Vector2 obstaclePosition;
    private bool wallInTheWay;

    //texts for debugging decisions
    public Text thoughtProcess;
    public Text jumpChosen;

    //idle + attack state variables
    private bool idle;
    private float idleChance;

    void Awake()
    {
        AI = transform.GetComponent<NewBotAI>();
        animator = transform.GetChild(0).transform.GetComponent<Animator>();

        canSetNewTarget = true;
        botState = "wander";

        rightJumps.Clear();
        leftJumps.Clear();
    }

    void Start()
    {
        StartCoroutine(wanderToNewTarget(0f));
    }

    void Update()
    {
        if (botState == "wander")
            wanderMode();

        if (botState == "idle" && AI.grounded && AI.touchingMap)
        {
            AI.movementDirX = 0;
            if (AI.grounded && AI.touchingMap && !idle)
                StartCoroutine(idleMode());
        }
    }

    //AI wanders around the map
    private void wanderMode()
    {
        float targetDistance = Vector2.Distance(target.position, transform.position);

        if (targetDistance < 3f && canSetNewTarget)
            StartCoroutine(wanderToNewTarget(0.6f));

        else if (timeElapsed > 20.0f && AI.grounded && AI.touchingMap && canSetNewTarget)
            StartCoroutine(wanderToNewTarget(0.6f));

        timeElapsed += Time.deltaTime;

        if (reconsiderJumpTimer >= 0)
            reconsiderJumpTimer -= Time.deltaTime;
        if (jumpAgainTimer >= 0)
            jumpAgainTimer -= Time.deltaTime;
    }

    //AI is idle
    private IEnumerator idleMode()
    {
        idle = true;
        thoughtProcess.text = "idle";

        yield return new WaitForSeconds(UnityEngine.Random.Range(1.5f, 3.2f));

        if (botState != "attack")
        {
            botState = "wander";
            StartCoroutine(wanderToNewTarget(0f));
            idle = false;
        }
    }

    //find a new target to wander to
    public IEnumerator wanderToNewTarget(float delay)
    {
        canSetNewTarget = false;
        yield return new WaitForSeconds(delay);

        if (testMode)
        {
            testMode = false;
            AI.movementDirX = (int)Mathf.Sign(target.position.x - transform.position.x);
            canSetNewTarget = true;
        }

        else
        {
            timeElapsed = 0;

            //bot is currently mid-air in a jump so redo search after a short delay
            if (animator.GetBool("jumped"))
                StartCoroutine(wanderToNewTarget(0.14f));

            //20% chance of going into idle state, 75% chance of staying in wander state
            idleChance = UnityEngine.Random.Range(10, 110);
            if (idleChance <= 30)
            {
                botState = "idle";
                yield return null;
            }

            //calculate random target position
            while (targetNumber == prevTargetNumber)
                targetNumber = UnityEngine.Random.Range(0, Targets.childCount);

            prevTargetNumber = targetNumber;
            target.position = Targets.transform.GetChild(targetNumber).position;

            //40% chance of switching directions immediately to find target
            int r = UnityEngine.Random.Range(0, 10);
            if (r >= 6 || AI.movementDirX == 0)
                AI.movementDirX = (int)Mathf.Sign(target.position.x - transform.position.x);

            yield return new WaitForSeconds(0.1f);
            canSetNewTarget = true;
        }

    }


    //if the bot is going to run into a wall, change directions
    private void turnAroundIfThereIsAWall()
    {
        Debug.Log("turnAround");
        if (AI.movementDirX == 1 && AI.wallToTheRight && Mathf.Abs(transform.position.x - AI.obstacleToTheRight.x) <= 3f && AI.touchingMap && AI.grounded)
        {
            thoughtProcess.text = "wall in the way so changing direction";
            AI.movementDirX = -1;
        }

        if (AI.movementDirX == -1 && AI.wallToTheLeft && Mathf.Abs(transform.position.x - AI.obstacleToTheLeft.x) <= 3f && AI.touchingMap && AI.grounded)
        {
            thoughtProcess.text = "wall in the way so changing direction";
            AI.movementDirX = 1;
        }
    }

    //if the bot can jump to another platform, decide whether it should 
    public void decideWhetherToJump()
    {
        if (AI.movementDirX == 1)
        {
            availableJumps = rightJumps;
            wallInTheWay = AI.wallToTheRight;
            hole = AI.rightHole;
            obstaclePosition = AI.obstacleToTheRight;
        }

        else if (AI.movementDirX == -1)
        {
            availableJumps = leftJumps;
            wallInTheWay = AI.wallToTheLeft;
            hole = AI.leftHole;
            obstaclePosition = AI.obstacleToTheLeft;
        }

        //if a jump is possible
        Debug.LogFormat("{0}, {1}, {2}, {3}", Mathf.Abs(AI.movementDirX) == 1, availableJumps.Count > 0, jumpAgainTimer <= 0f, AI.grounded);
        if (Mathf.Abs(AI.movementDirX) == 1 && availableJumps.Count > 0 && AI.grounded && jumpAgainTimer <= 0f)
        {
            //the bot must jump if there is a wall in front and no hole in front to drop down in
            if (wallInTheWay && Mathf.Abs(transform.position.x - obstaclePosition.x) < 7f && hole == Vector2.zero)
            {
                thoughtProcess.text = "must jump";
                StartCoroutine(executeJump(availableJumps[0]));
                return;
            }

            //the bot must jump if its target position is above it and there is either a hole or wall in front
            else if (target.position.y > transform.position.y && ((wallInTheWay && Mathf.Abs(transform.position.x - obstaclePosition.x) < 7f) || hole != Vector2.zero))
            {
                thoughtProcess.text = "must jump to get to target";
                foundHigherPlatform = false;

                for (int i = 0; i < availableJumps.Count; i++)
                {
                    //make sure the platform it jumps to is actually higher than the bot and gets the bot closer to the target 
                    if (availableJumps[i].getLandingPosition().y > transform.position.y + 1.4f &&
                     Mathf.Abs(availableJumps[i].getLandingPosition().x - target.position.x) < Mathf.Abs(transform.position.x - target.position.x))
                    {
                        StartCoroutine(executeJump(availableJumps[i]));
                        foundHigherPlatform = true;
                        return;
                    }
                }

                if (!foundHigherPlatform && hole != Vector2.zero && availableJumps[0].getLandingPosition().y > transform.position.y - 1f)
                {
                    StartCoroutine(executeJump(availableJumps[0]));
                    return;
                }
            }

            //the bot may choose to jump if the target position is above it but it has the option to continue forward (no hole in the way)
            else if (target.position.y > transform.position.y && hole == Vector2.zero && reconsiderJumpTimer <= 0f)
            {
                int r = UnityEngine.Random.Range(0, 100);

                //choose to jump 94% of the time
                if (r > 20 || r <= 13)
                {
                    thoughtProcess.text = "chose to jump";

                    //make sure the platform it jumps to is actually higher than the bot and gets the bot closer to the target 
                    for (int i = 0; i < availableJumps.Count; i++)
                    {
                        if (availableJumps[i].getLandingPosition().y > transform.position.y + 2f && availableJumps[i].getLandingPosition().y < target.position.y
                        && Mathf.Abs(availableJumps[i].getLandingPosition().x - target.position.x) < Mathf.Abs(transform.position.x - target.position.x))
                        {
                            StartCoroutine(executeJump(availableJumps[i]));
                            return;
                        }
                    }
                }

                //chose not to jump 6% of the time
                else
                {
                    reconsiderJumpTimer = 0.5f;
                    thoughtProcess.text = "chose not to jump fr";
                }
            }

            //if bot's heading downwards and sees a hole, drop down the hole 85% of the time 
            else if (target.position.y <= transform.position.y && hole != Vector2.zero)
            {
                foundHigherPlatform = false;
                int r = UnityEngine.Random.Range(0, 100);

                if (r >= 30 && r < 45)
                {
                    for (int i = 0; i < availableJumps.Count; i++)
                    {
                        //make sure the platform it jumps to is actually higher than the bot and gets the bot closer to the target 
                        if (availableJumps[i].getLandingPosition().y > transform.position.y - 3f &&
                         Mathf.Abs(availableJumps[i].getLandingPosition().x - target.position.x) < Mathf.Abs(transform.position.x - target.position.x))
                        {
                            StartCoroutine(executeJump(availableJumps[i]));
                            foundHigherPlatform = true;
                            return;
                        }
                    }
                }
            }
        }

        turnAroundIfThereIsAWall();
    }

    //alien fell onto some platform, so now figure out whether to head left or right
    public IEnumerator decideMovementAfterFallingDown()
    {
        Debug.Log("fell down");
        float targetHorizontalDistance = target.position.x - transform.position.x;
        float dir = Mathf.Sign(targetHorizontalDistance);

        //if target isn't clearly to the left or right, pick a random direction to head in
        if (Mathf.Abs(targetHorizontalDistance) < 2f)
            dir = UnityEngine.Random.Range(0, 2) * 2 - 1;
        else
            thoughtProcess.text = "heading towards target after falling down";

        yield return new WaitForSeconds(0.05f);

        //if there isn't a wall blocking the way (exception being if there's a drop down hole), head in the determined direction
        if (botState != "idle")
        {
            if (dir == -1 && (Mathf.Abs(transform.position.x - AI.obstacleToTheLeft.x) > 4f || !AI.wallToTheLeft || AI.leftHole != Vector2.zero))
                AI.movementDirX = -1;
            else if (Mathf.Abs(transform.position.x - AI.obstacleToTheRight.x) > 4f || !AI.wallToTheRight || AI.rightHole != Vector2.zero)
                AI.movementDirX = 1;
            else if ((Mathf.Abs(transform.position.x - AI.obstacleToTheLeft.x) > 4f || !AI.wallToTheLeft || AI.leftHole != Vector2.zero))
                AI.movementDirX = -1;
            else
                Debug.LogError("walls are blocking alien in on both sides????");
        }
    }


    //the alien executes a jump (with the specified configuration settings required for that jump)
    public IEnumerator executeJump(Jump jump)
    {
        jumpChosen.text = jump.getType() + ", " + jump.getJumpSpeed() + ", " + jump.getDelay() + ", " + jump.getMidAirSpeed();
        Debug.Break();

        jumpAgainTimer = 0.3f;

        speed = jump.getJumpSpeed();
        jumpDelay = jump.getDelay();
        newSpeed = jump.getMidAirSpeed();

        if (jump.getType() == "right jump" || jump.getType() == "left jump")
        {
            AI.jumpForceMultiplier = UnityEngine.Random.Range(1.03f, 1.05f);
            AI.jump(speed);
        }

        else if (jump.getType() == "right double jump" || jump.getType() == "left double jump")
        {
            AI.jump(speed);
            yield return new WaitForSeconds(jumpDelay);
            AI.doublejump(newSpeed, AI.movementDirX);
        }

        else if (jump.getType() == "right mini u-turn" || jump.getType() == "left mini u-turn")
        {
            AI.jump(speed);
            yield return new WaitForSeconds(jumpDelay);
            AI.movementDirX *= -1;
            speed += 0.4f;
        }

        else if (jump.getType() == "right u-turn" || jump.getType() == "left u-turn")
        {
            AI.jump(speed);
            yield return new WaitForSeconds(jumpDelay);
            AI.doublejump(newSpeed, -AI.movementDirX);
        }

        else
            Debug.LogError(jump.getType() + " is not a known type of jump");
    }
}
