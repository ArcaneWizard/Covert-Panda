using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;

public class JumpPath : MonoBehaviour
{
    private List<List<bool>> rightJumpList = new List<List<bool>>();
    private List<List<bool>> leftJumpList = new List<List<bool>>();
    public List<List<bool>> rightJumpGroundList = new List<List<bool>>();
    public List<List<bool>> leftJumpGroundList = new List<List<bool>>();

    public List<Transform> rightJumpPathColliders = new List<Transform>();
    public List<Transform> leftJumpPathColliders = new List<Transform>();
    public List<Transform> rightJumpPathGround = new List<Transform>();
    public List<Transform> leftJumpPathGround = new List<Transform>();

    public List<bool> rightJump100;
    public List<bool> rightJump80;
    public List<bool> rightJump60;
    public List<bool> rightJump40;
    public List<bool> rightDoubleJump1;
    public List<bool> rightDoubleJump2;
    public List<bool> rightDoubleJump3;
    public List<bool> rightDoubleJump4;

    public List<bool> leftJump100;
    public List<bool> leftJump80;
    public List<bool> leftJump60;
    public List<bool> leftJump40;
    public List<bool> leftDoubleJump1;
    public List<bool> leftDoubleJump2;
    public List<bool> leftDoubleJump3;
    public List<bool> leftDoubleJump4;

    private NewBotAI AI;
    private DecisionMaking decision;

    private int groundDiff;
    private float boost = 0.4f; //jump speed boost
    private List<GameObject> obstacles = new List<GameObject>();

    void Awake()
    {
        AI = transform.parent.transform.GetChild(0).transform.GetComponent<NewBotAI>();
        decision = transform.parent.transform.GetChild(0).transform.GetComponent<DecisionMaking>();

        rightJumpList.Add(rightJump100);
        rightJumpList.Add(rightJump80);
        rightJumpList.Add(rightJump60);
        rightJumpList.Add(rightJump40);
        rightJumpList.Add(rightDoubleJump1);
        rightJumpList.Add(rightDoubleJump2);
        rightJumpList.Add(rightDoubleJump3);
        rightJumpList.Add(rightDoubleJump4);

        leftJumpList.Add(leftJump100);
        leftJumpList.Add(leftJump80);
        leftJumpList.Add(leftJump60);
        leftJumpList.Add(leftJump40);
        leftJumpList.Add(leftDoubleJump1);
        leftJumpList.Add(leftDoubleJump2);
        leftJumpList.Add(leftDoubleJump3);
        leftJumpList.Add(leftDoubleJump4);
    }

    void Start()
    {
        //generateGroundChecks();

        StartCoroutine(getJumpTrajectory());
    }

