using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private Vector2 targetPos;
    private bool canSetNewTarget;
    public bool testMode;

    private float timeElapsed; //time elapsed since last updating target
    private float speed;
    private float jumpDelay;
    private float newSpeed;

    private float reconsiderJumpTimer = 0f;
    private float jumpAgainTimer = 0f;

    private List<Jump> availableJumps = new List<Jump>();
    private Vector2 hole;
    private Vector2 obstaclePosition;
    private bool wallInTheWay;

    public Text thoughtProcess;
    public Text jumpChosen;

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

        if (reconsiderJumpTimer >= 0)
            reconsiderJumpTimer -= Time.deltaTime;
        if (jumpAgainTimer >= 0)
            jumpAgainTimer -= Time.deltaTime;

        changeDirectionWhenFacingWall();
    }

    //if the bot is going to run into a wall, change directions
    private void changeDirectionWhenFacingWall()
    {
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
        if (Mathf.Abs(AI.movementDirX) == 1 && availableJumps.Count > 0 && AI.grounded && jumpAgainTimer <= 0f)
        {
            //the bot must jump if there is a wall in front and no hole in front to drop down in
            if (wallInTheWay && Mathf.Abs(transform.position.x - obstaclePosition.x) < 7f && hole == Vector2.zero)
            {
                thoughtProcess.text = "must jump";
                StartCoroutine(executeJump(availableJumps[0]));
            }

            //the bot must jump if its target position is above it and there is either a hole or wall in front
            else if (targetPos.y > transform.position.y && ((wallInTheWay && Mathf.Abs(transform.position.x - obstaclePosition.x) < 7f) || hole != Vector2.zero))
            {
                thoughtProcess.text = "must jump to get to target";

                for (int i = 0; i < availableJumps.Count; i++)
                {
                    if (availableJumps[i].getLandingPosition().y > transform.position.y + 2f)
                    {
                        StartCoroutine(executeJump(availableJumps[i]));
                        break;
                    }
                }
            }

            //the bot may choose to jump if the target position is above it but it could continue forward (no hole)
            else if (targetPos.y > transform.position.y && hole == Vector2.zero && reconsiderJumpTimer <= 0f)
            {
                int r = UnityEngine.Random.Range(0, 100);

                //chose to jump
                if (r > 20 || r <= 13)
                {
                    thoughtProcess.text = "chose to jump";

                    for (int i = 0; i < availableJumps.Count; i++)
                    {
                        if (availableJumps[i].getLandingPosition().y > transform.position.y + 2f)
                        {
                            StartCoroutine(executeJump(availableJumps[i]));
                            break;
                        }
                    }
                }

                //chose not to jump
                else
                {
                    reconsiderJumpTimer = 1f;

                    thoughtProcess.text = "chose not to jump"; ;
                }
            }
        }
    }

    //the alien executes a jump (with the specified configuration settings required for that jump)
    public IEnumerator executeJump(Jump jump)
    {
        jumpChosen.text = jump.getType() + ", " + jump.getJumpSpeed() + ", " + jump.getDelay() + ", " + jump.getMidAirSpeed();
        Debug.LogError("");

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

    //alien either jumped onto or fell onto some platform, so now figure out whether to head left or right
    public IEnumerator decideMovementAfterFallingDown()
    {
        float targetHorizontalDistance = targetPos.x - transform.position.x;
        float dir = Mathf.Sign(targetHorizontalDistance);

        //if target isn't clearly to the left or right, pick a random direction to head in
        if (Mathf.Abs(targetHorizontalDistance) < 2f)
            dir = UnityEngine.Random.Range(0, 2) * 2 - 1;

        //if target is clearly to the left or right, head left or right 90% of the time 
        else
        {
            int r = UnityEngine.Random.Range(0, 10);

            if (r == 4)
                dir *= -1;
        }

        thoughtProcess.text = "decided where to head after falling down";
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
                Debug.Log("set new target");
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
