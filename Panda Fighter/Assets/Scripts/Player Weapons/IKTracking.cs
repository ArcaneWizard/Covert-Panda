
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AimingDir
{
    //ideal coordinates when looking to the side, up or down ----> (DEFAULT FOR MOST WEAPONS)
    static Vector2 defaultPointingRight = new Vector2(1.307f, 2.101f);
    static Vector2 defaultPointingUp = new Vector2(0.27f, 3.29f);
    static Vector2 defaultPointingDown = new Vector2(0.11f, 0.81f);
    static Vector2 defaultShoulderPos = new Vector2(0.173f, 2.198f);

    public static List<Vector2> defaultAiming = new List<Vector2>()
    { defaultPointingRight, defaultPointingUp, defaultPointingDown, defaultShoulderPos };

    //ideal scythe coordinates when looking to the side, up or down 
    static Vector2 scythePointingRight = new Vector2(1.307f, 2.101f);
    static Vector2 scythePointingUp = new Vector2(0.27f, 3.29f);
    static Vector2 scythePointingDown = new Vector2(0.11f, 0.81f);
    static Vector2 scytheShoulderPos = new Vector2(0.228f, 2.125f);
    static Vector2 scytheAttackPos = new Vector2(0.672f, 0.977f);

    public static List<Vector2> scytheAiming = new List<Vector2>()
    { scythePointingRight, scythePointingUp, scythePointingDown, scytheShoulderPos, scytheAttackPos };

    //ideal shotgun coordinates when looking to the side, up or down 
    static Vector2 shotgunPointingRight = new Vector2(1.65f, 1.54f);
    static Vector2 shotgunPointingUp = new Vector2(-0.0715f, 2.728f);
    static Vector2 shotgunPointingDown = new Vector2(0.65f, -0.14f);
    static Vector2 shotgunShoulderPos = new Vector2(0.13f, 1.18f);

    public static List<Vector2> shotgunAiming = new List<Vector2>()
    { shotgunPointingRight, shotgunPointingUp, shotgunPointingDown, shotgunShoulderPos };
}

public class IKTracking : MonoBehaviour
{
    public List<Vector2> setIKCoordinates(string weapon)
    {
        if (weapon == "Shotgun")
            return AimingDir.shotgunAiming;
        else
            return AimingDir.defaultAiming;
    }
}