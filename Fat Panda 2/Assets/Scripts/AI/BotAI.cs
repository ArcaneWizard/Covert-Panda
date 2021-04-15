using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotAI : MonoBehaviour
{
    private Rigidbody2D rig;
    private Animator animator;
    private SpriteRenderer sR;
    private Transform leftFoot;
    private Transform rightFoot;

    private LayerMask map = (1 << 11);

    private EnvironmentDetection environmentInputs = new EnvironmentDetection();
    private FixedBotBehaviour fixedBotBehaviour = new FixedBotBehaviour();
    private Dictionary<EnvKey, EnvInfo> info;

    private float botSpeed = 5f;
    private float jumpForce = 420;

    private EnvKey action;
    private List<EnvKey> possibleActions = new List<EnvKey>();

    private int check = 0;

    private float stillTimer = -0.01f;

    private float maxtimebetweenActions = 4.2f;
    private float actionTimer = -0.01f;

    //private variables used between methods
    private Vector2 endSpot;
    private float jumpCoordinate;
    private EnvKey ceilingCheckAboveJump;
    private bool dontJump;

    void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        animator = transform.GetChild(0).transform.GetComponent<Animator>();
        sR = transform.GetChild(0).transform.GetComponent<SpriteRenderer>();
        leftFoot = transform.GetChild(1);
        rightFoot = transform.GetChild(2);

        botSpeed = Random.Range(4.3f, 5f);
    }

    void Start()
    {
        figureOutWhatToDo();
        StartCoroutine(constantWallChecks());
    }

    //scan environment, store this info in a dictionary, and decide on a plan of action
    private void figureOutWhatToDo()
    {
        environmentInputs.scanEnvironment(transform);
        info = environmentInputs.requestEnvironmentInfo();

        actionTimer = maxtimebetweenActions;
        pickPossiblePath(); 
    }

    //bot makes a plan of action (determines place to reach) and starts moving 
    private void pickPossiblePath()
    {
        //resest possibleActions list
        possibleActions.Clear();

        //get x position of closest left and right wall
        float rightWallLocation = info[new EnvKey('R', 0, 0)].getHitPoint().x;
        float leftWallLocation = info[new EnvKey('L', 0, 0)].getHitPoint().x;

        //the bot checks for flat surfaces at the same level or below it
        for (int offset = -5; offset <= 5; offset++)
        {
            EnvKey floorCheck = new EnvKey('G', offset, 0);

            //make sure this flat surface is not behind a wall (as the bot can't just walk straight to it then)
            if ((offset < 0 && info[floorCheck].getHitPoint().x > leftWallLocation) ||
                (offset > 0 && info[floorCheck].getHitPoint().x < rightWallLocation))
            {
                //if the surface is level with or below the bot's feet, the bot should consider going there
                if (info[floorCheck].getHitPoint().y <= transform.position.y)
                    possibleActions.Add(floorCheck);
            }
        }

        //if the potential places the bot can head to are clustered all to the left or all to the right, then the bot heads there
        if (possibleActions.Count >= 1 && possibleActionsInSameDirection())
        {
            action = pickPossibileAction();
            endSpot = info[action].getHitPoint();
            check = 1;

            rig.velocity = new Vector2(botSpeed * Mathf.Sign(action.x), rig.velocity.y);
        }

        //if the bot has a potential place it can go both left and right of it, it randomly chooses a direction to head in
        else if (possibleActions.Count >= 1)
        {
            action = pickPossibileAction();
            endSpot = info[action].getHitPoint();
            check = 1;

            rig.velocity = new Vector2(botSpeed * (Random.Range(0, 2) * 2 - 1), rig.velocity.y);
        }

        //if the bot spots no floor gaps, it heads towards the direction with more open space / away from a wall 
        else
        {
            float leftWallDistance = transform.position.x - info[new EnvKey('L', 0, 0)].getHitPoint().x;
            float rightWallDistance = info[new EnvKey('R', 0, 0)].getHitPoint().x - transform.position.x;

            rig.velocity = (rightWallDistance >= leftWallDistance) ?
                new Vector2(botSpeed, rig.velocity.y) : new Vector2(-botSpeed, rig.velocity.y);
        }

            Debug.LogFormat("{0}: {1}, {2}", action.direction, action.x, action.y);
        

        //bot checks for platforms that are one-two layers above it)
        

        actionsTakenToExecutePath();
    }

    //Now that the bot has chosen a place to reach, tell it how to get there
    private void actionsTakenToExecutePath()
    {
        //check for gaps in the way if the bot is heading far right
        if (action.x > 0 && action.direction == 'G')
        {
            for (int i = 1; i < action.x; i++)
            {
                EnvInfo nextRaycast = info[new EnvKey('G', i, action.y)];

                //some gap is in the way, so the bot will remember its coordinate to jump over it 
                if (nextRaycast.getHitPoint().y < info[action].getHitPoint().y - 0.1f)
                {
                    check = 2;
                    jumpCoordinate = nextRaycast.getHitPoint().x;
                    ceilingCheckAboveJump = new EnvKey('C', i, action.y);
                    break;
                }

            }
        }

       //check for gaps in the way if the bot is heading far left
       if (action.x < 0 && action.direction == 'G')
       {
            for (int i = -1; i > action.x; i--)
            {
                EnvInfo nextRaycast = info[new EnvKey('G', i, action.y)];

                //some gap is in the way, so the bot will remember its coordinate to jump over it 
                if (nextRaycast.getHitPoint().y < info[action].getHitPoint().y - 0.1f)
                {
                    check = 2; 
                    jumpCoordinate = nextRaycast.getHitPoint().x;
                    ceilingCheckAboveJump = new EnvKey('C', i, action.y);
                    break;
                }

            }
        }

       //if there is a gap in the way as the bot heads left or right
        if (action.x != 0 && action.direction == 'G' && check == 2)
        {
            dontJump = false;

            //if there is a ceiling in the way, the bot will jump when its closer to its end destination to avoid head butting into the ceiling
            if (info[new EnvKey(ceilingCheckAboveJump.direction, ceilingCheckAboveJump.x + (int)Mathf.Sign(action.x), ceilingCheckAboveJump.y)].getHitPoint().y < transform.position.y + 2f)
                dontJump = true;
        }
    }

    
    void Update()
    {
        fixedBotBehaviour.botOrientation(rig, sR);
        fixedBotBehaviour.botAnimation(animator, rig);

        botFallingSettings();
        botStillSettings();

        //make a new plan of action at least every 4.2 seconds
        if (actionTimer > 0)
            actionTimer -= Time.deltaTime;
        else
            figureOutWhatToDo();

        //bot is about to approach a gap that it needs to jump over to continue its plan of action
        if (check == 2 && ((action.x > 0 && transform.position.x > jumpCoordinate - 2) || (action.x < 0 && transform.position.x < jumpCoordinate + 2)))
        {
            if (!dontJump)
                fixedBotBehaviour.jump(rig, jumpForce);
            check = 3;
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

    private void botFallingSettings()
    {
        //bot is falling -> kill horizontal velocity
        if ((check == 1 || check == 3) && Mathf.Abs(endSpot.x - transform.position.x) < 0.25f)
        {
            check = 4 ;
            Debug.Log("bruhhhh " + check + " " + transform.position.y + ", " + endSpot.y);

            if (Mathf.Abs(endSpot.y - transform.position.y) > 2)
                rig.velocity = new Vector2(rig.velocity.x * Random.Range(0.05f, 0.4f), rig.velocity.y);
        }

        //bot is falling and just about to land -> make new decision
        if (check == 4 && transform.position.y <= 1.7f + endSpot.y)
        {
            check = 0;
            figureOutWhatToDo();
        }
    }

    //if the bot is still for 0.3 seconds for whatever reason, it will make a new plan of action 
    private void botStillSettings()
    {
        if (Mathf.Abs(rig.velocity.x) <= 0.6f && Mathf.Abs(rig.velocity.y) <= 0.2f && stillTimer < 0f)
        {
            stillTimer = Random.Range(0.1f, 0.3f);
            Debug.Log("timer reset");
        }

        if (stillTimer > 0f)
            stillTimer -= Time.deltaTime;

        if (stillTimer < 0f && Mathf.Abs(rig.velocity.x) <= 0.6f && Mathf.Abs(rig.velocity.y) <= 0.2f)
        {
            check = 0;
            figureOutWhatToDo();
            Debug.Log("new decision made by timer");
        }
    }

    //bot constantly checks for ground and nearby walls every 0.35 seconds
    private IEnumerator constantWallChecks()
    {
        Vector2 check = fixedBotBehaviour.wallChecks(leftFoot, rightFoot, map, transform);

        yield return new WaitForSeconds(0.35f);
        StartCoroutine(constantWallChecks());
    }
}
