using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotAI : MonoBehaviour
{
    public Transform player;
    private Rigidbody2D rig;
    private Animator animator;
    private SpriteRenderer sR;
    private Transform leftFoot;
    private Transform rightFoot;
    private Transform leftHead;
    private Transform rightHead;

    private EnvironmentDetection environmentInputs = new EnvironmentDetection();
    private FixedBotBehaviour fixedBotBehaviour = new FixedBotBehaviour();
    private Dictionary<EnvKey, EnvInfo> info;

    private float botSpeed = 4f;
    private float jumpForce = 570;

    private EnvKey action;
    private List<EnvKey> possibleLeftActions = new List<EnvKey>();
    private List<EnvKey> possibleRightActions = new List<EnvKey>();

    private int check = 0;
    private bool doWallCheck = false;
    private char lastAction = 'A';
    private int lastMovement = 1;

    private float stillTimer = -0.01f;
    private float actionTimer = -0.01f;
    private float isGroundedTimer = -0.01f;

    private float maxtimebetweenActions = 4.2f;
    private float scanForPlayerInterval = 0.15f;
    private float lastYVelocity = -2;

    //private variables used between methods
    private Vector2 endSpot;
    private float dir;
    private float jumpCoordinate;
    private EnvKey ceilingCheckAboveJump;
    private bool dontJump;
    private bool holeSpottedB4;
    private Dictionary<EnvKey, string> jumpPlatforms = new Dictionary<EnvKey, string>();
    private float jumpToHeight = 0;

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
        StartCoroutine(lookForPlayer());
    }

    //scan environment, store this info in a dictionary, and decide on a plan of action
    private void figureOutWhatToDo()
    {
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
        //resest action lists + variables
        possibleLeftActions.Clear();
        possibleRightActions.Clear();
        jumpPlatforms.Clear();
        holeSpottedB4 = false;

        //get x position of closest left and right wall
        float rightWallLocation = info[new EnvKey('R', 0, 0)].location().x;
        float leftWallLocation = info[new EnvKey('L', 0, 0)].location().x;

        //bot takes into account it's last action/location
        lastAction = action.Equals(null) ? 'A' : action.direction;
        lastMovement = action.Equals(null) ? 1 : (int)Mathf.Sign(action.x);

        //the bot looks to its left for 1) openings in the floor it can drop down in 2) seperate platforms it can jump to 
        for (int offset = -1; offset >= -5; offset--)
        {
            EnvKey floorCheck = new EnvKey('G', offset, 0);
            EnvKey prevFloorCheck = new EnvKey('G', offset + 1, 0);

            //make sure this opening in the floor is not behind a wall (as the bot can't just walk straight to it then)
            if (info[floorCheck].location().x > leftWallLocation)
            {
                //consider going to the first drop down opening on it's left 
                if (info[floorCheck].location().y <= transform.position.y - 2.1f && !holeSpottedB4)
                {
                    holeSpottedB4 = true;
                    possibleLeftActions.Add(floorCheck);
                }

                //consider going to the first seperate platform it can jump to on it's left 
                if (offset != -1 && info[floorCheck].location().y > transform.position.y - 2.1f && info[prevFloorCheck].location().y <= transform.position.y - 2.1f && holeSpottedB4)
                    possibleLeftActions.Add(floorCheck);
            }
        }

        //reset hole spotted variable 
        holeSpottedB4 = false;

        //the bot looks to its right for 1) openings in the floor it can drop down in 2) seperate platforms it can jump to 
        for (int offset = 1; offset <= 5; offset++)
        {
            EnvKey floorCheck = new EnvKey('G', offset, 0);
            EnvKey prevFloorCheck = new EnvKey('G', offset - 1, 0);

            //make sure this opening in the floor is not behind a wall (as the bot can't just walk straight to it then)
            if (info[floorCheck].location().x < rightWallLocation)
            {
                //consider going to the first drop down opening on it's right 
                if (info[floorCheck].location().y <= transform.position.y - 2.1f && !holeSpottedB4)
                {
                    holeSpottedB4 = true;
                    possibleRightActions.Add(floorCheck);
                }

                //consider going to the first seperate platform it can jump to on it's right 
                if (offset != 1 && info[floorCheck].location().y > transform.position.y - 2.1f && info[prevFloorCheck].location().y <= transform.position.y - 2.1f && holeSpottedB4)
                    possibleRightActions.Add(floorCheck);
            }
        }

        //if there is a hallway to the right, consider going there
        if (possibleRightActions.Count <= 1 && rightWallLocation > transform.position.x + 2f)
        {
            //bot should usually consider going right (if it wasn't already going left or if it can't go LEFT next turn)
            if (lastAction != 'L' || leftWallLocation >= transform.position.x - 2.5f)
                possibleRightActions.Add(new EnvKey('R', 0, 0));

            //if the bot was previously heading left, it should rarely consider going right
            else if (lastAction == 'L')
            {
                int r = Random.Range(0, 10);
                if (r >= 9)
                    possibleRightActions.Add(new EnvKey('R', 0, 0));
            }
        }

        //if there is a hallway to the left, still consider going there
        if (possibleLeftActions.Count <= 1 && leftWallLocation < transform.position.x - 2f)
        {
            //bot should usually consider going left  (if it wasn't already going right or if it can't go right next turn)
            if (lastAction != 'R' || rightWallLocation <= transform.position.x + 2.5f)
                possibleLeftActions.Add(new EnvKey('L', 0, 0));

            //if the bot was previously heading right, it should rarely consider going left
            else if (lastAction == 'R')
            {
                int r = Random.Range(0, 10);
                if (r >= 9)
                    possibleLeftActions.Add(new EnvKey('L', 0, 0));
            }
        }

        //the bot checks for openings in the ceiling
        for (int offset = -5; offset <= 5; offset++)
        {
            //get the y coordinate of the ceiling above the player
            EnvKey ceilingCheck = new EnvKey('C', offset, 0);
            EnvInfo ceiling = info[ceilingCheck];

            //get the expected height a ceiling should be (for a normal ceiling with no opening)
            float expectedHeight = 4f + transform.position.y;
            float ceilingHeight = ceiling.location().y;

            //if there is a gap in the ceiling, and its not behind a left or right wall 
            if (ceilingHeight > expectedHeight && ceiling.location().x < rightWallLocation && ceiling.location().x > leftWallLocation)
            {
                //if the bot theoretically jumped through the ceiling gap, calculate how far back the left and right walls are on the new platform they would land on
                Vector2 rayCastOrigin = new Vector2(transform.position.x + offset * 1.5f, transform.position.y + 4f);

                RaycastHit2D leftHit = Physics2D.Raycast(rayCastOrigin, Vector2.left, 9f, Constants.map);
                RaycastHit2D rightHit = Physics2D.Raycast(rayCastOrigin, Vector2.right, 9f, Constants.map);

                float leftUpperWall = (leftHit.collider != null) ? leftHit.point.x : rayCastOrigin.x - 9;
                float rightUpperWall = (rightHit.collider != null) ? rightHit.point.x : rayCastOrigin.x + 9;

                float rightUpperWallDistance = rightUpperWall - rayCastOrigin.x;
                float leftUpperWallDistance = rayCastOrigin.x - leftUpperWall;

                //setting these variables to miscellaneous values to avoid null errors
                EnvInfo leftCeiling = info[new EnvKey('C', 0, 0)];
                EnvInfo rightCeiling = info[new EnvKey('C', 0, 0)];
                float leftCeilingHeight = 0, rightCeilingHeight = 0;
                GameObject leftCeilingObject = null, rightCeilingObject = null;

                //can't do left ceiling checks for the left most ceiling and vice versa on the right side
                if (offset != -5)
                {
                    leftCeiling = info[new EnvKey('C', offset - 1, 0)];
                    leftCeilingHeight = leftCeiling.location().y;
                    leftCeilingObject = leftCeiling.gameObject();
                }
                if (offset != 5)
                {
                    rightCeiling = info[new EnvKey('C', offset + 1, 0)];
                    rightCeilingHeight = rightCeiling.location().y;
                    rightCeilingObject = rightCeiling.gameObject();
                }

                GameObject groundObject = info[new EnvKey('G', 0, 0)].gameObject();

                //get the tilt of the walls directly to the left and right of the bot right now
                EnvInfo rightWall = info[new EnvKey('R', 0, 0)];
                float rightWallTilt = rightWall.gameObject() ? rightWall.gameObject().transform.eulerAngles.z : 0f;

                EnvInfo leftWall = info[new EnvKey('L', 0, 0)];
                float leftWallTilt = leftWall.gameObject() ? leftWall.gameObject().transform.eulerAngles.z : 0f;

                //if bot spots an opening in the ceiling to its left or right and there is ceiling directly to the right of the opening,
                //then add this jump as a possible action as long as there is no nearby ramp to the right

                if (offset == 1)
                {
                    Debug.LogFormat("{0}, {1}, {2}, {3}, {4}, {5}", rightCeilingHeight < expectedHeight, rightUpperWallDistance > 2f, rightCeilingObject != groundObject, !rightWall.gameObject(), Mathf.Abs(rightCeiling.location().x - rightWall.location().x) > 1.51f, !wallIsActuallyARamp(rightWallTilt));
                }

                if (offset != 5 && rightCeilingHeight < expectedHeight && rightUpperWallDistance > 2f && rightCeilingObject != groundObject &&
                (!rightWall.gameObject() || Mathf.Abs(rightCeiling.location().x - rightWall.location().x) > 1.51f || !wallIsActuallyARamp(rightWallTilt)))
                {
                    if (offset > 0)
                    {
                        possibleRightActions.Add(ceilingCheck);
                    }
                    else if (offset < 0)
                        possibleLeftActions.Add(ceilingCheck);
                    else if (offset == 0)
                    {
                        if (ceiling.location().x >= transform.position.x)
                            possibleRightActions.Add(ceilingCheck);
                        else
                            possibleLeftActions.Add(ceilingCheck);
                    }

                    jumpPlatforms.Add(ceilingCheck, "right");
                    jumpToHeight = transform.position.y + 4f;
                }

                //if bot spots an opening in the ceiling to its left or right, but there is ceiling directly to the left of the opening in the ceiling, then add this jump as a possible action
                else if (offset != -5 && leftCeilingHeight < expectedHeight && leftUpperWallDistance > 2f && leftCeilingObject != groundObject &&
                (!leftWall.gameObject() || Mathf.Abs(leftCeiling.location().x - leftWall.location().x) > 1.51f || !wallIsActuallyARamp(leftWallTilt)))
                {
                    if (offset > 0)
                        possibleRightActions.Add(ceilingCheck);
                    else if (offset < 0)
                        possibleLeftActions.Add(ceilingCheck);
                    else if (offset == 0)
                    {
                        if (ceiling.location().x <= transform.position.x)
                            possibleLeftActions.Add(ceilingCheck);
                        else
                            possibleRightActions.Add(ceilingCheck);
                    }

                    jumpPlatforms.Add(ceilingCheck, "left");
                    jumpToHeight = transform.position.y + 4f;
                }
            }
        }

        //bot chooses where to head to
        action = pickPossibleAction();
        endSpot = info[action].location();

        action.printColored();

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
        else if (action.direction == 'C')
        {
            dir = Mathf.Sign(action.x);
            check = 301;
        }

        rig.velocity = new Vector2(botSpeed * dir, rig.velocity.y);
        actionsTakenToExecutePath();
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
                if (nextRaycast.location().y < info[action].location().y - 0.1f)
                {
                    check = 2;
                    jumpCoordinate = nextRaycast.location().x;
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
                if (nextRaycast.location().y < info[action].location().y - 0.1f)
                {
                    check = 2;
                    jumpCoordinate = nextRaycast.location().x;
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
            if (info[new EnvKey(ceilingCheckAboveJump.direction, ceilingCheckAboveJump.x + (int)Mathf.Sign(action.x), ceilingCheckAboveJump.y)].location().y < transform.position.y + 2f)
                dontJump = true;
        }

        //if heading left or right in a hallway, define the spot where the bot must refigure out what to do
        if (action.direction == 'R')
            endSpot = new Vector2(info[action].location().x - 2, 0);

        if (action.direction == 'L')
            endSpot = new Vector2(info[action].location().x + 2, 0);
    }

    //---------------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------------

    void Update()
    {
        fixedBotBehaviour.botOrientation(rig, sR);
        fixedBotBehaviour.botAnimation(animator, rig);

        botFallingSettings();
        botStillSettings();

        //bot is about to approach a gap that it needs to jump over to continue its plan of action
        if (check == 2 && action.direction == 'G' && ((action.x > 0 && transform.position.x > jumpCoordinate - 1.2f) || (action.x < 0 && transform.position.x < jumpCoordinate + 2)))
        {
            if (!dontJump)
                fixedBotBehaviour.jump(rig, jumpForce, true);
            check = 3;
        }

        //if walking straight left/right, bot needs to refigure out what to do after a while
        if (action.direction == 'L' && transform.position.x < endSpot.x)
            figureOutWhatToDo();
        else if (action.direction == 'R' && transform.position.x > endSpot.x)
            figureOutWhatToDo();

        //bot is ready to jump to a higher platform (it is near a ceiling gap)
        if (check == 301 && action.direction == 'C')
        {
            float multiplier = (jumpToHeight - transform.position.y) / 4f * 1.05f + 0.05f;

            //if bot is moving, lessen the jump height a tad bit
            multiplier *= (Mathf.Abs(rig.velocity.x) > 2) ? 0.95f : 1;

            //bot is moving right but plans to go left after jumping to the upper level
            if (jumpPlatforms[action] == "left" && rig.velocity.x > 0 && (transform.position.x - info[action].location().x) > Random.Range(0.05f, 0.15f))
            {
                jumpCoordinate = transform.position.y;
                fixedBotBehaviour.jump(rig, jumpForce * multiplier, false);
                rig.AddForce(new Vector2(Random.Range(20, 25), 0));
                check = 302;
            }

            //bot is moving left and plans to go left after jumping to the upper level
            else if (jumpPlatforms[action] == "left" && rig.velocity.x <= 0 && (transform.position.x - info[action].location().x) < Random.Range(1.8f, 2.2f))
            {
                rig.velocity = new Vector2(rig.velocity.x / 1.5f, rig.velocity.y);
                jumpCoordinate = transform.position.y;
                fixedBotBehaviour.jump(rig, jumpForce * multiplier, false);
                check = 303;
            }

            //bot is moving left but plans to go right after jumping to the upper level
            else if (jumpPlatforms[action] == "right" && rig.velocity.x < 0 && (info[action].location().x - transform.position.x) > Random.Range(0.5f, 0.6f))
            {
                jumpCoordinate = transform.position.y;
                fixedBotBehaviour.jump(rig, jumpForce * multiplier, false);
                rig.AddForce(new Vector2(Random.Range(-20, -25), 0));
                check = 304;
            }

            //bot is moving right and plans to go right after jumping to the upper level
            else if (jumpPlatforms[action] == "right" && rig.velocity.x >= 0 && (info[action].location().x - transform.position.x) < Random.Range(1.8f, 2.2f))
            {
                rig.velocity = new Vector2(Mathf.Abs(rig.velocity.x) / 1.5f, rig.velocity.y);
                jumpCoordinate = transform.position.y;
                fixedBotBehaviour.jump(rig, jumpForce * multiplier, false);
                check = 305;

            }
        }

        //once the bot jumps through a ceiling gap, give it further instructions that haven't already been delivered
        if (check >= 302 && transform.position.y > jumpCoordinate + 2.2f)
        {
            //bot jumped to the right, but now mid-way needs to start turning left
            if (jumpPlatforms[action] == "left" && check == 302)
            {
                rig.velocity = new Vector2(0, rig.velocity.y);
                rig.AddForce(new Vector2(Random.Range(-170, -150), 0));
                check = 303;
            }

            //bot jumped to the left, but now mid-way needs to start turning right
            if (jumpPlatforms[action] == "right" && check == 304)
            {
                rig.velocity = new Vector2(0, rig.velocity.y);
                rig.AddForce(new Vector2(Random.Range(150, 170), 0));
                check = 305;
            }
        }

        //make a new plan of action at least every 4.2 seconds
        if (actionTimer > 0)
            actionTimer -= Time.deltaTime;
        else
            figureOutWhatToDo();

        //check if the bot is grounded every 0.12 seconds
        if (isGroundedTimer <= 0)
        {
            //the bot is Grounded (x velocity has remained constant for last 0.12 seconds and y velocity is near 0)
            if (Mathf.Abs(lastYVelocity - rig.velocity.y) < 0.011f && rig.velocity.y < 0.2f)
            {
                if (check == 303 || check == 305 || check == 3)
                    figureOutWhatToDo();
            }

            lastYVelocity = rig.velocity.y;
            isGroundedTimer = 0.12f;
        }

        else
            isGroundedTimer -= Time.deltaTime;
    }

    //---------------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------------

    //bot randomly chooses to go left or right, then picks a destination to their left or right
    private EnvKey pickPossibleAction()
    {
        foreach (EnvKey key in possibleLeftActions)
            Debug.LogFormat("Left| {0}: {1}, {2}", key.direction, key.x, key.y);

        foreach (EnvKey key in possibleRightActions)
            Debug.LogFormat("Right| {0}: {1}, {2}", key.direction, key.x, key.y);

        //bot randomly picks a direction, but gives greater weight to moving in the same direction as before 
        int chooseDirection = Random.Range(0, 100);
        chooseDirection = (lastMovement == -1) ? (chooseDirection - 20) : chooseDirection + 20;

        //if the bot is on a ramp, it recalculates its decision super often, so make it super inclined to continue int he same direction
        EnvInfo ground = info[new EnvKey('G', 0, 0)];
        float groundTilt = (ground.gameObject()) ? ground.gameObject().transform.localEulerAngles.z : 0;

        if (ground.gameObject() && transform.position.y - ground.location().y < 1.6f && groundIsRamp(groundTilt))
        {
            chooseDirection += 40 * lastMovement;
            //Debug.Log("on ramp");
        }
        // else
        //     Debug.Log("off ramp " + ground + ", " + (transform.position.y - ground.location().y) + ", " + groundIsRamp(groundTilt));

        //sometimes the bot MUST go left or MUST go right
        if (possibleLeftActions.Count == 0)
            chooseDirection = 50;
        else if (possibleRightActions.Count == 0)
            chooseDirection = 0;

        Debug.Log(chooseDirection + ", " + lastMovement);
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
        if ((check == 1 || check == 3) && Mathf.Abs(endSpot.x - transform.position.x) < 0.25f && rig.velocity.y < 0)
        {
            check = 4;

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
            //Debug.Log("timer reset");
        }

        if (stillTimer > 0f)
            stillTimer -= Time.deltaTime;

        if (stillTimer < 0f && Mathf.Abs(rig.velocity.x) <= 0.6f && Mathf.Abs(rig.velocity.y) <= 0.2f)
        {
            check = 0;
            figureOutWhatToDo();
            //Debug.Log("new decision made by timer");
        }
    }

    /*//bot constantly checks for ground and nearby walls every 0.2 seconds
    private IEnumerator constantWallChecks()
    {
        WallChecker check;

        //don't do a wall check unless neccessary (to avoid lag) 
        if (doWallCheck)
            check = fixedBotBehaviour.wallChecks(leftFoot, rightFoot, leftHead, rightHead, Constants.map, transform);

        //wait 0.2 seconds and repeat method
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(constantWallChecks());
    }*/

    //bot scans for the player to shoot at the player
    private IEnumerator lookForPlayer()
    {
        BotAttacks botAttack = transform.GetComponent<BotAttacks>();
        RaycastHit2D scan = Physics2D.Raycast(botAttack.bulletSpawnPoint.position, player.position - botAttack.bulletSpawnPoint.position, 10, Constants.mapOrPlayer);

        //if the raycast coming from the enemy gun hit the player instead of a platform
        if (scan.collider != null && scan.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            botAttack.shootBullet(player.position - botAttack.bulletSpawnPoint.position);

        yield return new WaitForSeconds(0.3f);
        StartCoroutine(lookForPlayer());
    }

    private bool wallIsActuallyARamp(float zAngle)
    {
        float angle = zAngle % 180;
        return (angle < Constants.maxPlatformTilt || angle > 180 - Constants.maxPlatformTilt);
    }

    private bool groundIsRamp(float zAngle)
    {
        float angle = zAngle % 180;
        return (angle > 2 && angle < 178);
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        //if the bot is on some surface
        if (col.gameObject.layer == LayerMask.NameToLayer("Map"))
        {
            //determine if it's actual ground (could be slanted little, but shouldn't be vertical like a wall)
            float platformAngle = col.transform.eulerAngles.z;
            if (Mathf.Abs(platformAngle) < Constants.maxPlatformTilt && Mathf.Abs(rig.velocity.x) > 1f)
            {
                //make sure bot moves at a constant x velocity even on slanted ground
                if (rig.velocity.x > 0 && (rig.velocity.x > botSpeed + 0.15f || rig.velocity.x < botSpeed - 0.15f))
                {
                    rig.velocity = new Vector2(botSpeed, rig.velocity.y);
                    print("correct speed");
                }
                else if (rig.velocity.x < 0 && (rig.velocity.x < -botSpeed - 0.15f || rig.velocity.x > -botSpeed + 0.15f))
                    rig.velocity = new Vector2(-botSpeed, rig.velocity.y);
            }
        }
    }
}

