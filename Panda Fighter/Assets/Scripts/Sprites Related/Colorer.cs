using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

/* Adds color filters to the sprites of weapons, creature limbs, etc. at runtime to ensure uniformity in the entire game.
 * Ex. it should be easy to change the hue of a specific weapon & be confident that this change will apply to the entire game. */

public static class Colorer 
{
    private static Dictionary<LimbTypes, Color32> relativeLimbColors;
    private static Dictionary<Weapon, Color32> relativeWeaponColors;

    private static Color32 defaultColor = new Color32(255, 255, 255, 255);
    private static Color32 AmphelotColor = new Color32(173, 215, 255, 255);

    // Update specified limb's color
    public static void UpdateLimbColor(LimbTypes limb, SpriteRenderer renderer, Transform creature)
    {
        if (renderer == null)
            return;

        if (relativeLimbColors == null)
            initializeColorOfLimbs();

        renderer.color = relativeLimbColors[limb];
    }

    // Update specified weapon's color
    public static void UpdateWeaponColor(Weapon weapon, SpriteRenderer renderer, Transform creature)
    {
        if (renderer == null)
            return;

        if (relativeWeaponColors == null)
            initializeColorOfWeapons();

        renderer.color = relativeWeaponColors[weapon];
    }

    private static void initializeColorOfWeapons()
    {
        relativeWeaponColors = new Dictionary<Weapon, Color32>();
    }

    private static void initializeColorOfLimbs()
    {
        relativeLimbColors = new Dictionary<LimbTypes, Color32>();

        relativeLimbColors[LimbTypes.FrontUpperArm] = AmphelotColor;
        relativeLimbColors[LimbTypes.FrontLowerArm] = AmphelotColor;
        relativeLimbColors[LimbTypes.FrontHand] = AmphelotColor;

        relativeLimbColors[LimbTypes.FrontThigh] = AmphelotColor;
        relativeLimbColors[LimbTypes.FrontLeg] = AmphelotColor;
        relativeLimbColors[LimbTypes.FrontFoot] = AmphelotColor;

        relativeLimbColors[LimbTypes.Head] = AmphelotColor;
        relativeLimbColors[LimbTypes.Chest] = AmphelotColor;

        relativeLimbColors[LimbTypes.BackUpperArm] = AmphelotColor;
        relativeLimbColors[LimbTypes.BackLowerArm] = AmphelotColor;
        relativeLimbColors[LimbTypes.BackHand] = AmphelotColor;

        relativeLimbColors[LimbTypes.BackThigh] = AmphelotColor;
        relativeLimbColors[LimbTypes.BackLeg] = AmphelotColor;
        relativeLimbColors[LimbTypes.BackFoot] = AmphelotColor;
    }

}