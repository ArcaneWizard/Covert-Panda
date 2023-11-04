
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;

/// <summary> Offers methods to tell the AI to get a certain destination on the map pronto. 
/// A path is found using A* pathfinding and randomness.  </summary>
public class AI_FollowPath : MonoBehaviour
{
    private AI_Controller controller;
    private PathFinding pathFinding;

    private List<Node> currPath;
    public JourneyStatus journeyStatus { get; private set; }
    private int pathProgress;

    void Awake()
    {
        controller = transform.GetComponent<AI_Controller>();
        pathFinding = transform.parent.GetComponent<PathFinding>();
        journeyStatus = JourneyStatus.Ended;
    }

    public IEnumerator StartJourney(Vector3 destination)
    {
        journeyStatus = JourneyStatus.PendingStart;

        //scan available routes for 2 seconds
        StartCoroutine(pathFinding.FindMultiplePaths(2f, destination));

        while (pathFinding.ChosenPath == null)
            yield return new WaitForSeconds(Time.deltaTime * 2);

        pathFinding.PrintPathInConsole(pathFinding.ChosenPath);

        // immediately end this journey if no route is found
        if (pathFinding.ChosenPath.Count == 0)
            journeyStatus = JourneyStatus.Ended;

        // don't do anything if the AI prematurely ended its journey (ex. cuz the AI state changed)
        if (journeyStatus == JourneyStatus.Ended)
            yield break;

        // setup
        journeyStatus = JourneyStatus.Started;
        currPath = pathFinding.ChosenPath;
        pathProgress = 0;

        // head towards starting path's starting node
        if (transform.position.x < currPath[0].transform.position.x)
            controller.SetDirection(1);
        else
            controller.SetDirection(-1);
    }

    /// <summary> To be invoked every frame </summary>
    public void Tick()
    {
        if (journeyStatus == JourneyStatus.InProgress && controller.CurrAction == null 
            && controller.isGrounded && controller.isTouchingMap)
        {
            int dir = Math.Sign(currPath[pathProgress].transform.position.x - transform.position.x);
            controller.SetDirection(dir);
        }
    }

    public void TerminateJourney() => journeyStatus = JourneyStatus.Ended;
    public bool IsAiLost => journeyStatus == JourneyStatus.GotLost;

    /// <summary> Get the next node in the current path this AI needs to head to  </summary>
    public Vector3 GetNextPathNode()
    {
        if (currPath != null && pathProgress < currPath.Count)
            return currPath[pathProgress].transform.position;

        return Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != 8)
            return;

        if (journeyStatus == JourneyStatus.Started)
        {
            if (col.transform == currPath[0].transform)
                journeyStatus = JourneyStatus.InProgress;
            else
                getBackOnIntendedPath(col.transform);
        }

        if (journeyStatus == JourneyStatus.InProgress)
        {
            foreach (Transform neighbourNode in col.transform)
            {
                TrajectoryPath trajectory = neighbourNode.GetComponent<TrajectoryPath>();

                if (pathProgress < currPath.Count - 1 && currPath[pathProgress + 1].transform == trajectory.getChainedZone())
                {
                    controller.StartAction(trajectory.ConvertToAction(), col.transform);
                    pathProgress++;
                    break;
                }
            }
        }
    }

    // Helper method: find trajectory that gets you back on the main intended path, 
    // otherwise, teleport back on path. functionality will change in the future
    private void getBackOnIntendedPath(Transform decisionZone)
    {
        bool foundReroute = false;
        foreach (Transform neighborZone in decisionZone)
        {
            TrajectoryPath trajectory = neighborZone.transform.GetComponent<TrajectoryPath>();

            if (currPath[0].transform == trajectory.getChainedZone())
            {
                controller.StartAction(trajectory.ConvertToAction(), decisionZone);
                foundReroute = true;
                break;
            }
        }

        if (!foundReroute)
            transform.position = new Vector3(currPath[0].transform.position.x,
            currPath[0].transform.position.y + 0.5f, transform.position.z);
    }
}

public enum JourneyStatus
{
    PendingStart,
    Started,
    InProgress,
    Ended,
    GotLost
}