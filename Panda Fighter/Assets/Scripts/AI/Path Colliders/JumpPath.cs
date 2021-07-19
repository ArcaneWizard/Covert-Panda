using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine.UI;
using UnityEngine;

public class JumpPath : MonoBehaviour
{
    public List<List<bool>> rightJumpList = new List<List<bool>>(new List<bool>[13]);
    public List<List<bool>> leftJumpList = new List<List<bool>>(new List<bool>[13]);

    public List<Transform> rightJumpPathColliders = new List<Transform>();
    public List<Transform> leftJumpPathColliders = new List<Transform>();
    public List<Transform> rightJumpPathGround = new List<Transform>();
    public List<Transform> leftJumpPathGround = new List<Transform>();

    private NewBotAI AI;
    public Text possibleJumps;
    public Text jumpDecision;
    private DecisionMaking decision;

    private int groundDiff;
    private float boost = 0.4f; //jump speed boost
    private locationExists leftJumpTrajectory;
    private locationExists rightJumpTrajectory;

    private int rightDescentCollider;
    private int leftDescentCollider;

    private List<GameObject> obstacles = new List<GameObject>();

    void Awake()
    {
        AI = transform.parent.transform.GetChild(0).transform.GetComponent<NewBotAI>();
        decision = transform.parent.transform.GetChild(0).transform.GetComponent<DecisionMaking>();

        rightJumpList[0] = new List<bool>(new bool[14]);
        rightJumpList[1] = new List<bool>(new bool[15]);
        rightJumpList[2] = new List<bool>(new bool[12]);
        rightJumpList[3] = new List<bool>(new bool[15]);
        rightJumpList[4] = new List<bool>(new bool[12]);
        rightJumpList[5] = new List<bool>(new bool[12]);
        rightJumpList[6] = new List<bool>(new bool[11]);
        rightJumpList[7] = new List<bool>(new bool[12]);
        rightJumpList[8] = new List<bool>(new bool[13]);
        rightJumpList[9] = new List<bool>(new bool[12]);
        rightJumpList[10] = new List<bool>(new bool[12]);
        rightJumpList[11] = new List<bool>(new bool[9]);
        rightJumpList[12] = new List<bool>(new bool[9]);

        leftJumpList[0] = new List<bool>(new bool[14]);
        leftJumpList[1] = new List<bool>(new bool[15]);
        leftJumpList[2] = new List<bool>(new bool[12]);
        leftJumpList[3] = new List<bool>(new bool[15]);
        leftJumpList[4] = new List<bool>(new bool[12]);
        leftJumpList[5] = new List<bool>(new bool[12]);
        leftJumpList[6] = new List<bool>(new bool[11]);
        leftJumpList[7] = new List<bool>(new bool[12]);
        leftJumpList[8] = new List<bool>(new bool[13]);
        leftJumpList[9] = new List<bool>(new bool[12]);
        leftJumpList[10] = new List<bool>(new bool[12]);
        leftJumpList[11] = new List<bool>(new bool[9]);
        leftJumpList[12] = new List<bool>(new bool[9]);
    }

    void Start()
    {
        //generateGroundChecks();

        StartCoroutine(getJumpTrajectory());
    }

    private IEnumerator getJumpTrajectory()
    {
        if (decision.botState != "experimental")
        {
            AI.findWalls();
            decision.rightJumps.Clear();
            decision.leftJumps.Clear();

            if (AI.grounded && AI.touchingMap)
            {
                if ((AI.movementDirX == 1 || AI.movementDirX == 0))
                    getRightJumpTrajectory();
                else if (AI.movementDirX == -1)
                    getLeftJumpTrajectory();

                decision.decideWhetherToJump();
                jumpDecision.text = "deciding whether to jump...";
            }
            else
                jumpDecision.text = "not on ground yet";

            //show jumps available for debugging purposes
            possibleJumps.text = "Jumps: ";
            foreach (Jump jump in decision.rightJumps)
                possibleJumps.text += "\n" + jump.getType() + ", " + jump.getJumpSpeed() + ", " + jump.getDelay() + ", " + jump.getMidAirSpeed();
            foreach (Jump jump in decision.leftJumps)
                possibleJumps.text += "\n" + jump.getType() + ", " + jump.getJumpSpeed() + ", " + jump.getDelay() + ", " + jump.getMidAirSpeed();
        }

        yield return new WaitForSeconds(0.25f);
        StartCoroutine(getJumpTrajectory());
    }

