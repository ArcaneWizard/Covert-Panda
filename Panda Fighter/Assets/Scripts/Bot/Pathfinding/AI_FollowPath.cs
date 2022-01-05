
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
    public string journey { get; private set; }

    private List<Node> path;
    private int pathProgress;

    void Awake()
    {
        controller = transform.GetComponent<AI_Controller>();
        pathFinding = transform.parent.GetComponent<PathFinding>();

        journey = "ended";
    }

    public IEnumerator startJourney(Vector3 destination)
    {
        Debug.Log("bruh");
        journey = "pending start";

        //scan available routes for 2 seconds
        StartCoroutine(pathFinding.FindMultiplePaths(2f, destination));

        while (pathFinding.getChosenPath() == null)
            yield return new WaitForSeconds(Time.deltaTime * 2);

        pathFinding.debugPathInConsole(pathFinding.getChosenPath());

        //end journey if the route is already over, ie. alien is already where it needs to be
        if (pathFinding.getChosenPath().Count == 0)
            journey = "ended";

        //don't do anything if the AI prematurely ended its journey (ex. cuz the AI state changed)
        if (journey == "ended")
        {
            Debug.Log("test ended");
            yield break;
        }

        Debug.Log("test error");

        //reset variables
        journey = "started";
        this.path = pathFinding.getChosenPath();
        pathProgress = 0;

        //head towards starting path Node
        if (transform.position.x < path[0].transform.position.x)
            controller.setDirection(1);
        else
            controller.setDirection(-1);
    }

    public void tick()
    {
        if (journey == "in progress" && controller.actionProgress == "finished" && controller.isGrounded && controller.isTouchingMap)
            controller.setDirection(Math.Sign(path[pathProgress].transform.position.x - transform.position.x));
    }

    public void endJourney()
    {
        journey = "ended";
        //DebugGUI.debugText8 = "journey ended";
    }

    public bool gotLost() => journey == "got lost";

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

        if (journey == "started")
        {
            if (col.transform == path[0].transform)
                journey = "in progress";
            else
                getBackOnIntendedPath(col.transform);
        }

        if (journey == "in progress")
        {
            foreach (Transform neighbourNode in col.transform)
            {
                TestingTrajectories trajectory = neighbourNode.GetComponent<TestingTrajectories>();

                if (pathProgress < path.Count - 1 && path[pathProgress + 1].transform == trajectory.getChainedZone())
                {
                    controller.BeginAction(trajectory.convertToAction(), col.transform);
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
        Debug.Log("getting back on intended path");
        bool foundReroute = false;
        foreach (Transform neighborZone in decisionZone)
        {
            TestingTrajectories trajectory = neighborZone.transform.GetComponent<TestingTrajectories>();

            if (path[0].transform == trajectory.getChainedZone())
            {
                controller.BeginAction(trajectory.convertToAction(), decisionZone);
                foundReroute = true;
                break;
            }
        }

        if (!foundReroute)
            transform.position = new Vector3(path[0].transform.position.x,
            path[0].transform.position.y + 0.5f, transform.position.z);
    }


}
