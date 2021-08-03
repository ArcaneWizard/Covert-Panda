using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AimingDir
{
    //ideal coordinates when looking to the side, up or down ----> (DEFAULT FOR MOST WEAPONS)
    public static Vector2 defaultPointingRight = new Vector2(1.307f, 2.101f);
    public static Vector2 defaultPointingUp = new Vector2(0.27f, 3.29f);
    public static Vector2 defaultPointingDown = new Vector2(0.11f, 0.81f);
    public static Vector2 defaultShoulderPos = new Vector2(0.173f, 2.198f);

    //ideal scythe coordinates when looking to the side, up or down 
    public static Vector2 scythePointingRight = new Vector2(1.307f, 2.101f);
    public static Vector2 scythePointingUp = new Vector2(0.27f, 3.29f);
    public static Vector2 scythePointingDown = new Vector2(0.11f, 0.81f);
    public static Vector2 scytheShoulderPos = new Vector2(0.228f, 2.125f);
    public static Vector2 scytheAttackPos = new Vector2(0.672f, 0.977f);

    //ideal shotgun coordinates when looking to the side, up or down 
    public static Vector2 shotgunPointingRight = new Vector2(1.65f, 1.54f);
    public static Vector2 shotgunPointingUp = new Vector2(-0.0715f, 2.728f);
    public static Vector2 shotgunPointingDown = new Vector2(0.65f, -0.14f);
    public static Vector2 shotgunShoulderPos = new Vector2(0.13f, 1.18f);
}

public class IKTracking : MonoBehaviour
{
    private Sideview_Controller controller;

    void Awake()
    {
        controller = transform.GetComponent<Sideview_Controller>();
    }

    public void setIKCoordinates(string weapon)
    {
        if (weapon == "Shotgun")
        {
            controller.pointingRight = AimingDir.shotgunPointingRight;
            controller.pointingUp = AimingDir.shotgunPointingUp;
            controller.pointingDown = AimingDir.shotgunPointingDown;
            controller.shoulderPos = AimingDir.shotgunShoulderPos;
        }

        else
        {
            controller.pointingRight = AimingDir.defaultPointingRight;
            controller.pointingUp = AimingDir.defaultPointingUp;
            controller.pointingDown = AimingDir.defaultPointingDown;
            controller.shoulderPos = AimingDir.defaultShoulderPos;
        }

        controller.calculateShoulderAngles();
    }
}