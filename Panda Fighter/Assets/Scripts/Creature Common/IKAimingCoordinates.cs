using System.Collections.Generic;

using UnityEngine;

// Stores the Inverse Kinematic (IK) coordinates that properly setup the rotation of a creature's arms
// when aiming a weapon. Coordinates are grouped by which weapon type the arm limbs are meant to hold

public static class IKAimingCoordinates
{
    // ideal default-barrel weapon coordinates when looking to the side, up or down
    private static Vector2 defaultPointingRight = new Vector2(1.307f, 2.101f);
    private static Vector2 defaultPointingUp = new Vector2(0.27f, 3.29f);
    private static Vector2 defaultPointingDown = new Vector2(0.11f, 0.71f);
    private static Vector2 defaultShoulderPos = new Vector2(0.173f, 2.198f);

    public static List<Vector2> DefaultCoordinates = new List<Vector2>()
    { defaultPointingRight, defaultPointingUp, defaultPointingDown, defaultShoulderPos };

    // ideal long-barrel weapon coordinates when looking to the side, up or down 
    private static Vector2 longBarrelPointingRight = new Vector2(1.307f, 2.101f);
    private static Vector2 longBarrelPointingUp = new Vector2(0.27f, 3.29f);
    private static Vector2 longBarrelPointingDown = new Vector2(0.11f, 0.21f);
    private static Vector2 longBarrelShoulderPos = new Vector2(0.173f, 2.198f);

    public static List<Vector2> LongBarrelCoordinates = new List<Vector2>()
    { longBarrelPointingRight, longBarrelPointingUp, longBarrelPointingDown, longBarrelShoulderPos };

    //ideal meelee grip weapon coordinates when looking to the side, up or down 
    private static Vector2 meeleePointingRight = new Vector2(1.307f, 2.101f);
    private static Vector2 meeleePointingUp = new Vector2(0.27f, 3.29f);
    private static Vector2 meeleePointingDown = new Vector2(0.11f, 0.81f);
    private static Vector2 meeleeShoulderPos = new Vector2(0.228f, 2.155f);
    private static Vector2 meeleeAttackPos = new Vector2(0.672f, 0.977f);

    public static List<Vector2> MeeleeGripCoordinates = new List<Vector2>()
    { meeleePointingRight, meeleePointingUp, meeleePointingDown, meeleeShoulderPos, meeleeAttackPos };

    //ideal short-barrel weapon coordinates when looking to the side, up or down with front arm
    private static Vector2 shortBarrelPointingRight = new Vector2(0.278f, -1.456f);
    private static Vector2 shortBarrelPointingUp = new Vector2(-1.02f, 0.19f);
    private static Vector2 shortBarrelPointingDown = new Vector2(-1.35f, -3.34f);
    private static Vector2 shortBarrelShoulderPos = new Vector2(-1.33f, -1.4f);

    public static List<Vector2> ShortBarrelMainArmCoordinates = new List<Vector2>()
    { shortBarrelPointingRight, shortBarrelPointingUp, shortBarrelPointingDown, shortBarrelShoulderPos };


    //ideal short-barrel weapon coordinates when looking to the side, up or down wih back arm
    private static Vector2 shortBarrelPointingRight2 = new Vector2(1.95f, 0.845f);
    private static Vector2 shortBarrelPointingUp2 = new Vector2(0.41f, 2.34f);
    private static Vector2 shortBarrelPointingDown2 = new Vector2(0.78f, 0.08f);
    private static Vector2 shortBarrelShoulderPos2 = new Vector2(0.36f, 0.67f);

    public static List<Vector2> ShortBarrelOtherArmCoordinates = new List<Vector2>()
    { shortBarrelPointingRight2, shortBarrelPointingUp2, shortBarrelPointingDown2, shortBarrelShoulderPos2 };

    //ideal pistol grip weapon coordinates when looking to the side, up or down
    private static Vector2 pistolPointingRight = new Vector2(1.98f, 2.49f);
    private static Vector2 pistolPointingUp = new Vector2(-0.1f, 3.94f);
    private static Vector2 pistolPointingDown = new Vector2(0.31f, -0.15f);
    private static Vector2 pistolShoulderPos = new Vector2(0.09f, 2.18f);

    public static List<Vector2> PistolGripCoordinates = new List<Vector2>()
    { pistolPointingRight, pistolPointingUp, pistolPointingDown, pistolShoulderPos };

    //ideal shoulder-heavy weapon coordinates when looking to the side, up or down 
    private static Vector2 shoulderPointingRight = new Vector2(2.077f, 2.722f);
    private static Vector2 shoulderPointingUp = new Vector2(-0.1804f, 4.209f);
    private static Vector2 shoulderPointingDown = new Vector2(1.15f, 0.548f);
    private static Vector2 shoulderShoulderPos = new Vector2(0f, 2.7f);

    public static List<Vector2> ShoulderRestCoordinates = new List<Vector2>()
    { shoulderPointingRight, shoulderPointingUp, shoulderPointingDown, shoulderShoulderPos };
}
