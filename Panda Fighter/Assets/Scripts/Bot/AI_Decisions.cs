
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AI_Decisions : MonoBehaviour
{
    private AI_Controller controller;
    private List<AI_ACTION> AI_ACTIONS = new List<AI_ACTION>();
    private TestingTrajectories trajectory;

    private Queue<Transform> decisionZones = new Queue<Transform>();

    public Text decisionZonesPending;
    public Text lastDecision;
    private System.Random random, random2;

    void Awake()
    {
        controller = transform.GetComponent<AI_Controller>();
        random = new System.Random();
        random2 = new System.Random();
    }

    void Update()
    {
        if (decisionZones.Count >= 1 && getSquaredDistanceBtwnVectors(decisionZones.Peek().position, transform.position) > 49)
        {
            Debug.Log("Discarded " + decisionZones.Peek() + " " + getSquaredDistanceBtwnVectors(decisionZones.Peek().position, transform.position));
            decisionZones.Dequeue();
            updateZonesText();
            return;
        }

        if (decisionZones.Count >= 1 && controller.actionProgress == "finished")
        {
            controller.decisionZone = decisionZones.Dequeue();
            updateZonesText();

            AI_ACTIONS.Clear();
            foreach (Transform decision in controller.decisionZone)
            {
                trajectory = decision.transform.GetComponent<TestingTrajectories>();
                if (decision.gameObject.activeSelf && trajectory.movementDirX == controller.dirX)
                    addTrajectoryAsPossibleAction();
                else if (decision.gameObject.activeSelf && trajectory.movementDirX == -controller.dirX && random2.Next(0, 100) >= 98)
                    addTrajectoryAsPossibleAction();
            }

            if (AI_ACTIONS.Count == 0 && controller.decisionZone.childCount > 0)
                addTrajectoryAsPossibleAction();

            int r = random.Next(0, AI_ACTIONS.Count);
            controller.AI_action = AI_ACTIONS[r];

            controller.actionProgress = "started";
            controller.shouldExecuteAction = true;
        }

        lastDecision.text = controller.AI_action.action + (controller.decisionZone ? ", " + controller.decisionZone.name : "none");
    }

    //if the AI bot collides with a decision making zone
    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col.transform.name);
        if (col.gameObject.layer == 8 && col.transform != controller.decisionZone)
        {
            if (decisionZones.Count > 0 && col.transform == decisionZones.Peek())
                return;

            Debug.Log("Queue " + col.transform.name);
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
                controller.AI_action = defineNewAction("keepWalking");
            else if (trajectory.fallDown)
                controller.AI_action = defineNewAction("fallDown");
            else if (trajectory.fallDownCurve)
                controller.AI_action = defineNewAction("fallDownCurve");
            else if (trajectory.doubleJump)
                controller.AI_action = defineNewAction("doubleJump");
            else
                controller.AI_action = defineNewAction("normalJump");

            AI_ACTIONS.Add(controller.AI_action);
        }

    }

    private AI_ACTION defineNewAction(string actionName)
    {
        return new AI_ACTION(actionName, trajectory.movementDirX, trajectory.speedRange, trajectory.timeB4Change, trajectory.changedSpeed, trajectory.bonusTrait, trajectory.transform.GetChild(0).position);
    }

}