    void Update()
    {
        if (AI.movementDirX == 1 && leftJumpPathColliders[0].gameObject.activeSelf)
            checkRightPathsAndNotLeftPaths(true);
        else if (AI.movementDirX == -1 && rightJumpPathColliders[0].gameObject.activeSelf)
            checkRightPathsAndNotLeftPaths(false);
    }

    private void checkRightPathsAndNotLeftPaths(bool check)
    {
        foreach (Transform path in leftJumpPathColliders)
            path.gameObject.SetActive(!check);
        foreach (Transform path in leftJumpPathGround)
            path.gameObject.SetActive(!check);
        foreach (Transform path in rightJumpPathColliders)
            path.gameObject.SetActive(check);
        foreach (Transform path in rightJumpPathGround)
            path.gameObject.SetActive(check);
    }

    private void getRightJumpTrajectory()
    {
        //cycle through the 8 jumps in a random order and see if any are possible
        int a = UnityEngine.Random.Range(0, 12);
        int b = (UnityEngine.Random.Range(0, 2) == 1) ? 5 : 7;

        Debug.Log(decision.rightJumps.Count);
        for (int i = 1; i <= 13; i++)
        {
            rightJumpTrajectory = tryRightJumpTrajectory(a);

            if (rightJumpTrajectory.doesExist())
                decision.rightJumps.Add(encodeJump("right ", a, rightJumpTrajectory.endLocation()));

            a = (a + b) % 13;
        }
    }

    private void getLeftJumpTrajectory()
    {
        //cycle through the 8 jumps in a randpm order and see if any are possible
        int a = UnityEngine.Random.Range(0, 12);
        int b = (UnityEngine.Random.Range(0, 2) == 1) ? 5 : 7;

        for (int i = 1; i <= 13; i++)
        {
            leftJumpTrajectory = tryLeftJumpTrajectory(a);

            if (leftJumpTrajectory.doesExist())
                decision.leftJumps.Add(encodeJump("left ", a, leftJumpTrajectory.endLocation()));

            a = (a + b) % 13;
        }
    }

