using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_StateManager : MonoBehaviour
{
    public AI_STATE AI_STATE { get { return state; } private set { state = value; } }
    [SerializeField]
    private AI_STATE state;

    [SerializeField]
    private Transform placesToVisit, target;
    public bool useTargetForTesting;
    public Vector2 visitingPlaceLocation { get; private set; }
    private CentralController controller;
    private AI_FollowPath AI_pathFollower;
    private PathFinding pathFinding;

    private float timer;

    void Awake()
    {
        controller = transform.GetChild(0).GetComponent<CentralController>();
        AI_pathFollower = transform.GetChild(0).GetComponent<AI_FollowPath>();
        pathFinding = transform.GetComponent<PathFinding>();
    }

    void Start()
    {
        enterWanderingStateAndWander();
    }

    void Update()
    {
        if (state == AI_STATE.Wandering && visitingPlaceLocation != null)
        {
            if (getSquaredDistanceBtwnVectors(transform.GetChild(0).position, visitingPlaceLocation) < 16f) { 
                AI_pathFollower.endJourney();
                enterWanderingStateAndWander();
                Debug.Log("VPL updated");
            }
        }
    }
    public void enterIdleState()
    {
        state = AI_STATE.Idle;
    }

    public void enterWanderingStateAndWander()
    {
        state = AI_STATE.Wandering;

        StartCoroutine(figureOutWhereToWanderTo((List<Node> path) => {
            AI_pathFollower.follow(path);
        }));
    }

    private IEnumerator figureOutWhereToWanderTo(System.Action<List<Node>> callbackOnFinish)
    {
        visitingPlaceLocation = (useTargetForTesting) 
            ? target.position 
            : placesToVisit.GetChild(UnityEngine.Random.Range(0, placesToVisit.childCount)).position;;

        StartCoroutine(pathFinding.FindMultiplePaths(2f, visitingPlaceLocation));

        while (pathFinding.getChosenPath() == null)
            yield return new WaitForSeconds(Time.deltaTime * 2);

        pathFinding.debugPathInConsole(pathFinding.getChosenPath());
        callbackOnFinish(pathFinding.getChosenPath());
    }

    private float getSquaredDistanceBtwnVectors(Vector2 a, Vector2 b)
    {
        Vector2 distanceBtwnVectors = a - b;
        return distanceBtwnVectors.x * distanceBtwnVectors.x + distanceBtwnVectors.y * distanceBtwnVectors.y;
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(visitingPlaceLocation, 1);
    }
}

public enum AI_STATE
{
    Wandering,
    Attack,
    Idle,
    Flee
}
