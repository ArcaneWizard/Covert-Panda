
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static Vector2 levitationBoost = new Vector2(0, 750);
    public static LayerMask map = (1 << 11);
    public static LayerMask mapOrPlayer = (1 << 11) | (1 << 12);
    public static float maxPlatformTilt = 46f;
}
