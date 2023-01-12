using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Orders the sprites such to prevent overlaping sprite glitches between distinct creatures
 * or within a single creature. Provides methods to orders the limbs of creatures or any provided sprite." */

public class Orderer : MonoBehaviour 
{
    private static Dictionary<LimbTypes, int> orderOfLimbs;
    private static Dictionary<Weapon, int> orderOFWeapons;

    // handle any sprite's ordering
    public static void UpdateSpriteOrder(SpriteRenderer renderer, Transform entity) 
    {
        if (renderer == null)
            return;

        int order = renderer.sortingOrder;
        if (entity.tag.Equals("Player"))
            renderer.sortingOrder = order % 100 + 10000;
        else
            renderer.sortingOrder = order % 100 + 100 * entity.GetSiblingIndex() + 100;
    }

    // handle limb ordering
    public static void UpdateLimbOrder(LimbTypes limb, SpriteRenderer renderer, Transform entity)
    { 
        if (renderer == null)
            return;

        if (orderOfLimbs == null)
            initializeOrderOfLimbs();

        renderer.sortingLayerName = "Default";
        renderer.sortingOrder = orderOfLimbs[limb];

        if (entity.tag.Equals("Player"))
            renderer.sortingOrder = orderOfLimbs[limb] % 100 + 10000;
        else
            renderer.sortingOrder = orderOfLimbs[limb] % 100 + 100 * entity.GetSiblingIndex() + 100;
    }

    private static void initializeOrderOfLimbs()
    {
        orderOfLimbs = new Dictionary<LimbTypes, int>();

        orderOfLimbs[LimbTypes.FrontUpperArm] = 42;
        orderOfLimbs[LimbTypes.FrontLowerArm] = 41;
        orderOfLimbs[LimbTypes.FrontHand] = 40;

        orderOfLimbs[LimbTypes.FrontThigh] = 32;
        orderOfLimbs[LimbTypes.FrontLeg] = 31;
        orderOfLimbs[LimbTypes.FrontFoot] = 30;

        orderOfLimbs[LimbTypes.Head] = 28;
        orderOfLimbs[LimbTypes.Chest] = 26;

        orderOfLimbs[LimbTypes.BackUpperArm] = 22;
        orderOfLimbs[LimbTypes.BackLowerArm] = 21;
        orderOfLimbs[LimbTypes.BackHand] = 20;

        orderOfLimbs[LimbTypes.BackThigh] = 12;
        orderOfLimbs[LimbTypes.BackLeg] = 11;
        orderOfLimbs[LimbTypes.BackFoot] = 10;
    }
}