using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class LimbCollection : MonoBehaviour
{
    public Sprite chest;
    public Sprite mainArmPad;
    public Sprite mainArm;
    public Sprite mainHand;
    public Sprite backArmPad;
    public Sprite backArm;
    public Sprite backHand;
    public Sprite leftThigh;
    public Sprite leftLeg;
    public Sprite leftFoot;
    public Sprite rightThigh;
    public Sprite rightLeg;
    public Sprite rightFoot;
    public Sprite head;

    public Dictionary<limbTypes, Sprite> sprites = new Dictionary<limbTypes, Sprite>();

#if (UNITY_EDITOR)
    void OnValidate()
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
            updateSprites();
    }
#endif

    void Awake() => updateSprites();

    private void updateSprites()
    {
        sprites.Clear();

        sprites.Add(limbTypes.chest, chest);
        sprites.Add(limbTypes.leftThigh, leftThigh);
        sprites.Add(limbTypes.rightThigh, rightThigh);
        sprites.Add(limbTypes.leftFoot, leftFoot);
        sprites.Add(limbTypes.rightFoot, rightFoot);
        sprites.Add(limbTypes.leftLeg, leftLeg);
        sprites.Add(limbTypes.rightLeg, rightLeg);
        sprites.Add(limbTypes.mainArm, mainArm);
        sprites.Add(limbTypes.backArm, backArm);
        sprites.Add(limbTypes.mainArmPad, mainArmPad);
        sprites.Add(limbTypes.backArmPad, backArmPad);
        sprites.Add(limbTypes.mainHand, mainHand);
        sprites.Add(limbTypes.backHand, backHand);
        sprites.Add(limbTypes.head, head);
    }

    public Sprite returnSprite(limbTypes limbType)
    {
        if (sprites.Count == 0)
            updateSprites();

        return sprites[limbType];
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