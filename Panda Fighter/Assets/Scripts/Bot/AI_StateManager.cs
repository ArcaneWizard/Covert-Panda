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
    public Vector2 visitingPlaceLocation { get; private set; }

    private CentralController controller;
    private PathFinding pathFinding;

    private float timer;

    void Awake()
    {
        controller = transform.GetChild(0).GetComponent<CentralController>();
        pathFinding = transform.GetComponent<PathFinding>();
    }

    void Start()
    {
        StartCoroutine(configureWanderingState());
    }

    void Update()
    {
        if (state == AI_STATE.Wandering)
        {
            if (getSquaredDistanceBtwnVectors(transform.position, visitingPlaceLocation) < 16f)
                visitingPlaceLocation = getLocationOfNewPlaceToVisit();
        }
    }

    public IEnumerator configureWanderingState()
    {
        state = AI_STATE.Wandering;
        visitingPlaceLocation = getLocationOfNewPlaceToVisit();

        StartCoroutine(pathFinding.FindMultiplePaths(0.8f, visitingPlaceLocation));
        yield return new WaitForSeconds(0.8f + 2 * Time.deltaTime);

        pathFinding.debugPathInConsole(pathFinding.getChosenPath());
    }

    public void enterIdleState()
    {
        state = AI_STATE.Idle;
    }

    private Vector2 getLocationOfNewPlaceToVisit()
    {
        return placesToVisit.GetChild(UnityEngine.Random.Range(0, placesToVisit.childCount)).position;
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