    private locationExists tryRightJumpTrajectory(int i)
    {
        //normal jump where none of the initial colliders in the jump will collide with an obstacle during upward ascent
        if (i <= 3 && !rightJumpList[i][1] && !rightJumpList[i][2] && !rightJumpList[i][3] && !rightJumpList[i][4] && !rightJumpList[i][5] && !rightJumpList[i][6])
        {
            for (int collider = 7; collider < rightJumpList[i].Count; collider++)
            {
                //if a downward descent collider is colliding with the map (bool equal to true)
                if (rightJumpList[i][collider])
                {
                    //check this a different platform from the one the player is already standing on
                    obstacles = rightJumpPathColliders[i].transform.GetChild(collider).transform.GetComponent<RightPathCollider>().obstacles;
                    if (GameObject.ReferenceEquals(obstacles[obstacles.Count - 1].gameObject, AI.generalGround)) return new locationExists(false, Vector3.zero);

                    //return whether there is empty space above this platform (will be if it's a landable surface), and the location of the platform
                    return new locationExists(
                        !rightJumpPathGround[i].transform.GetChild(collider - 7).transform.GetComponent<PathCollider>().touchingObstacle,
                        rightJumpPathColliders[i].transform.GetChild(collider).transform.position
                    );
                }
            }
        }

        //double jump where none of the initial boxes in the jump will collide with an obstacle during upward ascent
        if (i >= 4 && i <= 8 && !rightJumpList[i][1] && !rightJumpList[i][2] && !rightJumpList[i][3] && !rightJumpList[i][4]
        && !rightJumpList[i][5] && !rightJumpList[i][6] && !rightJumpList[i][7] && !rightJumpList[i][8])
        {
            for (int collider = 9; collider < rightJumpList[i].Count; collider++)
            {
                //if a downward descent collider is colliding with the map (bool equal to true)
                if (rightJumpList[i][collider])
                {
                    //check this a different platform from the one the player is already standing on
                    obstacles = rightJumpPathColliders[i].transform.GetChild(collider).transform.GetComponent<RightPathCollider>().obstacles;
                    if (GameObject.ReferenceEquals(obstacles[obstacles.Count - 1].gameObject, AI.generalGround)) return new locationExists(false, Vector3.zero);

                    //return whether there is empty space above this platform (will be if it's a landable surface), and the location of the platform
                    return new locationExists(
                        !rightJumpPathGround[i].transform.GetChild(collider - 9).transform.GetComponent<PathCollider>().touchingObstacle,
                        rightJumpPathColliders[i].transform.GetChild(collider).transform.position
                    );
                }
            }
        }

        //double jump w/ u-turn where none of the initial boxes in the jump will collide with an obstacle during upward ascent
        if (i >= 9 && i <= 10 && !rightJumpList[i][1] && !rightJumpList[i][2] && !rightJumpList[i][3] && !rightJumpList[i][4]
        && !rightJumpList[i][5] && !rightJumpList[i][6] && !rightJumpList[i][7] && !rightJumpList[i][8])
        {
            for (int collider = 9; collider < rightJumpList[i].Count; collider++)
            {
                //if a downward descent collider is colliding with the map (bool equal to true)
                if (rightJumpList[i][collider])
                {
                    //check this a different platform from the one the player is already standing on
                    obstacles = rightJumpPathColliders[i].transform.GetChild(collider).transform.GetComponent<RightPathCollider>().obstacles;
                    if (GameObject.ReferenceEquals(obstacles[obstacles.Count - 1].gameObject, AI.generalGround)) return new locationExists(false, Vector3.zero);

                    //return whether there is empty space above this platform (will be if it's a landable surface), and the location of the platform
                    return new locationExists(
                        !rightJumpPathGround[i].transform.GetChild(collider - 9).transform.GetComponent<PathCollider>().touchingObstacle,
                        rightJumpPathColliders[i].transform.GetChild(collider).transform.position
                    );
                }
            }
        }

        //normal u-turn jump where none of the initial boxes in the jump will collide with an obstacle during upward ascent
        if (i >= 11 && i <= 12 && !rightJumpList[i][1] && !rightJumpList[i][2] && !rightJumpList[i][3] && !rightJumpList[i][4]
        && !rightJumpList[i][5])
        {
            for (int collider = 6; collider < rightJumpList[i].Count; collider++)
            {
                //if a downward descent collider is colliding with the map (bool equal to true)
                if (rightJumpList[i][collider])
                {
                    //check this a different platform from the one the player is already standing on
                    obstacles = rightJumpPathColliders[i].transform.GetChild(collider).transform.GetComponent<RightPathCollider>().obstacles;
                    if (GameObject.ReferenceEquals(obstacles[obstacles.Count - 1].gameObject, AI.generalGround)) return new locationExists(false, Vector3.zero);

                    //return whether there is empty space above this platform (will be if it's a landable surface), and the location of the platform
                    return new locationExists(
                        !rightJumpPathGround[i].transform.GetChild(collider - 6).transform.GetComponent<PathCollider>().touchingObstacle,
                        rightJumpPathColliders[i].transform.GetChild(collider).transform.position
                    );
                }
            }
        }

        //an obstacle would be in the way during this jump path
        return new locationExists(false, Vector3.zero);
    }

