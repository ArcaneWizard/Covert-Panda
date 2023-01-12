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

        limbs.Add(LimbTypes.chest, creature.chest);
        limbs.Add(LimbTypes.leftThigh, creature.leftThigh);
        limbs.Add(LimbTypes.rightThigh, creature.rightThigh);
        limbs.Add(LimbTypes.leftFoot, creature.leftFoot);
        limbs.Add(LimbTypes.rightFoot, creature.rightFoot);
        limbs.Add(LimbTypes.leftLeg, creature.leftLeg);
        limbs.Add(LimbTypes.rightLeg, creature.rightLeg);
        limbs.Add(LimbTypes.mainArm, creature.mainArm);
        limbs.Add(LimbTypes.backArm, creature.backArm);
        limbs.Add(LimbTypes.mainArmPad, creature.mainArmPad);
        limbs.Add(LimbTypes.backArmPad, creature.backArmPad);
        limbs.Add(LimbTypes.mainHand, creature.mainHand);
        limbs.Add(LimbTypes.backHand, creature.backHand);
        limbs.Add(LimbTypes.head, creature.head);
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
    chest,
    mainArmPad,
    mainArm,
    mainHand,
    backArmPad,
    backArm,
    backHand,
    leftThigh,
    leftLeg,
    leftFoot,
    rightThigh,
    rightLeg,
    rightFoot,
    head
}