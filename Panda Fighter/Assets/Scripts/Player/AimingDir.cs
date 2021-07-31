using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AimingDir
{
    //ideal gun coordinates when looking to the side, up or down 
    public static Vector2 gunPointingRight = new Vector2(1.307f, 2.101f);
    public static Vector2 gunPointingUp = new Vector2(0.27f, 3.29f);
    public static Vector2 gunPointingDown = new Vector2(0.11f, 0.81f);
    public static Vector2 gunShoulderPos = new Vector2(0.173f, 2.198f);

    //ideal scythe coordinates when looking to the side, up or down 
    public static Vector2 scythePointingRight = new Vector2(1.307f, 2.101f);
    public static Vector2 scythePointingUp = new Vector2(0.27f, 3.29f);
    public static Vector2 scythePointingDown = new Vector2(0.11f, 0.81f);
    public static Vector2 scytheShoulderPos = new Vector2(0.228f, 2.125f);
    public static Vector2 scytheAttackPos = new Vector2(0.672f, 0.977f);
}
