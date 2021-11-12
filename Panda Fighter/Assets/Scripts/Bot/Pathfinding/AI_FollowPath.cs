
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.CompilerServices;

public class AI_FollowPath : MonoBehaviour
{
    private AI_Controller controller;
    private AI_StateManager stateManager;
    private System.Random random, random2;

    private string journey = "pending start";

    private List<Node> path;
    private int pathProgress;

    void Awake()
    {
        controller = transform.GetComponent<AI_Controller>();
        stateManager = transform.parent.GetComponent<AI_StateManager>();
        random = new System.Random();
        random2 = new System.Random();
    }

    //follow the provided path (to some intended target)
    public void follow(List<Node> path) 
    {
        //reset variables
        journey = "started";
        this.path = path;
        pathProgress = 0;

        //head towards starting path Node
        if (transform.GetChild(0).position.x < path[0].transform.position.x)
            controller.setDirection(1);
        else
            controller.setDirection(-1);
    }

    //bot no longer must follow set path
    public void endJourney() => journey = "ended";

    void Update()
    {
        //AI bot heads in the direction of it's next node when it's grounded
        if (stateManager.AI_STATE == AI_STATE.Wandering && journey == "in progress" &&
        controller.isGrounded && controller.isTouchingMap) {
            controller.setDirection(Math.Sign(path[pathProgress].transform.position.x - transform.position.x));
        }
    }

    //AI passes a node, gets potential routes from that node to neighbor nodes, and then
    //figures out which route allows it to stay on its intended path
    private void OnTriggerEnter2D(Collider2D col) 
    {
        if (col.gameObject.layer == 8) {
            
            if (journey == "started") {
                if (col.transform == path[0].transform)
                    journey = "in progress";
                else 
                    getBackOnIntendedPath(col.transform);
            }

            if (journey == "in progress") {

                foreach (Transform neighbourNode in col.transform) {
                    TestingTrajectories trajectory = neighbourNode.GetComponent<TestingTrajectories>();

                    if (pathProgress < path.Count-1 && path[pathProgress+1].transform == trajectory.chainedDirectionZone) {
                        controller.beginAction(convertTrajectoryToAction(trajectory), col.transform);
                        pathProgress++;
                        break;
                    }
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
            TestingTrajectories trajectory = neighborZone.transform.GetComponent<TestingTrajectories>();

            //if the bot found the path's starting node, head there
            if (path[0].transform == trajectory.chainedDirectionZone){
                controller.beginAction(convertTrajectoryToAction(trajectory), decisionZone);
                foundReroute = true;
                break;
            }
        }

        //otherwise teleport to the path's starting node
        if (!foundReroute)
            transform.position = new Vector3(path[0].transform.position.x, 
            path[0].transform.position.y + 0.5f, transform.position.z);
    }

    //Converts trajectory info to a condensed, easy to read form (of type AI_ACTION)
    private AI_ACTION convertTrajectoryToAction(TestingTrajectories trajectory)
    {
        AI_ACTION action;
            
            if (trajectory.headStraight)
                action = defineNewAction("keepWalking", trajectory);
            else if (trajectory.fallDown)
                action = defineNewAction("fallDown", trajectory);
            else if (trajectory.fallDownCurve)
                action = defineNewAction("fallDownCurve", trajectory);
            else if (trajectory.doubleJump)
                action = defineNewAction("doubleJump", trajectory);
            else
                action = defineNewAction("normalJump", trajectory);
        
        return action;
    }
    
    //helper method for converting trajectory info to a condensed readable form
    private AI_ACTION defineNewAction(string actionName, TestingTrajectories trajectory) => 
    new AI_ACTION(actionName,trajectory.movementDirX, trajectory.speedRange, 
    trajectory.timeB4Change, trajectory.changedSpeed, trajectory.bonusTrait, 
    trajectory.transform.GetChild(0).position);
    
}
