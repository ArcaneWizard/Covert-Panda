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
    private Transform leftHead;
    private Transform rightHead;

    private LayerMask map = (1 << 11);

    private EnvironmentDetection environmentInputs = new EnvironmentDetection();
    private FixedBotBehaviour fixedBotBehaviour = new FixedBotBehaviour();
    private Dictionary<EnvKey, EnvInfo> info;

    private float botSpeed = 4f;
    private float jumpForce = 420;

    private EnvKey action;
    private List<EnvKey> possibleLeftActions = new List<EnvKey>();
    private List<EnvKey> possibleRightActions = new List<EnvKey>();

    private int check = 0;
    private bool doWallCheck = false;
    private char lastAction = 'A';

    private float stillTimer = -0.01f;

    private float maxtimebetweenActions = 4.2f;
    private float actionTimer = -0.01f;

    //private variables used between methods
    private Vector2 endSpot;
    private float dir;
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
        leftHead = transform.GetChild(3);
        rightHead = transform.GetChild(4);

        botSpeed = Random.Range(4.3f, 5f);
    }

    void Start()
    {
        figureOutWhatToDo();
    }

    //scan environment, store this info in a dictionary, and decide on a plan of action
    private void figureOutWhatToDo()
    {
        Debug.Log("called bt");
        environmentInputs.scanEnvironment(transform);
        info = environmentInputs.requestEnvironmentInfo();

        actionTimer = maxtimebetweenActions;
        figureOutWhereToHeadTo(); 
    }

    //---------------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------------


    //bot makes a plan of action (determines place to reach) and starts moving 
    private void figureOutWhereToHeadTo()
    {
        //resest possibleActions list
        possibleLeftActions.Clear();
        possibleRightActions.Clear();

        //get x position of closest left and right wall
        float rightWallLocation = info[new EnvKey('R', 0, 0)].getHitPoint().x;
        float leftWallLocation = info[new EnvKey('L', 0, 0)].getHitPoint().x;

        //bot takes into account it's last location/action 
        lastAction = action.Equals(null) ? 'A' : action.direction;

        //the bot checks for flat surfaces at the same level or below it
        for (int offset = -5; offset <= 5; offset++)
        {
            EnvKey floorCheck = new EnvKey('G', offset, 0);

            //make sure this flat surface is not behind a wall (as the bot can't just walk straight to it then)
            if ((offset < 0 && info[floorCheck].getHitPoint().x > leftWallLocation) ||
                (offset > 0 && info[floorCheck].getHitPoint().x < rightWallLocation))
            {
                //if it's an opening in the floor, the bot should consider going there
                if (info[floorCheck].getHitPoint().y <= transform.position.y - 2.1f)
                {
                    //add the opening to its corresponding action list
                    if (offset > 0) 
                        possibleRightActions.Add(floorCheck);
                    else
                        possibleLeftActions.Add(floorCheck);
                }
            }

            //if there were no right gaps found but there is a hallway to the right, still consider going there
            if (possibleRightActions.Count == 0 && rightWallLocation > transform.position.x + 1f && offset == 5)
            {
                Debug.Log("right wall considered");
                //bot should usually consider it could go right (if it wasn't already going left or if it can't go LEFT next turn)
                if (lastAction != 'L' || leftWallLocation >= transform.position.x - 1f)
                    possibleRightActions.Add(new EnvKey('R', 0, 0));

                //if the bot was previously heading left, it should rarely consider going right
                else if (lastAction == 'L')
                {
                    int r = Random.Range(0, 10);
                    if (r >= 8)
                        possibleRightActions.Add(new EnvKey('R', 0, 0));
                }
            }


            //if there were no left gaps found but there is a hallway to the left, still consider going there
            if (possibleLeftActions.Count == 0 && leftWallLocation < transform.position.x - 1f && offset == 1)
            {
                Debug.Log("left wall considered");
                //bot should usually consider going left  (if it wasn't already going right or if it can't go right next turn)
                if (lastAction != 'R' || rightWallLocation <= transform.position.x + 1f)
                    possibleLeftActions.Add(new EnvKey('L', 0, 0));

                //if the bot was previously heading right, it should rarely consider going left
                else if (lastAction == 'R')
                {
                    int r = Random.Range(0, 10);
                    if (r >= 8)
                        possibleLeftActions.Add(new EnvKey('L', 0, 0));
                }
            }
        }

        //bot chooses where to head to
        action = pickPossibleAction();
        endSpot = info[action].getHitPoint();
        
        //bot heads to the place it chose
        if (action.direction == 'G')
        {
            check = 1;
            dir = Mathf.Sign(action.x);
        }
        else if (action.direction == 'R')
        {
            check = 201;
            dir = 1;
        }
        else if (action.direction == 'L')
        {
            check = 201;
            dir = -1;
        }

        rig.velocity = new Vector2(botSpeed * dir, rig.velocity.y);
        actionsTakenToExecutePath();

        Debug.LogFormat("{0}: {1}, {2}", action.direction, action.x, action.y);
    }

    //---------------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------------

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

        //if heading left or right in a hallway, define the spot where the bot must refigure out what to do
        if (action.direction == 'R')
            endSpot = new Vector2(info[action].getHitPoint().x - 2, 0);

        if (action.direction == 'L')
            endSpot = new Vector2(info[action].getHitPoint().x + 2, 0);
    }

    //---------------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------------

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
        if (check == 2 && action.direction == 'G' && ((action.x > 0 && transform.position.x > jumpCoordinate - 2) || (action.x < 0 && transform.position.x < jumpCoordinate + 2)))
        {
            if (!dontJump)
                fixedBotBehaviour.jump(rig, jumpForce);
            check = 3;
        }

        //if walking straight left/right, bot needs to refigure out what to do after a while
        if (action.direction == 'L' && transform.position.x < endSpot.x)
            figureOutWhatToDo();
        else if (action.direction == 'R' && transform.position.x > endSpot.x)
            figureOutWhatToDo();
    }

    //---------------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------------

    //bot randomly chooses to go left or right, then picks a destination to their left or right
    private EnvKey pickPossibleAction()
    {
        int chooseDirection = Random.Range(0, 100);

        //sometimes the bot MUST go left or MUST go right
        if (possibleLeftActions.Count == 0)
            chooseDirection = 50;
        else if (possibleRightActions.Count == 0)
            chooseDirection = 0;

        //pick a left or right destination
        if (chooseDirection < 50 && possibleLeftActions.Count > 0)
        {
            int choice = Random.Range(0, possibleLeftActions.Count);
            return possibleLeftActions[choice];
        }

        else if (chooseDirection >= 50 && possibleRightActions.Count > 0)
        {
            int choice = Random.Range(0, possibleRightActions.Count);
            return possibleRightActions[choice];
        }

        //if the bot somehow has no left/right paths it found (should be impossible), pick a random direction
        else
        {
            Debug.LogError("both action lists were empty :(");

            int r = Random.Range(0, 10);
            char dir = (r <= 4) ? 'R' : 'L'; 
            return new EnvKey(dir, 0, 0);
        }
    }

    //---------------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------------

    private void botFallingSettings()
    {
        //bot is falling -> kill horizontal velocity
        if ((check == 1 || check == 3) && Mathf.Abs(endSpot.x - transform.position.x) < 0.25f)
        {
            check = 4 ;

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

    //bot constantly checks for ground and nearby walls every 0.2 seconds
    private IEnumerator constantWallChecks()
    {
        WallChecker check;

        //don't do a wall check unless neccessary (to avoid lag) 
        if (doWallCheck)
            check = fixedBotBehaviour.wallChecks(leftFoot, rightFoot, leftHead, rightHead, map, transform);

        //wait 0.2 seconds and repeat method
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(constantWallChecks());
    }
}
