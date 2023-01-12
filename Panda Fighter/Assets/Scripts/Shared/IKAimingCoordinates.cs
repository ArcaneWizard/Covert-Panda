
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores the Inverse Kinematic (IK) coordinates that properly setup the rotation of a creature's arms
// when aiming a weapon. Coordinates are grouped by which weapon type the arm limbs are meant to hold

public static class IKAimingCoordinates
{
    //ideal coordinates when looking to the side, up or down ----> (DEFAULT FOR MOST WEAPONS)
    private static Vector2 defaultPointingRight = new Vector2(1.307f, 2.101f);
    private static Vector2 defaultPointingUp = new Vector2(0.27f, 3.29f);
    private static Vector2 defaultPointingDown = new Vector2(0.11f, 0.81f);
    private static Vector2 defaultShoulderPos = new Vector2(0.173f, 2.198f);

    public static List<Vector2> DefaultCoordinates = new List<Vector2>()
    { defaultPointingRight, defaultPointingUp, defaultPointingDown, defaultShoulderPos };

    //ideal scythe coordinates when looking to the side, up or down 
    private static Vector2 meeleePointingRight = new Vector2(1.307f, 2.101f);
    private static Vector2 meeleePointingUp = new Vector2(0.27f, 3.29f);
    private static Vector2 meeleePointingDown = new Vector2(0.11f, 0.81f);
    private static Vector2 meeleeShoulderPos = new Vector2(0.228f, 2.125f);
    private static Vector2 meeleeAttackPos = new Vector2(0.672f, 0.977f);

    public static List<Vector2> MeeleeGripCoordinates = new List<Vector2>()
    { meeleePointingRight, meeleePointingUp, meeleePointingDown, meeleeShoulderPos, meeleeAttackPos };

    //ideal shortBarrel coordinates when looking to the side, up or down with right arm
    private static Vector2 shortBarrelPointingRight = new Vector2(1.17f, -0.22f);
    private static Vector2 shortBarrelPointingUp = new Vector2(-0.13f, 1.55f);
    private static Vector2 shortBarrelPointingDown = new Vector2(-0.39f, -1.62f);
    private static Vector2 shortBarrelShoulderPos = new Vector2(-0.35f, -0.15f);

    public static List<Vector2> ShortBarrelMainArm = new List<Vector2>()
    { shortBarrelPointingRight, shortBarrelPointingUp, shortBarrelPointingDown, shortBarrelShoulderPos };


    //ideal shortBarrel coordinates when looking to the side, up or down wih left arm
    private static Vector2 shortBarrelPointingRight2 = new Vector2(1.96f, 0.82f);
    private static Vector2 shortBarrelPointingUp2 = new Vector2(0.53f, 2.35f);
    private static Vector2 shortBarrelPointingDown2 = new Vector2(0.69f, -0.81f);
    private static Vector2 shortBarrelShoulderPos2 = new Vector2(0.37f, 0.71f);

    public static List<Vector2> ShortBarrelOtherArm = new List<Vector2>()
    { shortBarrelPointingRight2, shortBarrelPointingUp2, shortBarrelPointingDown2, shortBarrelShoulderPos2 };

    //ideal  pistol coordinates when looking to the side, up or down 
    private static Vector2 pistolPointingRight = new Vector2(1.98f, 2.49f);
    private static Vector2 pistolPointingUp = new Vector2(-0.1f, 3.94f);
    private static Vector2 pistolPointingDown = new Vector2(0.31f, 0.48f);
    private static Vector2 pistolShoulderPos = new Vector2(0.09f, 2.18f);

    public static List<Vector2> PistolGripCoordinates = new List<Vector2>()
    { pistolPointingRight, pistolPointingUp, pistolPointingDown, pistolShoulderPos };

    //ideal shoulder rest coordinates when looking to the side, up or down 
    private static Vector2 shoulderPointingRight = new Vector2(2.077f, 2.722f);
    private static Vector2 shoulderPointingUp = new Vector2(-0.1804f, 4.209f);
    private static Vector2 shoulderPointingDown = new Vector2(1.15f, 0.578f);
    private static Vector2 shoulderShoulderPos = new Vector2(0f, 2.7f);

    public static List<Vector2> ShoulderRestCoordinates = new List<Vector2>()
    { shoulderPointingRight, shoulderPointingUp, shoulderPointingDown, shoulderShoulderPos };
}