    private locationExists tryLeftJumpTrajectory(int i)
    {
        //normal jump where none of the initial colliders in the jump will collide with an obstacle during upward ascent
        if (i <= 3 && !leftJumpList[i][1] && !leftJumpList[i][2] && !leftJumpList[i][3] && !leftJumpList[i][4] && !leftJumpList[i][5] && !leftJumpList[i][6])
        {
            for (int collider = 7; collider < leftJumpList[i].Count; collider++)
            {
                //if a downward descent collider is colliding with the map (bool equal to true)
                if (leftJumpList[i][collider])
                {
                    //check this a different platform from the one the player is already standing on
                    obstacles = leftJumpPathColliders[i].transform.GetChild(collider).transform.GetComponent<LeftPathCollider>().obstacles;
                    if (GameObject.ReferenceEquals(obstacles[obstacles.Count - 1].gameObject, AI.generalGround)) return new locationExists(false, Vector3.zero);

                    //return whether there is empty space above this platform (will be if it's a landable surface), and the location of the platform
                    return new locationExists(
                        !leftJumpPathGround[i].transform.GetChild(collider - 7).transform.GetComponent<PathCollider>().touchingObstacle,
                        leftJumpPathColliders[i].transform.GetChild(collider).transform.position
                    );
                }
            }
        }

        //double jump where none of the initial boxes in the jump will collide with an obstacle during upward ascent
        if (i >= 4 && i <= 8 && !leftJumpList[i][1] && !leftJumpList[i][2] && !leftJumpList[i][3] && !leftJumpList[i][4]
        && !leftJumpList[i][5] && !leftJumpList[i][6] && !leftJumpList[i][7] && !leftJumpList[i][8])
        {
            for (int collider = 9; collider < leftJumpList[i].Count; collider++)
            {
                //if a downward descent collider is colliding with the map (bool equal to true)
                if (leftJumpList[i][collider])
                {
                    //check this a different platform from the one the player is already standing on
                    obstacles = leftJumpPathColliders[i].transform.GetChild(collider).transform.GetComponent<LeftPathCollider>().obstacles;
                    if (GameObject.ReferenceEquals(obstacles[obstacles.Count - 1].gameObject, AI.generalGround)) return new locationExists(false, Vector3.zero);

                    //return whether there is empty space above this platform (will be if it's a landable surface), and the location of the platform
                    return new locationExists(
                        !leftJumpPathGround[i].transform.GetChild(collider - 9).transform.GetComponent<PathCollider>().touchingObstacle,
                        leftJumpPathColliders[i].transform.GetChild(collider).transform.position
                    );
                }
            }
        }

        //double jump w/ u-turn where none of the initial boxes in the jump will collide with an obstacle during upward ascent
        if (i >= 9 && i <= 10 && !leftJumpList[i][1] && !leftJumpList[i][2] && !leftJumpList[i][3] && !leftJumpList[i][4]
        && !leftJumpList[i][5] && !leftJumpList[i][6] && !leftJumpList[i][7] && !leftJumpList[i][8])
        {
            for (int collider = 9; collider < leftJumpList[i].Count; collider++)
            {
                //if a downward descent collider is colliding with the map (bool equal to true)
                if (leftJumpList[i][collider])
                {
                    //check this a different platform from the one the player is already standing on
                    obstacles = leftJumpPathColliders[i].transform.GetChild(collider).transform.GetComponent<LeftPathCollider>().obstacles;
                    if (GameObject.ReferenceEquals(obstacles[obstacles.Count - 1].gameObject, AI.generalGround)) return new locationExists(false, Vector3.zero);

                    //return whether there is empty space above this platform (will be if it's a landable surface), and the location of the platform
                    return new locationExists(
                        !leftJumpPathGround[i].transform.GetChild(collider - 9).transform.GetComponent<PathCollider>().touchingObstacle,
                        leftJumpPathColliders[i].transform.GetChild(collider).transform.position
                    );
                }
            }
        }

        //normal u-turn jump where none of the initial boxes in the jump will collide with an obstacle during upward ascent
        if (i >= 11 && i <= 12 && !leftJumpList[i][1] && !leftJumpList[i][2] && !leftJumpList[i][3] && !leftJumpList[i][4]
        && !leftJumpList[i][5])
        {
            for (int collider = 6; collider < leftJumpList[i].Count; collider++)
            {
                //if a downward descent collider is colliding with the map (bool equal to true)
                if (leftJumpList[i][collider])
                {
                    //check this a different platform from the one the player is already standing on
                    obstacles = leftJumpPathColliders[i].transform.GetChild(collider).transform.GetComponent<LeftPathCollider>().obstacles;
                    if (GameObject.ReferenceEquals(obstacles[obstacles.Count - 1].gameObject, AI.generalGround)) return new locationExists(false, Vector3.zero);

                    //return whether there is empty space above this platform (will be if it's a landable surface), and the location of the platform
                    return new locationExists(
                        !leftJumpPathGround[i].transform.GetChild(collider - 6).transform.GetComponent<PathCollider>().touchingObstacle,
                        leftJumpPathColliders[i].transform.GetChild(collider).transform.position
                    );
                }
            }
        }

        //an obstacle would be in the way during this jump path
        return new locationExists(false, Vector3.zero);
    }

