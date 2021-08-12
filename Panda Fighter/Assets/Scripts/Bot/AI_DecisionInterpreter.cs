
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AI_DecisionInterpreter : MonoBehaviour
{
    private AI_Controller controller;
    private AI_StateManager stateManager;
    private List<AI_ACTION> AI_ACTIONS = new List<AI_ACTION>();
    private AI_ACTION action;
    private TestingTrajectories trajectory;

    private Queue<Transform> decisionZones = new Queue<Transform>();

    public Text decisionZonesPending;
    public Text lastDecision;

    private int headingDirX;
    private float distance;
    private System.Random random, random2;

    void Awake()
    {
        controller = transform.GetChild(0).GetComponent<AI_Controller>();
        stateManager = transform.GetComponent<AI_StateManager>();
        random = new System.Random();
        random2 = new System.Random();
    }

    void Update()
    {
        //discard the next pending decision zone if the bot has gotten too far from it
        if (decisionZones.Count >= 1 && getSquaredDistanceBtwnVectors(decisionZones.Peek().position, transform.position) > 49)
        {
            Debug.Log("Discarded " + decisionZones.Peek() + " " + getSquaredDistanceBtwnVectors(decisionZones.Peek().position, transform.position));
            decisionZones.Dequeue();
            updateZonesText();
            return;
        }

        //analyze the next pending decision if the bot isn't performing an action rn
        if (decisionZones.Count >= 1 && controller.actionProgress == "finished")
        {
            controller.registerNewDecisionZone(decisionZones.Dequeue());
            updateZonesText();

            prepDecisionZone();
            narrowDownPossibleActions();
            controller.executeNewAIAction(pickPossibleAction());
        }

        lastDecision.text = controller.AI_action.action + (controller.decisionZone ? ", " + controller.decisionZone.name : "none");
    }

    private void narrowDownPossibleActions()
    {
        //for each decision within the zone
        foreach (Transform decision in controller.decisionZone)
        {
            if (stateManager.AI_STATE == AI_STATE.Wandering)
            {
                headingDirX = (int)Mathf.Sign(stateManager.visitingPlaceLocation.x - transform.position.x);
                if (trajectoryIsFavorableWhileWandering(decision, headingDirX))
                    addTrajectoryAsPossibleAction();
            }
        }
    }

    private bool trajectoryIsFavorableWhileWandering(Transform decision, int headingDirX)
    {
        trajectory = decision.transform.GetComponent<TestingTrajectories>();
        distance = transform.position.x - stateManager.visitingPlaceLocation.x;

        //if the trajectory is far away x-coordinate wise
        if (Mathf.Abs(distance) > 8.5f && decision.gameObject.activeSelf)
        {
            //consider the trajectory if it heads in the same direction as wherever the bot is heading to 
            if (trajectory.movementDirX == headingDirX)
            {
                DebugGUI.debugText1 += "\n same direction as the visiting place";
                return true;
            }

            //consider an opposite direction trajectory 2% of the time
            else if (trajectory.movementDirX == -headingDirX && random2.Next(0, 100) >= 98)
            {
                DebugGUI.debugText1 += "\n opposite dir of the visiting place";
                return true;
            }
        }

        //if the trajectory is close x-coordinate wise
        else if (decision.gameObject.activeSelf)
        {
            //consider the trajectory if it's y position gets the bot closer to its target
            if ((stateManager.visitingPlaceLocation.y - trajectory.endPoint.y) < (stateManager.visitingPlaceLocation.y - transform.position.y))
            {
                DebugGUI.debugText1 += "\n close x action";
                return true;
            }
            else
                DebugGUI.debugText1 += "\n no close x action";
        }

        return false;
    }

    private void prepDecisionZone()
    {
        AI_ACTIONS.Clear();
        DebugGUI.debugText1 = "";

        //throw error if decision zone has 0 specified decisions
        if (controller.decisionZone.childCount == 0)
            Debug.LogError("Empty Decision Zone");
    }

    private AI_ACTION pickPossibleAction()
    {
        //if no trajectories were chosen for consideration, consider a random one
        if (AI_ACTIONS.Count == 0 && controller.decisionZone.childCount > 0)
        {
            trajectory = controller.decisionZone.GetChild(UnityEngine.Random.Range(0, controller.decisionZone.childCount)).transform.GetComponent<TestingTrajectories>();
            addTrajectoryAsPossibleAction();
            DebugGUI.debugText1 += "\n random action chosen";
        }

        int r = random.Next(0, AI_ACTIONS.Count);
        return AI_ACTIONS[r];
    }

    //if the AI bot collides with a decision making zone
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 8 && col.transform != controller.decisionZone)
        {
            if (decisionZones.Count > 0 && col.transform == decisionZones.Peek())
                return;

            decisionZones.Enqueue(col.transform);
            updateZonesText();
        }
    }

    private float getSquaredDistanceBtwnVectors(Vector2 a, Vector2 b)
    {
        Vector2 distanceBtwnVectors = a - b;
        return distanceBtwnVectors.x * distanceBtwnVectors.x + distanceBtwnVectors.y * distanceBtwnVectors.y;
    }

    private void updateZonesText()
    {
        decisionZonesPending.text = "";
        foreach (Transform decisionZone in decisionZones)
        {
            decisionZonesPending.text += decisionZone.name + "\n";
        }
    }

    private void addTrajectoryAsPossibleAction()
    {
        for (int i = 0; i < trajectory.considerationWeight; i++)
        {
            if (trajectory.headStraight)
                action = defineNewAction("keepWalking");
            else if (trajectory.fallDown)
                action = defineNewAction("fallDown");
            else if (trajectory.fallDownCurve)
                action = defineNewAction("fallDownCurve");
            else if (trajectory.doubleJump)
                action = defineNewAction("doubleJump");
            else
                action = defineNewAction("normalJump");

            AI_ACTIONS.Add(action);
        }

    }

    private AI_ACTION defineNewAction(string actionName)
    {
        return new AI_ACTION(actionName, trajectory.movementDirX, trajectory.speedRange, trajectory.timeB4Change, trajectory.changedSpeed, trajectory.bonusTrait, trajectory.transform.GetChild(0).position);
    }
}
