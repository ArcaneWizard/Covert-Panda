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
    private AI_ACTION action;
    private TestingTrajectories trajectory;

    private Queue<Transform> decisionZones = new Queue<Transform>();
    private Transform currentDecisionZone;
    private Collider2D decisionZoneAiIsTouching;

    private int headingDirX;
    private float distance;
    private System.Random random;

    private bool wander, stuckOnTheWall;
    private Vector3 lastPos;

    void Awake()
    {
        controller = transform.GetComponent<AI_Controller>();
        random = new System.Random();

        StartCoroutine(stuckOnWall());
    }

    // initialize variables + start wandering (called when AI enters the Wander State)
    public void startWandering()
    {
        wander = true;
        decisionZones.Clear();

        // manually add any decision zone that the AI has already entered to the queue
        if (decisionZoneAiIsTouching != null)
            OnTriggerEnter2D(decisionZoneAiIsTouching);
    }

    // reset variables + stop wandering (called when AI exits the Wander State)
    public void stopWandering()
    {
        wander = false;
        decisionZones.Clear();
        controller.CurrentActionIsFinished();
    }

    // wandering logic (called every frame when the AI's in the Wander State)
    public void tick()
    {
        DebugGUI.debugText6 = "wander: " + wander;
        if (!wander || decisionZones.Count == 0)
            return;

        DebugGUI.debugText2 = controller.AI_action.action + (controller.decisionZone ? ", " +
                controller.decisionZone.name : "none");

        String a = "";
        foreach (Transform zone in decisionZones)
            a += zone.name + " " + zone.position + ", " + transform.position + " \n";
        DebugGUI.debugText1 = a;
        DebugGUI.debugText6 = "2";

        // discard the next pending decision zone if the bot has gotten too far from it,
        // distance wise or elevation wise
        if (getSquaredDistanceBtwnVectors(decisionZones.Peek().position, transform.position) > 900f
            || Mathf.Abs(decisionZones.Peek().position.y - transform.position.y) > 12f)
        {
            DebugGUI.debugText3 = ("Discarded " + decisionZones.Peek() + " " +
             getSquaredDistanceBtwnVectors(decisionZones.Peek().position, transform.position));
            decisionZones.Dequeue();
            return;
        }

        DebugGUI.debugText6 = "3 and " + controller.actionProgress;

        // perform a new action if the bot is not currently executing an action.
        if (controller.actionProgress == "finished")
            beginNewAction();

        // if the bot is stuck on a wall while grounded, flip what direction it was heading, 
        // set it to head right if it didn't have a direction, and manually add any decision
        // zone that the AI has already entered to the queue
        if (controller.isGrounded && stuckOnTheWall)
        {
            stuckOnTheWall = false;
            controller.setDirection(-controller.dirX);

            if (controller.dirX == 0)
                controller.setDirection(1);

            if (decisionZoneAiIsTouching != null)
                OnTriggerEnter2D(decisionZoneAiIsTouching);
        }
    }

    // pick the next pending decision zone in the queue, and choose a random
    // trajectory branching out from it
    private void beginNewAction()
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
        DebugGUI.debugText7 = currentDecisionZone.name + ": " + (AI_ACTIONS[r].ToString());
        controller.BeginAction(AI_ACTIONS[r], currentDecisionZone);
    }

    // when the AI bot touches a decision zone, add it to the list. However, it must
    // contain at least 1 viable trajectory to follow and not be a repeat of the last
    // zone added to the queue
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (decisionZones.Count > 0 && col.transform == decisionZones.Peek())
            return;

        if (col.gameObject.layer == 8)
            decisionZones.Enqueue(col.transform);
    }

    // update what decision zone the AI is currently touching 
    private void OnTriggerStay2D(Collider2D col)
    {
        decisionZoneAiIsTouching = col.gameObject.layer == 8 ? col : null;
    }

    // helper method that returns squared distance btwn 2 vectors
    private float getSquaredDistanceBtwnVectors(Vector2 a, Vector2 b)
    {
        Vector2 distanceBtwnVectors = a - b;
        return distanceBtwnVectors.x * distanceBtwnVectors.x + distanceBtwnVectors.y * distanceBtwnVectors.y;
    }

    // update a bool to indicate whether the AI is stuck in a wall. Checks if the AI is
    // in the wander state and either not moving or moving in the direction of a wall 
    // closeby. If so and the bot hasn't moved much in the last 0.2 seconds, it's stuck.
    private IEnumerator stuckOnWall()
    {
        if (wander)
        {
            if (controller.dirX == 0 || (controller.dirX == 1 && controller.wallToTheRight) ||
                    (controller.dirX == -1 && controller.wallToTheLeft))
                stuckOnTheWall = getSquaredDistanceBtwnVectors(lastPos, transform.position) < 0.23f;
        }

        lastPos = transform.position;
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(stuckOnWall());
    }
}