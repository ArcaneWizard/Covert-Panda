using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSideCollider : MonoBehaviour
{
    public BotAI BotAI;

    void Awake()
    {
        if (gameObject.name != "Forward" && gameObject.name != "Backward")
            Debug.LogError("Don't change this gameObject's name as it is used to identify it in the sideview_controller script");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BotAI.nextToWall = gameObject.name;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        BotAI.nextToWall = gameObject.name;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        BotAI.nextToWall = "";
    }
}
