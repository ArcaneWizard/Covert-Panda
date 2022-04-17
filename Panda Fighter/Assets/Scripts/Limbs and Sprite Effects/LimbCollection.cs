using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LimbCollection : MonoBehaviour
{
    public Creatures creature;
    public Dictionary<limbTypes, Sprite> limbs = new Dictionary<limbTypes, Sprite>();

    //update the collection of limbs for this creature type, stored in a dictionary by limb name
    public void updateLimbs()
    {
        limbs.Clear();

        limbs.Add(limbTypes.chest, creature.chest);
        limbs.Add(limbTypes.leftThigh, creature.leftThigh);
        limbs.Add(limbTypes.rightThigh, creature.rightThigh);
        limbs.Add(limbTypes.leftFoot, creature.leftFoot);
        limbs.Add(limbTypes.rightFoot, creature.rightFoot);
        limbs.Add(limbTypes.leftLeg, creature.leftLeg);
        limbs.Add(limbTypes.rightLeg, creature.rightLeg);
        limbs.Add(limbTypes.mainArm, creature.mainArm);
        limbs.Add(limbTypes.backArm, creature.backArm);
        limbs.Add(limbTypes.mainArmPad, creature.mainArmPad);
        limbs.Add(limbTypes.backArmPad, creature.backArmPad);
        limbs.Add(limbTypes.mainHand, creature.mainHand);
        limbs.Add(limbTypes.backHand, creature.backHand);
        limbs.Add(limbTypes.head, creature.head);
    }

    //return a limb based off limb name
    public Sprite returnLimb(limbTypes limbType)
    {
        if (limbs.Count == 0)
            updateLimbs();

        return limbs[limbType];
    }

}

public enum limbTypes
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