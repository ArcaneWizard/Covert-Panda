
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.CompilerServices;

public class AI_FollowPath : MonoBehaviour
{
    private AI_Controller controller;
    private PathFinding pathFinding;
    private System.Random random, random2;

    //VALUES: pending start, started, in progress, ended, got lost
    public string journey { get; private set; }

    private List<Node> path;
    private int pathProgress;

    void Awake()
    {
        controller = transform.GetComponent<AI_Controller>();
        pathFinding = transform.parent.GetComponent<PathFinding>();
        random = new System.Random();
        random2 = new System.Random();

        journey = "ended";
    }

    //AI heads to the specified destination
    public IEnumerator startJourney(Vector3 destination)
    {
        journey = "pending start";

        //scan available routes for 2 seconds
        StartCoroutine(pathFinding.FindMultiplePaths(2f, destination));

        while (pathFinding.getChosenPath() == null)
            yield return new WaitForSeconds(Time.deltaTime * 2);

        pathFinding.debugPathInConsole(pathFinding.getChosenPath());

        //don't do anything if the AI prematurely ended its journey (ex. cuz of a state change)
        if (journey == "ended")
            yield return null;

        //reset variables
        journey = "started";
        this.path = pathFinding.getChosenPath();
        pathProgress = 0;

        //head towards starting path Node
        if (transform.GetChild(0).position.x < path[0].transform.position.x)
            controller.setDirection(1);
        else
            controller.setDirection(-1);
    }

    //To be called every frame while AI is on a journey
    public void tick()
    {
        if (journey == "in progress" && controller.actionProgress == "finished" && controller.isGrounded && controller.isTouchingMap)
            controller.setDirection(Math.Sign(path[pathProgress].transform.position.x - transform.position.x));
    }

    //AI no longer heads toward a target destination
    public void endJourney()
    {
        journey = "ended";
        DebugGUI.debugText8 = "journey ended";
    }

    //AI accidently got off course on its path
    public bool gotLost() => journey == "got lost";

    //AI passes a node, gets potential routes from that node to neighbor nodes, and then
    //figures out which route allows it to stay on its intended path
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
            bool stayedOnPath = false;
            foreach (Transform neighbourNode in col.transform)
            {
                TestingTrajectories trajectory = neighbourNode.GetComponent<TestingTrajectories>();

                if (pathProgress < path.Count - 1 && path[pathProgress + 1].transform == trajectory.chainedDirectionZone)
                {
                    controller.beginAction(trajectory.convertToAction(), col.transform);
                    pathProgress++;
                    stayedOnPath = true;
                    break;
                }
            }

            if (!stayedOnPath)
                journey = "got lost";
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

            //if the bot found the path's starting node, head there
            if (path[0].transform == trajectory.chainedDirectionZone)
            {
                controller.beginAction(trajectory.convertToAction(), decisionZone);
                foundReroute = true;
                break;
            }
        }

        //otherwise teleport to the path's starting node
        if (!foundReroute)
            transform.position = new Vector3(path[0].transform.position.x,
            path[0].transform.position.y + 0.5f, transform.position.z);
    }


}