    private Jump encodeJump(string dir, int jumpIndex, Vector3 landing)
    {
        if (jumpIndex == 0)
            return new Jump(dir + "jump", 8.0f, 0f, 0f, landing);
        else if (jumpIndex == 1)
            return new Jump(dir + "jump", 6.4f, 0f, 0f, landing);
        else if (jumpIndex == 2)
            return new Jump(dir + "jump", 5.6f, 0f, 0f, landing);
        else if (jumpIndex == 3)
            return new Jump(dir + "jump", 4.8f, 0f, 0f, landing);
        else if (jumpIndex == 4)
            return new Jump(dir + "double jump", 8.0f, 0.5f, 6.4f + boost, landing);
        else if (jumpIndex == 5)
            return new Jump(dir + "double jump", 4.8f, 0.5f, 6.4f + boost, landing);
        else if (jumpIndex == 6)
            return new Jump(dir + "double jump", 7.0f, 0.3f, 5.5f + boost, landing);
        else if (jumpIndex == 7)
            return new Jump(dir + "double jump", 2.0f, 0.6f, 3.6f + boost, landing);
        else if (jumpIndex == 8)
            return new Jump(dir + "double jump", 6.4f, 0.6f, 6.4f + boost, landing);
        else if (jumpIndex == 9)
            return new Jump(dir + "u-turn", 8.0f, 0.6f, 6.4f + boost, landing);
        else if (jumpIndex == 10)
            return new Jump(dir + "u-turn", 8.0f, 0.7f, 8.0f + boost, landing);
        else if (jumpIndex == 11)
            return new Jump(dir + "mini u-turn", 8.0f, 0.35f, 8.0f, landing);
        else if (jumpIndex == 12)
            return new Jump(dir + "mini u-turn", 7.0f, 0.42f, 7.0f, landing);
        else
        {
            Debug.LogWarning("jump has not been coded for");
            return new Jump(dir + "jump", 8.0f + boost, 0f, 0f, transform.position + new Vector3(0, 2, 0));
        }
    }

