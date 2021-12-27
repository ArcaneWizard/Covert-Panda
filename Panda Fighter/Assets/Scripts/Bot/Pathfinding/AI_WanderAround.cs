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
    private float switchDirectionOdds = 30f;

    private AI_ACTION action;
    private TestingTrajectories trajectory;

    private Queue<Transform> decisionZones = new Queue<Transform>();
    private Transform currentDecisionZone;

    private int headingDirX;
    private float distance;
    private System.Random random;

    private bool shouldWander;
    private bool justEnteredWanderingState;

    void Awake()
    {
        controller = transform.GetComponent<AI_Controller>();
        random = new System.Random();
    }

    // start wandering around. Resets settings, updates that the AI should resume
    // wandering and that it just entered the wandering state 
    public void startWandering()
    {
        decisionZones.Clear();
        AI_ACTIONS.Clear();

        shouldWander = true;
        justEnteredWanderingState = true;
    }

    // stop wandering around. Updates that the AI should stop wandering
    public void stopWandering() => shouldWander = false;

    // called every frame. Returns early if the AI isn't in the wander state or if there
    // are no queued decision zones to analyze
    public void tick()
    {
        if (!shouldWander || decisionZones.Count == 0)
            return;

        // JUST FOR DEBUGGING, IGNORE
        DebugGUI.debugText3 = controller.AI_action.action + (controller.decisionZone ? ", " +
            controller.decisionZone.name : "none");
        String a = "zones: \n";
        foreach (Transform zone in decisionZones)
            a += zone.name + " \n";
        DebugGUI.debugText1 = a;

        // discards the next queued up decision zone if the bot has gotten too far from it distance
        // wise or elevation wise 
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
        // same direction - left or right - as opposed to switching directions). Send a call to the
        // AI controller to handle the logic of executing the chosen action 
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

    // if the AI bot enters a decision zone, add it to the queue of decisions
    // to execute. Howwever, only check for decision zones when the AI is in the wander state,
    // and never queue the same decision zone to be aexecuted back to back
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!shouldWander)
            return;

        if (decisionZones.Count > 0 && col.transform == decisionZones.Peek())
            return;

        if (col.gameObject.layer == 8)
        {
            decisionZones.Enqueue(col.transform);
            justEnteredWanderingState = false;
        }
    }

    // if the AI bot is in the wander state and just entered the wander state (from another state),
    // check if the bot is already INSIDE a decision zone. If so, add it to the queue of decisions
    // to execute 
    private void OnTriggerStay2D(Collider2D col)
    {
        if (!shouldWander || !justEnteredWanderingState)
            return;

        if (col.gameObject.layer == 8)
        {
            decisionZones.Enqueue(col.transform);
            justEnteredWanderingState = false;
        }
    }

    // helper method that returns squared distance btwn 2 vectors
    private float getSquaredDistanceBtwnVectors(Vector2 a, Vector2 b)
    {
        Vector2 distanceBtwnVectors = a - b;
        return distanceBtwnVectors.x * distanceBtwnVectors.x + distanceBtwnVectors.y * distanceBtwnVectors.y;
    }
}