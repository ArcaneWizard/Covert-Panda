using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotAI : MonoBehaviour
{
    private Rigidbody2D rig;
    private Animator animator;
    private SpriteRenderer sR;

    private EnvironmentDetection environmentInputs = new EnvironmentDetection();
    private Dictionary<EnvKey, EnvInfo> info;

    private float botSpeed = 4.5f;

    private EnvKey action;
    private List<EnvKey> possibleActions = new List<EnvKey>();

    private int check;
    private Vector2 endSpot;

    private float maxTimeAllowedStill = 0.3f;
    private float stillTimer = -0.01f;

    private float maxtimebetweenActions = 4.2f;
    private float actionTimer = -0.01f;

    void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        animator = transform.GetChild(0).transform.GetComponent<Animator>();
        sR = transform.GetChild(0).transform.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        figureOutWhatToDo();
    }

    //scan environment, store this info in a dictionary, and decide on a plan of action
    private void figureOutWhatToDo()
    {
        environmentInputs.scanEnvironment(transform);
        info = environmentInputs.requestEnvironmentInfo();

        actionTimer = maxtimebetweenActions;
        checkForFalls(); 
    }

    private void checkForFalls()
    {
        possibleActions.Clear();

        //the bot checks for openings in the ground to its left or right
        for (int offset = -5; offset <= 5; offset++)
        {
            EnvKey floorCheck = new EnvKey('G', offset, 0);

            if (info[floorCheck].getHitPoint().y < transform.position.y - 3f)
                possibleActions.Add(floorCheck);
        }

        //if the floor gaps are all to the left or all to the right of the bot, it heads toward them
        if (possibleActions.Count >= 1 && possibleActionsInSameDirection())
            rig.velocity = new Vector2(botSpeed * Mathf.Sign(possibleActions[0].x), rig.velocity.y);

        //if the bot has floor gaps both left and right of it, it randomly chooses a direction
        else if (possibleActions.Count >= 1)
        {
            EnvKey action = pickPossibileAction();
            endSpot = info[action].getHitPoint();
            check = 1;

            rig.velocity = new Vector2(botSpeed * Mathf.Sign(action.x), rig.velocity.y);
        }

        //if the bot spots no floor gaps, it heads towards the place with more open space / away from the wall (ie. at corners)
        else
        {
            float leftWallDistance = transform.position.x - info[new EnvKey('L', 0, 0)].getHitPoint().x;
            float rightWallDistance = info[new EnvKey('R', 0, 0)].getHitPoint().x - transform.position.x;

            rig.velocity = (rightWallDistance >= leftWallDistance) ?
                new Vector2(botSpeed, rig.velocity.y) : new Vector2(-botSpeed, rig.velocity.y);
        }
    }

    //check if all floor gaps are in the same direction relative to the bot (ex. all are to the left)
    private bool possibleActionsInSameDirection()
    {
        float dir = Mathf.Sign(possibleActions[0].x);

        foreach (EnvKey possibility in possibleActions)
        {
            if (Mathf.Sign(possibility.x) != dir)
                return false;
        }

        return true;
    }

    //choose among the possibile plans of action for the bot
    private EnvKey pickPossibileAction()
    {
        int choice = Random.Range(0, possibleActions.Count);
        return possibleActions[choice];
    }

    void Update()
    {
        botOrientation();
        botAnimation();

        botFallingSettings();
        botStillSettings();

        //make a new plan of action at least every 4.2 seconds
        if (actionTimer > 0) 
            actionTimer -= Time.deltaTime;
        else
            figureOutWhatToDo();

    }

    private void botFallingSettings()
    {
        //bot is falling -> kill horizontal velocity
        if (check == 1 && rig.velocity.y < -0.4f && rig.velocity.y > -0.8f)
        {
            check = 101;
            rig.velocity = new Vector2(0, rig.velocity.y);
            Debug.Log(endSpot.y);
        }

        //bot is falling and just about to land -> make new decision
        if (check == 101 && transform.position.y <= 1.7f + endSpot.y)
        {
            check = 0;
            figureOutWhatToDo();
        }
    }

    //if the bot is still for 0.3 seconds for whatever reason, it will make a new plan of action 
    private void botStillSettings()
    {
        if (Mathf.Abs(rig.velocity.x) <= 0.2f && Mathf.Abs(rig.velocity.y) <= 0.2f && stillTimer < 0f)
        {
            stillTimer = maxTimeAllowedStill;
            Debug.Log("timer reset");
        }

        if (stillTimer > 0f)
            stillTimer -= Time.deltaTime;

        if (stillTimer < 0f && Mathf.Abs(rig.velocity.x) <= 0.2f && Mathf.Abs(rig.velocity.y) <= 0.2f)
        {
            check = 0;
            figureOutWhatToDo();
            Debug.Log("new decision made by timer");
        }
    }

    //set bot's idle or move animation
    private void botAnimation()
    {
        if (Mathf.Abs(rig.velocity.x) > 0)
            animator.SetInteger("State", 1);
        else
            animator.SetInteger("State", 0);
    }

    //orient the bot to face the direction they're moving in
    private void botOrientation()
    {
        if (rig.velocity.x > 0)
            sR.flipX = true;
        else if (rig.velocity.x < 0)
            sR.flipX = false;
    }
}
