using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;


/// <summary> 
/// Adds color filters to the sprites of weapons, creature limbs, etc. at runtime to ensure uniformity in the entire game.
/// Ex. it should be easy to change the hue of a specific weapon & be confident that this change will apply to the entire game.
/// </summary>

public static class Colorer 
{
    private static Dictionary<LimbTypes, Color32> relativeLimbColors;

    private static Color32 defaultColor = new Color32(255, 255, 255, 255);
    private static Color32 AmphelotColor = new Color32(182, 200, 217, 255);

    // Update specified limb's color
    public static void UpdateLimbColor(LimbTypes limb, SpriteRenderer renderer, Transform creature)
    {
        if (renderer == null)
            return;

        if (relativeLimbColors == null)
            initializeColorOfLimbs(defaultColor);

        renderer.color = relativeLimbColors[limb];
    }

    private static void initializeColorOfLimbs(Color32 color)
    {
        relativeLimbColors = new Dictionary<LimbTypes, Color32>();

        relativeLimbColors[LimbTypes.FrontUpperArm] = color;
        relativeLimbColors[LimbTypes.FrontLowerArm] = color;
        relativeLimbColors[LimbTypes.FrontHand] = color;

        relativeLimbColors[LimbTypes.FrontThigh] = color;
        relativeLimbColors[LimbTypes.FrontLeg] = color;
        relativeLimbColors[LimbTypes.FrontFoot] = color;

        relativeLimbColors[LimbTypes.Head] = color;
        relativeLimbColors[LimbTypes.Chest] = color;

        relativeLimbColors[LimbTypes.BackUpperArm] = color;
        relativeLimbColors[LimbTypes.BackLowerArm] = color;
        relativeLimbColors[LimbTypes.BackHand] = color;

        relativeLimbColors[LimbTypes.BackThigh] = color;
        relativeLimbColors[LimbTypes.BackLeg] = color;
        relativeLimbColors[LimbTypes.BackFoot] = color;
    }

}