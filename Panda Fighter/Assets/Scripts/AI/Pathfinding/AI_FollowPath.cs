
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;

public class AI_FollowPath : MonoBehaviour
{
    private AI_Controller controller;
    private PathFinding pathFinding;

    //VALUES: pending start, started, in progress, ended, got lost
    public Journey journey { get; private set; }

    private List<Node> path;
    private int pathProgress;

    void Awake()
    {
        controller = transform.GetComponent<AI_Controller>();
        pathFinding = transform.parent.GetComponent<PathFinding>();

        journey = Journey.Ended;
    }

    public IEnumerator startJourney(Vector3 destination)
    {
        journey = Journey.PendingStart;

        //scan available routes for 2 seconds
        StartCoroutine(pathFinding.FindMultiplePaths(2f, destination));

        while (pathFinding.getChosenPath() == null)
            yield return new WaitForSeconds(Time.deltaTime * 2);

        pathFinding.debugPathInConsole(pathFinding.getChosenPath());

        //end journey if the route is already over, ie. alien is already where it needs to be
        if (pathFinding.getChosenPath().Count == 0)
            journey = Journey.Ended;

        //don't do anything if the AI prematurely ended its journey (ex. cuz the AI state changed)
        if (journey == Journey.Ended)
            yield break;

        //reset variables
        journey = Journey.Started;
        this.path = pathFinding.getChosenPath();
        pathProgress = 0;

        //head towards starting path Node
        if (transform.position.x < path[0].transform.position.x)
            controller.SetDirection(1);
        else
            controller.SetDirection(-1);
    }

    public void tick()
    {
        if (journey == Journey.InProgress && controller.currAction == null && controller.isGrounded && controller.isTouchingMap)
        {
            int dir = Math.Sign(path[pathProgress].transform.position.x - transform.position.x);
            controller.SetDirection(dir);
        }
    }

    public void endJourney() => journey = Journey.Ended;
    public bool gotLost() => journey == Journey.GotLost;

    public Vector3 nextPathNode()
    {
        if (path != null && pathProgress < path.Count)
            return path[pathProgress].transform.position;

        return Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != 8)
            return;

        if (journey == Journey.Started)
        {
            if (col.transform == path[0].transform)
                journey = Journey.InProgress;
            else
                getBackOnIntendedPath(col.transform);
        }

        if (journey == Journey.InProgress)
        {
            foreach (Transform neighbourNode in col.transform)
            {
                TrajectoryPath trajectory = neighbourNode.GetComponent<TrajectoryPath>();

                if (pathProgress < path.Count - 1 && path[pathProgress + 1].transform == trajectory.getChainedZone())
                {
                    controller.StartAction(trajectory.ConvertToAction(), col.transform);
                    pathProgress++;
                    break;
                }
            }
        }
    }

    //Helper method: find trajectory that gets you back on the main intended path, 
    //otherwise teleport back on path
    private void getBackOnIntendedPath(Transform decisionZone)
    {
        bool foundReroute = false;
        foreach (Transform neighborZone in decisionZone)
        {
            TrajectoryPath trajectory = neighborZone.transform.GetComponent<TrajectoryPath>();

            if (path[0].transform == trajectory.getChainedZone())
            {
                controller.StartAction(trajectory.ConvertToAction(), decisionZone);
                foundReroute = true;
                break;
            }
        }

        if (!foundReroute)
            transform.position = new Vector3(path[0].transform.position.x,
            path[0].transform.position.y + 0.5f, transform.position.z);
    }
}

public enum Journey
{
    PendingStart,
    Started,
    InProgress,
    Ended,
    GotLost
}