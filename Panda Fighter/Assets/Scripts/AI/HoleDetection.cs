using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleDetection : MonoBehaviour
{
    private NewBotAI AI;
    private List<GameObject> grounds = new List<GameObject>();

    void Awake()
    {
        AI = transform.parent.parent.parent.transform.GetComponent<NewBotAI>();

        if (transform.parent.name != "Right Detection" && transform.parent.name != "Left Detection")
            Debug.LogError("Don't change ground detector's name as it's used to identify it in this script");
    }

    void Update()
    {
        transform.localEulerAngles = -transform.parent.parent.eulerAngles;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 11)
        {
            if (!grounds.Contains(other.gameObject))
                grounds.Add(other.gameObject);

            if (transform.parent.name == "Right Detection")
                AI.rightGround[transform.GetSiblingIndex()] = true;

            else if (transform.parent.name == "Left Detection")
                AI.leftGround[transform.GetSiblingIndex()] = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 11)
        {
            if (grounds.Contains(other.gameObject))
                grounds.Remove(other.gameObject);

            if (transform.parent.name == "Right Detection" && grounds.Count == 0)
                AI.rightGround[transform.GetSiblingIndex()] = false;

            else if (transform.parent.name == "Left Detection" && grounds.Count == 0)
                AI.leftGround[transform.GetSiblingIndex()] = false;
        }
    }
}
