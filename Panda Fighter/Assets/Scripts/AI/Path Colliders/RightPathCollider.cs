using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightPathCollider : MonoBehaviour
{
    private JumpPath jumpPath;
    private List<GameObject> obstacles = new List<GameObject>();

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
                jumpPath.rightJump100[transform.GetSiblingIndex()] = setting;

            else if (transform.parent.name == "80% Jump")
                jumpPath.rightJump80[transform.GetSiblingIndex()] = setting;

            else if (transform.parent.name == "60% Jump")
                jumpPath.rightJump60[transform.GetSiblingIndex()] = setting;

            else if (transform.parent.name == "40% Jump")
                jumpPath.rightJump40[transform.GetSiblingIndex()] = setting;
            else
                Debug.LogError("jump path hasn't been defined yet");
        }

        else if (transform.parent.parent.name == "Right Path DJ Colliders")
        {
            if (transform.parent.name == "DJ, 0.5 sec, 8.0 speed, 6.4 speed")
                jumpPath.rightDoubleJump1[transform.GetSiblingIndex()] = setting;

            else if (transform.parent.name == "DJ, 0.5 sec, 4.8  speed, 6.4 speed")
                jumpPath.rightDoubleJump2[transform.GetSiblingIndex()] = setting;

            else if (transform.parent.name == "DJ, 0.3 sec, 2.4 speed, 3.6 speed")
                jumpPath.rightDoubleJump3[transform.GetSiblingIndex()] = setting;

            else if (transform.parent.name == "DJ, 0.6 sec, 0 speed, 3.6 speed")
                jumpPath.rightDoubleJump4[transform.GetSiblingIndex()] = setting;
            else
                Debug.LogError(gameObject.name + " jump path hasn't been defined yet");
        }
        else
            Debug.LogError("jump path hasn't been defined yet");
    }
}
