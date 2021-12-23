using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class LimbSwapper : MonoBehaviour
{
    public limbTypes limbType;

    private Transform armBones;
    public LimbCollection spriteCollection;

    private SpriteRenderer sR;
    private UnityEngine.U2D.Animation.SpriteSkin spriteSkin;

    void Start()
    {
        initializeVariables();
        updateSpriteAndBoneTransforms();
    }

#if (UNITY_EDITOR)
    void OnValidate()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        initializeVariables();
        updateSpriteAndBoneTransforms();
    }

    void Update()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        initializeVariables();
        if (sR.sprite != spriteCollection.returnSprite(limbType))
            updateSpriteAndBoneTransforms();
    }
#endif

    // Initialize the entity's collection of sprites for each limb. Using that, 
    // update the sprite for this gameobject based on the limb type selected. Also
    // update the bone associated with this sprite to be the same one associated with
    // the previous sprite.
    private void updateSpriteAndBoneTransforms()
    {
        sR.sprite = spriteCollection.returnSprite(limbType);
        if (spriteSkin.boneTransforms.Length > 0)
            spriteSkin.boneTransforms[0] = spriteSkin.rootBone;
    }

    // Initialize the entity's collection of sprites for each limb 
    private void findSpriteCollection()
    {
        if (!armBones)
            armBones = transform.parent;

        while (!spriteCollection && armBones)
        {
            if (armBones.GetComponent<LimbCollection>())
                spriteCollection = armBones.GetComponent<LimbCollection>();
            else
                armBones = armBones.parent;
        }
    }

    // Initialize the private fields if they aren't already defined
    private void initializeVariables()
    {
        if (!sR)
            sR = transform.GetComponent<SpriteRenderer>();

        if (!spriteSkin)
            spriteSkin = transform.GetComponent<UnityEngine.U2D.Animation.SpriteSkin>();
    }
}

