using System.Collections.Generic;

using UnityEngine;

public class AI_WanderAround : MonoBehaviour
{
    private AI_Controller controller;

    // percent chance of considering actions that reverse direction
    private float switchDirectionOdds = 30f;
    private System.Random random;

    private TrajectoryPath trajectoryPath;
    private Queue<Transform> decisionZones = new Queue<Transform>();
    private Transform currentDecisionZone;

    private bool shouldWander;
    private bool justEnteredWanderingState;

    void Awake()
    {
        controller = transform.GetComponent<AI_Controller>();
        random = new System.Random();
    }

    // start wandering around. Resets settings, updates that the AI should resume
    // wandering and that it just entered the wandering state 
    public void StartWandering()
    {
        decisionZones.Clear();

        shouldWander = true;
        justEnteredWanderingState = true;
    }

    // stop wandering around. Updates that the AI should stop wandering
    public void StopWandering()
    {
        shouldWander = false;
        controller.EndAction();
    }

    // Called every frame. Returns early if the AI isn't in the wander state or if there
    // are no queued decision zones to analyze
    public void Tick()
    {
        if (!shouldWander || decisionZones.Count == 0)
            return;

        // discards the next queued up decision zone if the bot has gotten too far from it distance
        // wise or elevation wise 
        if (getSquaredDistanceBtwnVectors(decisionZones.Peek().position, transform.position) > 900
        || Mathf.Abs(decisionZones.Peek().position.y - transform.position.y) > 9f) {
            decisionZones.Dequeue();
            return;
        }

        // retrive the next, pending decision zone if the bot isn't performing an action.
        // pick a random action to perform within this new decision zone (with a preference to moving in the 
        // same direction - left or right - as opposed to switching directions). 
        if (controller.CurrAction == null) {
            currentDecisionZone = decisionZones.Dequeue();
            List<AIAction> AI_ACTIONS = new List<AIAction>();

            if (currentDecisionZone.childCount == 0)
                Debug.LogError("Empty Decision Zone");

            // AI is picky and chooses actions it prefers
            if (Random.Range(0, 100) > switchDirectionOdds) {
                foreach (Transform decision in currentDecisionZone) {
                    trajectoryPath = decision.transform.GetComponent<TrajectoryPath>();
                    if (trajectoryPath == null)
                        continue;

                    if (trajectoryPath.ConvertToAction().DirX == controller.DirX) {
                        for (int i = 0; i < trajectoryPath.ConsiderationWeight; i++)
                            AI_ACTIONS.Add(trajectoryPath.ConvertToAction());
                    }
                }
            }

            // If the AI was too picky and didn't choose an action, just consider all actions
            if (AI_ACTIONS.Count == 0) {
                foreach (Transform decision in currentDecisionZone) {
                    trajectoryPath = decision.transform.GetComponent<TrajectoryPath>();
                    if (trajectoryPath == null)
                        continue;

                    for (int i = 0; i < trajectoryPath.ConsiderationWeight; i++)
                        AI_ACTIONS.Add(trajectoryPath.ConvertToAction());
                }
            }

            // Don't do anything if there are still no available actions (decision zone configured incorrectly)
            if (AI_ACTIONS.Count == 0)
                return;

            int actionIndex = random.Next(0, AI_ACTIONS.Count);
            controller.StartAction(AI_ACTIONS[actionIndex], currentDecisionZone);
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

        if (col.gameObject.layer == 8) {
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

        if (col.gameObject.layer == 8) {
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