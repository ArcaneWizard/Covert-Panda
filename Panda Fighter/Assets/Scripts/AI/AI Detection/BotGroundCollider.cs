﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotGroundCollider : MonoBehaviour
{
    public NewBotAI AI;

    void Awake()
    {
        if (gameObject.name != "Left foot" && gameObject.name != "Right foot" && gameObject.name != "Center foot")
            Debug.LogError("Don't change this gameObject's name as it is used to identify it in the sideview_controller script");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        confirmGround(true, other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        confirmGround(true, other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        confirmGround(false, other);
    }

    private void confirmGround(bool confirmation, Collider2D other)
    {
        if (gameObject.name == "Left foot" && other.gameObject.layer == 11)
            AI.leftFootGround = (confirmation) ? other.gameObject : null;
        else if (gameObject.name == "Right foot" && other.gameObject.layer == 11)
            AI.rightFootGround = (confirmation) ? other.gameObject : null;
        else if (gameObject.name == "Center foot" && other.gameObject.layer == 11)
            AI.generalGround = (confirmation) ? other.gameObject : null;
    }
}