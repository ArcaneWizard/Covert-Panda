using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Assertions.Must;

public class AI_WanderAround : MonoBehaviour
{
    private AI_Controller controller;
    private List<AI_ACTION> AI_ACTIONS = new List<AI_ACTION>();
    private AI_ACTION action;
    private TestingTrajectories trajectory;

    private Queue<Transform> decisionZones = new Queue<Transform>();
    private Transform currentDecisionZone;

    private int headingDirX;
    private float distance;
    private System.Random random;

    private bool wander;

    void Awake()
    {
        controller = transform.GetComponent<AI_Controller>();
        random = new System.Random();
    }

    //start wandering around
    public void startWandering()
    {
        decisionZones.Clear();
        AI_ACTIONS.Clear();
        wander = true;
    }

    //stop wandering around
    public void stopWandering() => wander = false;

    //to be called every frame whlie wandering
    public void tick()
    {
        if (!wander || decisionZones.Count == 0)
            return;

        //discard the next pending decision zone if the bot has gotten too far from it
        if (getSquaredDistanceBtwnVectors(decisionZones.Peek().position, transform.position) > 49)
        {
            DebugGUI.debugText5 = ("Discarded " + decisionZones.Peek() + " " +
             getSquaredDistanceBtwnVectors(decisionZones.Peek().position, transform.position));
            decisionZones.Dequeue();
            return;
        }

        //analyze the next pending decision if the bot isn't performing an action rn
        if (controller.actionProgress == "finished")
        {
            currentDecisionZone = decisionZones.Dequeue();
            AI_ACTIONS.Clear();

            if (currentDecisionZone.childCount == 0)
                Debug.LogError("Empty Decision Zone");

            foreach (Transform decision in currentDecisionZone)
            {
                trajectory = decision.transform.GetComponent<TestingTrajectories>();
                for (int i = 0; i < trajectory.considerationWeight; i++)
                {
                    AI_ACTIONS.Add(trajectory.convertToAction());
                }
            }

            int r = random.Next(0, AI_ACTIONS.Count);
            controller.beginAction(AI_ACTIONS[r], currentDecisionZone);
        }

        //In scene view, display the action done and decision zone used
        DebugGUI.debugText6 = controller.AI_action.action + (controller.decisionZone ? ", " +
                controller.decisionZone.name : "none");

    }

    //if the AI bot passes a new decision zone, add it to the list
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!wander)
            return;

        if (decisionZones.Count > 0 && col.transform == decisionZones.Peek())
            return;

        if (col.gameObject.layer == 8 && col.transform != controller.decisionZone)
            decisionZones.Enqueue(col.transform);
    }

    //Helper method that returns squared distance btwn 2 vectors
    private float getSquaredDistanceBtwnVectors(Vector2 a, Vector2 b)
    {
        Vector2 distanceBtwnVectors = a - b;
        return distanceBtwnVectors.x * distanceBtwnVectors.x + distanceBtwnVectors.y * distanceBtwnVectors.y;
    }
}