    private IEnumerator getJumpTrajectory()
    {
        AI.findWalls();
        decision.rightJumps.Clear();
        decision.leftJumps.Clear();

        if (AI.grounded && AI.touchingMap && (AI.movementDirX == 1 || AI.movementDirX == 0))
            getRightJumpTrajectory();

        else if (AI.grounded && AI.touchingMap && AI.movementDirX == -1)
            getLeftJumpTrajectory();

        if (AI.grounded && AI.touchingMap)
            decision.decideWhetherToJump();

        yield return new WaitForSeconds(0.4f);
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

    private void generateGroundChecks()
    {
        for (int i = 0; i <= 1; i++)
        {
            for (int j = 0; j <= 3; j++)
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

        for (int i = 2; i <= 3; i++)
        {
            for (int j = 0; j <= 3; j++)
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

                for (int a = 0; a <= 6; a++)
                {
                    Destroy(transform.GetChild(i).transform.GetChild(j).transform.GetChild(a).gameObject);
                }
            }
        }

        for (int i = 0; i <= 3; i++)
        {
            transform.GetChild(i).name += "Ground";
            for (int j = 0; j <= 3; j++)
            {
                transform.GetChild(i).transform.GetChild(j).name += " Ground";
            }
        }
    }

    private void getRightJumpTrajectory()
    {
        //cycle through the 8 jumps in a random order and see if any are possible
        int a = UnityEngine.Random.Range(0, 8);
        int b = UnityEngine.Random.Range(0, 8);

        for (int i = 1; i <= 8; i++)
        {
            if (determineIfRightJumpIsPossible(a))
                decision.rightJumps.Add(encodeRightJump(a));

            a = (a + b) % 8;
        }
    }

    private void getLeftJumpTrajectory()
    {
        //cycle through the 8 jumps in a randpm order and see if any are possible
        int a = UnityEngine.Random.Range(0, 8);
        int b = UnityEngine.Random.Range(0, 8);

        for (int i = 1; i <= 8; i++)
        {
            if (determineIfLeftJumpIsPossible(a))
                decision.leftJumps.Add(encodeLeftJump(a));

            a = (a + b) % 8;
        }
    }

    private bool determineIfRightJumpIsPossible(int i)
    {
        //general pattern: 
        //1) check none of the initial boxes in the jump collide with an obstacle during upward ascent
        //2) then check if there is ground to land on in the downward descent 
        // For the descent collider (collider A) that detects an object (object A)
        // confirm the special collider above Collider A is in air, as that means object A is a surface that can be landed on
        // also check that object A isn't literally the same ground the alien is on

        //normal jump where none of the initial boxes in the jump collide with an obstacle during upward ascent
        if (i <= 3 && !rightJumpList[i][1] && !rightJumpList[i][2] && !rightJumpList[i][3] && !rightJumpList[i][4] && !rightJumpList[i][5])
        {
            for (int collider = 6; collider < rightJumpList[i].Count; collider++)
            {
                if (rightJumpList[i][collider])
                {
                    obstacles = rightJumpPathColliders[i].transform.GetChild(collider).transform.GetComponent<RightPathCollider>().obstacles;
                    if (GameObject.ReferenceEquals(obstacles[obstacles.Count - 1].gameObject, AI.generalGround)) return false;
                    return !rightJumpPathGround[i].transform.GetChild(collider - 6).transform.GetComponent<PathCollider>().touchingObstacle;
                }
            }
        }

        //double jump where none of the initial boxes in the jump collide with an obstacle during upward ascent
        if (i >= 4 && i <= 7 && !rightJumpList[i][1] && !rightJumpList[i][2] && !rightJumpList[i][3] && !rightJumpList[i][4]
        && !rightJumpList[i][5] && !rightJumpList[i][6] && !rightJumpList[i][7])
        {
            for (int collider = 8; collider < rightJumpList[i].Count; collider++)
            {
                if (rightJumpList[i][collider])
                {
                    obstacles = rightJumpPathColliders[i].transform.GetChild(collider).transform.GetComponent<RightPathCollider>().obstacles;
                    if (GameObject.ReferenceEquals(obstacles[obstacles.Count - 1].gameObject, AI.generalGround)) return false;
                    return !rightJumpPathGround[i].transform.GetChild(collider - 8).transform.GetComponent<PathCollider>().touchingObstacle;
                }
            }
        }

        //an obstacle would be in the way during this jump path
        return false;
    }

    private bool determineIfLeftJumpIsPossible(int i)
    {
        //general pattern: 
        //1) check none of the initial boxes in the jump collide with an obstacle during upward ascent
        //2) then check if there is ground to land on in the downward descent (ground colliders are above their respective actual ones)

        //normal jump where none of the initial boxes in the jump collide with an obstacle during upward ascent
        if (i <= 3 && !leftJumpList[i][1] && !leftJumpList[i][2] && !leftJumpList[i][3] && !leftJumpList[i][4] && !leftJumpList[i][5])
        {
            for (int collider = 6; collider < leftJumpList[i].Count; collider++)
            {
                if (leftJumpList[i][collider])
                {
                    obstacles = leftJumpPathColliders[i].transform.GetChild(collider).transform.GetComponent<LeftPathCollider>().obstacles;
                    if (i == 3)
                    {
                        Debug.Log(collider);
                        Debug.Log(GameObject.ReferenceEquals(obstacles[obstacles.Count - 1].gameObject, AI.generalGround));
                        Debug.Log(!leftJumpPathGround[i].transform.GetChild(collider - 6).transform.GetComponent<PathCollider>().touchingObstacle);
                    }
                    if (GameObject.ReferenceEquals(obstacles[obstacles.Count - 1].gameObject, AI.generalGround)) return false;
                    return !leftJumpPathGround[i].transform.GetChild(collider - 6).transform.GetComponent<PathCollider>().touchingObstacle;
                }
            }
        }

        //double jump where none of the initial boxes in the jump collide with an obstacle during upward ascent
        if (i >= 4 && i <= 7 && !leftJumpList[i][1] && !leftJumpList[i][2] && !leftJumpList[i][3] && !leftJumpList[i][4]
        && !leftJumpList[i][5] && !leftJumpList[i][6] && !leftJumpList[i][7])
        {
            for (int collider = 8; collider < leftJumpList[i].Count; collider++)
            {
                if (leftJumpList[i][collider])
                {
                    obstacles = leftJumpPathColliders[i].transform.GetChild(collider).transform.GetComponent<LeftPathCollider>().obstacles;
                    if (GameObject.ReferenceEquals(obstacles[obstacles.Count - 1].gameObject, AI.generalGround)) return false;
                    return !leftJumpPathGround[i].transform.GetChild(collider - 8).transform.GetComponent<PathCollider>().touchingObstacle;
                }
            }
        }

        //an obstacle would be in the way during this jump path
        return false;
    }

    private Jump encodeRightJump(int jumpIndex)
    {
        if (jumpIndex == 0)
            return new Jump("right jump", 8.0f + boost, 0f, 0f);
        else if (jumpIndex == 1)
            return new Jump("right jump", 6.4f + boost, 0f, 0f);
        else if (jumpIndex == 2)
            return new Jump("right jump", 4.8f + boost, 0f, 0f);
        else if (jumpIndex == 3)
            return new Jump("right jump", 3.2f + boost, 0f, 0f);
        else if (jumpIndex == 4)
            return new Jump("right double jump", 8.0f, 0.5f, 6.4f + boost);
        else if (jumpIndex == 5)
            return new Jump("right double jump", 4.8f, 0.5f, 6.4f + boost);
        else if (jumpIndex == 6)
            return new Jump("right double jump", 2.4f, 0.3f, 3.6f + boost);
        else if (jumpIndex == 7)
            return new Jump("right double jump", 0f, 0.6f, 3.6f + boost);
        else
        {
            Debug.LogWarning("jump has not been coded for");
            return new Jump("right jump", 8.0f + boost, 0f, 0f);
        }
    }

    private Jump encodeLeftJump(int jumpIndex)
    {
        if (jumpIndex == 0)
            return new Jump("left jump", 8.0f + boost, 0f, 0f);
        else if (jumpIndex == 1)
            return new Jump("left jump", 6.4f + boost, 0f, 0f);
        else if (jumpIndex == 2)
            return new Jump("left jump", 4.8f + boost, 0f, 0f);
        else if (jumpIndex == 3)
            return new Jump("left jump", 3.2f + boost, 0f, 0f);
        else if (jumpIndex == 4)
            return new Jump("left double jump", 8.0f, 0.5f, 6.4f + boost);
        else if (jumpIndex == 5)
            return new Jump("left double jump", 4.8f, 0.5f, 6.4f + boost);
        else if (jumpIndex == 6)
            return new Jump("left double jump", 2.4f, 0.3f, 3.6f + boost);
        else if (jumpIndex == 7)
            return new Jump("left double jump", 0f, 0.6f, 3.6f + boost);
        else
        {
            Debug.LogWarning("jump has not been coded for");
            return new Jump("left jump", 8.0f + boost, 0f, 0f);
        }
    }

}
