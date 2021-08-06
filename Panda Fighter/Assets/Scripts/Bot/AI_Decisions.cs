
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI_Decisions : MonoBehaviour
{
    private AI_Controller controller;
    private List<AI_ACTION> AI_ACTIONS = new List<AI_ACTION>();
    private TestingTrajectories trajectory;

    private Queue<Transform> decisionZones = new Queue<Transform>();
    public Text decisionZonesPending;
    public Text lastDecision;

    void Awake()
    {
        controller = transform.GetComponent<AI_Controller>();
    }

    void Update()
    {
        if (decisionZones.Count >= 1 && Mathf.Abs(decisionZones.Peek().position.x - transform.position.x) > 8.5f)
        {
            Debug.Log("Discarded " + decisionZones.Peek());
            decisionZones.Dequeue();
            return;
        }

        if (decisionZones.Count >= 1 && controller.actionProgress == "finished")
        {
            controller.decisionZone = decisionZones.Dequeue();

            AI_ACTIONS.Clear();
            foreach (Transform decision in controller.decisionZone)
            {
                if (decision.gameObject.activeSelf)
                    addTrajectoryAsPossibleAction(decision);
            }

            int r = UnityEngine.Random.Range(0, AI_ACTIONS.Count);
            controller.AI_action = AI_ACTIONS[r];

            controller.actionProgress = "started";
            controller.shouldExecuteAction = true;
        }

        decisionZonesPending.text = decisionZones.Count + " zones queued up";

        lastDecision.text = controller.AI_action.action + (controller.decisionZone ? ", " + controller.decisionZone.name : "none");
    }

    //if the AI bot collides with a decision making zone
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 8)
            decisionZones.Enqueue(col.transform);
    }

    private void addTrajectoryAsPossibleAction(Transform decision)
    {
        trajectory = decision.transform.GetComponent<TestingTrajectories>();

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
