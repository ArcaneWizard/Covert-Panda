using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightPathCollider : MonoBehaviour
{
    private JumpPath jumpPath;
    [HideInInspector]
    public List<GameObject> obstacles = new List<GameObject>();

    void Awake()
    {
        jumpPath = transform.parent.parent.parent.transform.GetComponent<JumpPath>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 11)
        {
            if (!obstacles.Contains(other.gameObject))
                obstacles.Add(other.gameObject);

            setObstacle(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 11)
        {
            if (obstacles.Contains(other.gameObject))
                obstacles.Remove(other.gameObject);

            if (obstacles.Count == 0)
                setObstacle(false);
        }
    }

    private void setObstacle(bool setting)
    {
        if (transform.parent.parent.name == "Right Path Colliders")
        {
            if (transform.parent.name == "100% Jump")
                jumpPath.rightJumpList[0][transform.GetSiblingIndex()] = setting;

            else if (transform.parent.name == "80% Jump")
                jumpPath.rightJumpList[1][transform.GetSiblingIndex()] = setting;

            else if (transform.parent.name == "70% Jump")
                jumpPath.rightJumpList[2][transform.GetSiblingIndex()] = setting;

            else if (transform.parent.name == "60% Jump")
                jumpPath.rightJumpList[3][transform.GetSiblingIndex()] = setting;
            else
                Debug.LogError("jump path hasn't been defined yet");
        }

        else if (transform.parent.parent.name == "Right Path DJ Colliders")
        {
            if (transform.parent.name == "DJ, 0.5 sec, 8.0 speed, 6.4 speed")
                jumpPath.rightJumpList[4][transform.GetSiblingIndex()] = setting;

            else if (transform.parent.name == "DJ, 0.5 sec, 4.8  speed, 6.4 speed")
                jumpPath.rightJumpList[5][transform.GetSiblingIndex()] = setting;

            else if (transform.parent.name == "DJ, 0.3 sec, 7.0 speed, 5.5 speed")
                jumpPath.rightJumpList[6][transform.GetSiblingIndex()] = setting;

            else if (transform.parent.name == "DJ, 0.6 sec, 2.0 speed, 3.6 speed")
                jumpPath.rightJumpList[7][transform.GetSiblingIndex()] = setting;

            else if (transform.parent.name == "DJ, 0.6 sec, 6.4 speed, 6.4 speed")
                jumpPath.rightJumpList[8][transform.GetSiblingIndex()] = setting;
            else
                Debug.LogError(gameObject.name + " jump path hasn't been defined yet");
        }

        else if (transform.parent.parent.name == "Right U Turn Colliders")
        {
            if (transform.parent.name == "U turn, 0.6 sec, 8.0 speed, 6.4 speed")
                jumpPath.rightJumpList[9][transform.GetSiblingIndex()] = setting;

            else if (transform.parent.name == "U turn, 0.7 sec, 8.0 speed, 8.0 speed")
                jumpPath.rightJumpList[10][transform.GetSiblingIndex()] = setting;

            else if (transform.parent.name == "U mini turn, 8.0 speed, 0.35 sec")
                jumpPath.rightJumpList[11][transform.GetSiblingIndex()] = setting;

            else if (transform.parent.name == "U mini turn, 7.0 speed, 0.42 sec")
                jumpPath.rightJumpList[12][transform.GetSiblingIndex()] = setting;
            else
                Debug.LogError(gameObject.name + " jump path hasn't been defined yet");
        }

        else
            Debug.LogError("jump path hasn't been defined yet");
    }
}
