using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

/* Orders the sprites such to prevent overlaping sprite glitches between distinct creatures
 * or within a single creature. Provides methods to orders the limbs of creatures or any provided sprite." */

public static class Orderer 
{
    private static Dictionary<LimbTypes, int> relativeLimbOrder;
    private static Dictionary<Weapon, OrderInfo> relativeWeaponOrder;

    private struct OrderInfo
    {
        public string sortingLayer;
        public int order;

        public OrderInfo(string sortingLayer, int order)
        {
            this.sortingLayer = sortingLayer;
            this.order = order;
        }
    }

    // update the provided sprite's order to prevent overlap glitches
    public static void UpdateSpriteOrder(SpriteRenderer renderer, Transform creature) 
    {
        if (renderer == null)
            return;

        renderer.sortingOrder = getUniqueOrderForThisCreature(renderer.sortingOrder, creature);
    }

    // handle limb ordering to prevent overlap glitches between limbs or different creatures
    public static void UpdateLimbOrder(LimbTypes limb, SpriteRenderer renderer, Transform creature)
    { 
        if (renderer == null)
            return;

        if (relativeLimbOrder == null)
            setRelativeOrderOfLimbs();

        renderer.sortingLayerName = SortingLayer.Default;
        renderer.sortingOrder = getUniqueOrderForThisCreature(relativeLimbOrder[limb], creature);
    }

    // handle weapon ordering to prevent overlap glitches between weapons/limbs or between weapons of different creatures
    public static void UpdateWeaponOrder(Weapon weapon, SpriteRenderer renderer, Transform creature)
    {
        if (renderer == null)
            return;

        OrderInfo info = getWeaponOrderInfo(weapon);
        renderer.sortingLayerName = info.sortingLayer;
        renderer.sortingOrder = getUniqueOrderForThisCreature(info.order, creature);
    }

    // applies offset to the provided order to prevent overlap glitches across different creatures
    private static int getUniqueOrderForThisCreature(int order, Transform creature)
    {
        if (order < 0 || order >= 100)
            order %= 100;

        if (creature.tag.Equals("Player"))
            return order + 10000;
        else
            return order + 100 * (creature.GetSiblingIndex() + 1);
    }

    private static OrderInfo getWeaponOrderInfo(Weapon weapon)
    {
        if (relativeWeaponOrder == null)
            setRelativeOrderOfWeapons();

        if (relativeWeaponOrder.TryGetValue(weapon, out OrderInfo info))
            return info;
        else
            return defaultWeaponInfo;
    }

    private static OrderInfo defaultWeaponInfo = new OrderInfo(SortingLayer.Default, 78);

    private static void setRelativeOrderOfWeapons()
    {
        relativeWeaponOrder = new Dictionary<Weapon, OrderInfo>();
        relativeWeaponOrder[Weapon.RocketLauncher] = new OrderInfo(SortingLayer.Default, 40);
        relativeWeaponOrder[Weapon.ArcticSprayer] = new OrderInfo(SortingLayer.Default, 40);
    }

    private static void setRelativeOrderOfLimbs()
    {
        relativeLimbOrder = new Dictionary<LimbTypes, int>();

        relativeLimbOrder[LimbTypes.FrontUpperArm] = 83;
        relativeLimbOrder[LimbTypes.FrontLowerArm] = 81;
        relativeLimbOrder[LimbTypes.FrontHand] = 99;

        relativeLimbOrder[LimbTypes.FrontThigh] = 72;
        relativeLimbOrder[LimbTypes.FrontLeg] = 71;
        relativeLimbOrder[LimbTypes.FrontFoot] = 70;

        relativeLimbOrder[LimbTypes.Head] = 58;
        relativeLimbOrder[LimbTypes.Chest] = 56;

        relativeLimbOrder[LimbTypes.BackUpperArm] = 32;
        relativeLimbOrder[LimbTypes.BackLowerArm] = 31;
        relativeLimbOrder[LimbTypes.BackHand] = 30;

        relativeLimbOrder[LimbTypes.BackThigh] = 22;
        relativeLimbOrder[LimbTypes.BackLeg] = 21;
        relativeLimbOrder[LimbTypes.BackFoot] = 20;
    }

}