    private void generateGroundChecks()
    {
        //for normal jumps
        for (int i = 0; i <= 1; i++)
        {
            //for diff paths
            for (int j = 0; j <= 3; j++)
            {
                int childCount = transform.GetChild(i).transform.GetChild(j).childCount;
                //for all colliders
                for (int k = 0; k < childCount - 1; k++)
                {
                    Vector3 pos = transform.GetChild(i).transform.GetChild(j).transform.GetChild(k).transform.position;

                    transform.GetChild(i).transform.GetChild(j).transform.GetChild(k).transform.position =
                    new Vector3(transform.GetChild(i).transform.GetChild(j).transform.GetChild(k + 1).position.x, pos.y, pos.z);

                    Transform col = transform.GetChild(i).transform.GetChild(j).transform.GetChild(k);

                    if (col.transform.GetComponent<RightPathCollider>())
                        Destroy(col.transform.GetComponent<RightPathCollider>());
                    if (col.transform.GetComponent<LeftPathCollider>())
                        Destroy(col.transform.GetComponent<LeftPathCollider>());

                    col.gameObject.AddComponent<PathCollider>();
                }

                Destroy(transform.GetChild(i).transform.GetChild(j).transform.GetChild(childCount - 1).gameObject);

                for (int a = 0; a <= 5; a++)
                {
                    Destroy(transform.GetChild(i).transform.GetChild(j).transform.GetChild(a).gameObject);
                }
            }
        }

        //for double jumps
        for (int i = 2; i <= 3; i++)
        {
            for (int j = 0; j <= 4; j++)
            {
                int childCount = transform.GetChild(i).transform.GetChild(j).childCount;
                for (int k = 0; k < childCount - 1; k++)
                {
                    Vector3 pos = transform.GetChild(i).transform.GetChild(j).transform.GetChild(k).transform.position;
                    Transform col = transform.GetChild(i).transform.GetChild(j).transform.GetChild(k);

                    transform.GetChild(i).transform.GetChild(j).transform.GetChild(k).transform.position =
                    new Vector3(transform.GetChild(i).transform.GetChild(j).transform.GetChild(k + 1).position.x,
                    pos.y, pos.z);

                    if (col.transform.GetComponent<RightPathCollider>())
                        Destroy(col.transform.GetComponent<RightPathCollider>());
                    if (col.transform.GetComponent<LeftPathCollider>())
                        Destroy(col.transform.GetComponent<LeftPathCollider>());

                    col.gameObject.AddComponent<PathCollider>();
                }

                Destroy(transform.GetChild(i).transform.GetChild(j).transform.GetChild(childCount - 1).gameObject);

                for (int a = 0; a <= 7; a++)
                {
                    Destroy(transform.GetChild(i).transform.GetChild(j).transform.GetChild(a).gameObject);
                }
            }
        }

        //for u turns
        for (int i = 4; i <= 5; i++)
        {
            for (int j = 0; j <= 1; j++)
            {
                int childCount = transform.GetChild(i).transform.GetChild(j).childCount;
                for (int k = 0; k < childCount - 1; k++)
                {
                    Vector3 pos = transform.GetChild(i).transform.GetChild(j).transform.GetChild(k).transform.position;
                    Transform col = transform.GetChild(i).transform.GetChild(j).transform.GetChild(k);

                    transform.GetChild(i).transform.GetChild(j).transform.GetChild(k).transform.position =
                    new Vector3(transform.GetChild(i).transform.GetChild(j).transform.GetChild(k + 1).position.x,
                    pos.y, pos.z);

                    if (col.transform.GetComponent<RightPathCollider>())
                        Destroy(col.transform.GetComponent<RightPathCollider>());
                    if (col.transform.GetComponent<LeftPathCollider>())
                        Destroy(col.transform.GetComponent<LeftPathCollider>());

                    col.gameObject.AddComponent<PathCollider>();
                }

                Destroy(transform.GetChild(i).transform.GetChild(j).transform.GetChild(childCount - 1).gameObject);

                for (int a = 0; a <= 7; a++)
                {
                    Destroy(transform.GetChild(i).transform.GetChild(j).transform.GetChild(a).gameObject);
                }
            }

            for (int j = 2; j <= 3; j++)
            {
                int childCount = transform.GetChild(i).transform.GetChild(j).childCount;
                for (int k = 0; k < childCount - 1; k++)
                {
                    Vector3 pos = transform.GetChild(i).transform.GetChild(j).transform.GetChild(k).transform.position;
                    Transform col = transform.GetChild(i).transform.GetChild(j).transform.GetChild(k);

                    transform.GetChild(i).transform.GetChild(j).transform.GetChild(k).transform.position =
                    new Vector3(transform.GetChild(i).transform.GetChild(j).transform.GetChild(k + 1).position.x,
                    pos.y, pos.z);

                    if (col.transform.GetComponent<RightPathCollider>())
                        Destroy(col.transform.GetComponent<RightPathCollider>());
                    if (col.transform.GetComponent<LeftPathCollider>())
                        Destroy(col.transform.GetComponent<LeftPathCollider>());

                    col.gameObject.AddComponent<PathCollider>();
                }

                Destroy(transform.GetChild(i).transform.GetChild(j).transform.GetChild(childCount - 1).gameObject);

                for (int a = 0; a <= 4; a++)
                {
                    Destroy(transform.GetChild(i).transform.GetChild(j).transform.GetChild(a).gameObject);
                }
            }
        }

        for (int i = 0; i <= 5; i++)
        {
            transform.GetChild(i).name += " Ground";
        }
    }

