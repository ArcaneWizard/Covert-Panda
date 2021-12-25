using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Assertions.Must;
using System.Security.Policy;

public class AI_WanderAround : MonoBehaviour
{
    public AI_Controller controller { get; private set; }
    private List<AI_ACTION> AI_ACTIONS = new List<AI_ACTION>();

    // percent chance of considering actions that reverse direction
    private float switchDirectionOdds = 20f;

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

        //In scene view, display the action done and decision zone used
        DebugGUI.debugText3 = controller.AI_action.action + (controller.decisionZone ? ", " +
                controller.decisionZone.name : "none");

        String a = "hello + \n";

        foreach (Transform zone in decisionZones)
            a += zone.name + " \n";
        DebugGUI.debugText1 = a;


        //discard the next pending decision zone if the bot has gotten too far from it
        if (getSquaredDistanceBtwnVectors(decisionZones.Peek().position, transform.position) > 900
        || Mathf.Abs(decisionZones.Peek().position.y - transform.position.y) > 9f)
        {
            DebugGUI.debugText5 = ("Discarded " + decisionZones.Peek() + " " +
             getSquaredDistanceBtwnVectors(decisionZones.Peek().position, transform.position));
            decisionZones.Dequeue();
            return;
        }

        // retrive the next pending decision zone once the bot finishes performing it's last action.
        // pick a random action to perform within the decision zone (with a preference to moving in the 
        // same direction - left or right - as opposed to switching directions)
        if (controller.actionProgress == "finished")
        {
            currentDecisionZone = decisionZones.Dequeue();
            AI_ACTIONS.Clear();

            if (currentDecisionZone.childCount == 0)
                Debug.LogError("Empty Decision Zone");

            if (UnityEngine.Random.Range(0, 100) > switchDirectionOdds)
            {
                foreach (Transform decision in currentDecisionZone)
                {
                    trajectory = decision.transform.GetComponent<TestingTrajectories>();

                    if (trajectory.convertToAction().dirX == controller.dirX)
                    {
                        for (int i = 0; i < trajectory.considerationWeight; i++)
                            AI_ACTIONS.Add(trajectory.convertToAction());
                    }
                }
            }

            if (AI_ACTIONS.Count == 0)
            {
                foreach (Transform decision in currentDecisionZone)
                {
                    trajectory = decision.transform.GetComponent<TestingTrajectories>();
                    for (int i = 0; i < trajectory.considerationWeight; i++)
                        AI_ACTIONS.Add(trajectory.convertToAction());
                }
            }

            int actionIndex = random.Next(0, AI_ACTIONS.Count);
            DebugGUI.debugText4 = (AI_ACTIONS[actionIndex].ToString());
            controller.BeginAction(AI_ACTIONS[actionIndex], currentDecisionZone);
        }
    }

    //if the AI bot passes a new decision zone, add it to the list
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!wander)
            return;

        if (decisionZones.Count > 0 && col.transform == decisionZones.Peek())
            return;

        if (col.gameObject.layer == 8)
            decisionZones.Enqueue(col.transform);
    }

    //Helper method that returns squared distance btwn 2 vectors
    private float getSquaredDistanceBtwnVectors(Vector2 a, Vector2 b)
    {
        Vector2 distanceBtwnVectors = a - b;
        return distanceBtwnVectors.x * distanceBtwnVectors.x + distanceBtwnVectors.y * distanceBtwnVectors.y;
    }
}