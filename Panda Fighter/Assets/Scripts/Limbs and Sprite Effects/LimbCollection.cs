using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LimbCollection : MonoBehaviour
{
    [HideInInspector] public Creatures creature;
    public Dictionary<LimbTypes, Sprite> limbs = new Dictionary<LimbTypes, Sprite>();

    //update the collection of limbs for this creature type, stored in a dictionary by limb name
    public void UpdateLimbs()
    {
        limbs.Clear();

        limbs.Add(LimbTypes.Chest, creature.chest);
        limbs.Add(LimbTypes.FrontThigh, creature.leftThigh);
        limbs.Add(LimbTypes.BackThigh, creature.rightThigh);
        limbs.Add(LimbTypes.FrontFoot, creature.leftFoot);
        limbs.Add(LimbTypes.BackFoot, creature.rightFoot);
        limbs.Add(LimbTypes.FrontLeg, creature.leftLeg);
        limbs.Add(LimbTypes.BackLeg, creature.rightLeg);
        limbs.Add(LimbTypes.FrontLowerArm, creature.mainArm);
        limbs.Add(LimbTypes.BackLowerArm, creature.backArm);
        limbs.Add(LimbTypes.FrontUpperArm, creature.mainArmPad);
        limbs.Add(LimbTypes.BackUpperArm, creature.backArmPad);
        limbs.Add(LimbTypes.FrontHand, creature.mainHand);
        limbs.Add(LimbTypes.BackHand, creature.backHand);
        limbs.Add(LimbTypes.Head, creature.head);
    }

    //return a limb based off limb name
    public Sprite ReturnLimb(LimbTypes limbType)
    {
        if (limbs.Count == 0)
            UpdateLimbs();

        return limbs[limbType];
    }

}

public enum LimbTypes
{
    Chest,
    FrontUpperArm,
    FrontLowerArm,
    FrontHand,
    BackUpperArm,
    BackLowerArm,
    BackHand,
    FrontThigh,
    FrontLeg,
    FrontFoot,
    BackThigh,
    BackLeg,
    BackFoot,
    Head
}