    private void generateLargerColliders()
    {
        //for normal jumps
        for (int i = 0; i <= 1; i++)
        {
            //for diff paths
            for (int j = 0; j <= 3; j++)
            {
                int childCount = transform.GetChild(i).transform.GetChild(j).childCount;
                //for all colliders
                for (int k = 0; k < childCount; k++)
                {
                    //transform.GetChild(i).transform.GetChild(j).transform.GetChild(k).transform.
                }
            }
        }

        //for double jumps
        for (int i = 2; i <= 3; i++)
        {
            for (int j = 0; j <= 4; j++)
            {
                int childCount = transform.GetChild(i).transform.GetChild(j).childCount;
                for (int k = 0; k < childCount - 1; k++)
                {
                    Vector3 pos = transform.GetChild(i).transform.GetChild(j).transform.GetChild(k).transform.position;
                    Transform col = transform.GetChild(i).transform.GetChild(j).transform.GetChild(k);

                    transform.GetChild(i).transform.GetChild(j).transform.GetChild(k).transform.position =
                    new Vector3(transform.GetChild(i).transform.GetChild(j).transform.GetChild(k + 1).position.x,
                    pos.y, pos.z);

                    if (col.transform.GetComponent<RightPathCollider>())
                        Destroy(col.transform.GetComponent<RightPathCollider>());
                    if (col.transform.GetComponent<LeftPathCollider>())
                        Destroy(col.transform.GetComponent<LeftPathCollider>());

                    col.gameObject.AddComponent<PathCollider>();
                }

                Destroy(transform.GetChild(i).transform.GetChild(j).transform.GetChild(childCount - 1).gameObject);

                for (int a = 0; a <= 7; a++)
                {
                    Destroy(transform.GetChild(i).transform.GetChild(j).transform.GetChild(a).gameObject);
                }
            }
        }

        //for u turns
        for (int i = 4; i <= 5; i++)
        {
            for (int j = 0; j <= 1; j++)
            {
                int childCount = transform.GetChild(i).transform.GetChild(j).childCount;
                for (int k = 0; k < childCount - 1; k++)
                {
                    Vector3 pos = transform.GetChild(i).transform.GetChild(j).transform.GetChild(k).transform.position;
                    Transform col = transform.GetChild(i).transform.GetChild(j).transform.GetChild(k);

                    transform.GetChild(i).transform.GetChild(j).transform.GetChild(k).transform.position =
                    new Vector3(transform.GetChild(i).transform.GetChild(j).transform.GetChild(k + 1).position.x,
                    pos.y, pos.z);

                    if (col.transform.GetComponent<RightPathCollider>())
                        Destroy(col.transform.GetComponent<RightPathCollider>());
                    if (col.transform.GetComponent<LeftPathCollider>())
                        Destroy(col.transform.GetComponent<LeftPathCollider>());

                    col.gameObject.AddComponent<PathCollider>();
                }

                Destroy(transform.GetChild(i).transform.GetChild(j).transform.GetChild(childCount - 1).gameObject);

                for (int a = 0; a <= 7; a++)
                {
                    Destroy(transform.GetChild(i).transform.GetChild(j).transform.GetChild(a).gameObject);
                }
            }

            for (int j = 2; j <= 3; j++)
            {
                int childCount = transform.GetChild(i).transform.GetChild(j).childCount;
                for (int k = 0; k < childCount - 1; k++)
                {
                    Vector3 pos = transform.GetChild(i).transform.GetChild(j).transform.GetChild(k).transform.position;
                    Transform col = transform.GetChild(i).transform.GetChild(j).transform.GetChild(k);

                    transform.GetChild(i).transform.GetChild(j).transform.GetChild(k).transform.position =
                    new Vector3(transform.GetChild(i).transform.GetChild(j).transform.GetChild(k + 1).position.x,
                    pos.y, pos.z);

                    if (col.transform.GetComponent<RightPathCollider>())
                        Destroy(col.transform.GetComponent<RightPathCollider>());
                    if (col.transform.GetComponent<LeftPathCollider>())
                        Destroy(col.transform.GetComponent<LeftPathCollider>());

                    col.gameObject.AddComponent<PathCollider>();
                }

                Destroy(transform.GetChild(i).transform.GetChild(j).transform.GetChild(childCount - 1).gameObject);

                for (int a = 0; a <= 4; a++)
                {
                    Destroy(transform.GetChild(i).transform.GetChild(j).transform.GetChild(a).gameObject);
                }
            }
        }

        for (int i = 0; i <= 5; i++)
        {
            transform.GetChild(i).name += " Ground";
        }
    }

}
