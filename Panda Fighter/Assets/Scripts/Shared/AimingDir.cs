
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AimingDir
{
    //ideal coordinates when looking to the side, up or down ----> (DEFAULT FOR MOST WEAPONS)
    static Vector2 defaultPointingRight = new Vector2(1.307f, 2.101f);
    static Vector2 defaultPointingUp = new Vector2(0.27f, 3.29f);
    static Vector2 defaultPointingDown = new Vector2(0.11f, 0.81f);
    static Vector2 defaultShoulderPos = new Vector2(0.173f, 2.198f);

    public static List<Vector2> DefaultAiming = new List<Vector2>()
    { defaultPointingRight, defaultPointingUp, defaultPointingDown, defaultShoulderPos };

    //ideal scythe coordinates when looking to the side, up or down 
    static Vector2 meeleePointingRight = new Vector2(1.307f, 2.101f);
    static Vector2 meeleePointingUp = new Vector2(0.27f, 3.29f);
    static Vector2 meeleePointingDown = new Vector2(0.11f, 0.81f);
    static Vector2 meeleeShoulderPos = new Vector2(0.228f, 2.125f);
    static Vector2 meeleeAttackPos = new Vector2(0.672f, 0.977f);

    public static List<Vector2> MeeleeGripAiming = new List<Vector2>()
    { meeleePointingRight, meeleePointingUp, meeleePointingDown, meeleeShoulderPos, meeleeAttackPos };

    //ideal shortBarrel coordinates when looking to the side, up or down 
    static Vector2 shortBarrelPointingRight = new Vector2(1.65f, 1.54f);
    static Vector2 shortBarrelPointingUp = new Vector2(-0.0715f, 2.728f);
    static Vector2 shortBarrelPointingDown = new Vector2(0.65f, -0.14f);
    static Vector2 shortBarrelShoulderPos = new Vector2(0.13f, 1.18f);

    public static List<Vector2> ShortBarrelAiming = new List<Vector2>()
    { shortBarrelPointingRight, shortBarrelPointingUp, shortBarrelPointingDown, shortBarrelShoulderPos };

    //ideal  pistol coordinates when looking to the side, up or down 
    static Vector2 pistolPointingRight = new Vector2(1.98f, 2.49f);
    static Vector2 pistolPointingUp = new Vector2(-0.1f, 3.94f);
    static Vector2 pistolPointingDown = new Vector2(0.31f, 0.48f);
    static Vector2 pistolShoulderPos = new Vector2(0.09f, 2.18f);

    public static List<Vector2> PistolGripAiming = new List<Vector2>()
    { pistolPointingRight, pistolPointingUp, pistolPointingDown, pistolShoulderPos };

    //ideal shoulder rest coordinates when looking to the side, up or down 
    static Vector2 shoulderPointingRight = new Vector2(1.893f, 3.017f);
    static Vector2 shoulderPointingUp = new Vector2(-0.1804f, 4.209f);
    static Vector2 shoulderPointingDown = new Vector2(1.15f, 0.578f);
    static Vector2 shoulderShoulderPos = new Vector2(0.09f, 2.18f);

    public static List<Vector2> ShoulderRestAiming = new List<Vector2>()
    { shoulderPointingRight, shoulderPointingUp, shoulderPointingDown, shoulderShoulderPos };
}
