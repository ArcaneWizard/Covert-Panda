using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using UnityEditor.Experimental.SceneManagement;

public class LimbSwapper : MonoBehaviour
{
    //what limb this gameobject should be
    public LimbTypes limbType; 

    //collection of all limbs
    public LimbSettings limbSettings; 

    private Transform armBones;
    private int counter;
    private SpriteRenderer sR;
    private UnityEngine.U2D.Animation.SpriteSkin spriteSkin;
    
    void Start()
    {
        initializeComponents();
        updateSpriteAndBoneTransforms();

        Transform creature = limbSettings.transform.parent.parent;
        Orderer.UpdateLimbOrder(limbType, sR, creature);
        Colorer.UpdateLimbColor(limbType, sR, creature);

        Destroy(this);
    }

    // Initialize components if they are null
    private void initializeComponents()
    {
        if (!sR)
            sR = transform.GetComponent<SpriteRenderer>();

        if (!spriteSkin)
            spriteSkin = transform.GetComponent<UnityEngine.U2D.Animation.SpriteSkin>();
    }

    // Updates a given limb to be what it's set to. Ie. update the sprite to be a left arm, or a right foot, or a head, etc.
    // Also reattache the bone rigged to that limb (ex. head bone) to this new sprite 
    private void updateSpriteAndBoneTransforms()
    {   
        sR.sprite = limbSettings.ReturnLimb(limbType);
        if (spriteSkin.boneTransforms.Length > 0)
            spriteSkin.boneTransforms[0] = spriteSkin.rootBone;

        PolygonCollider2D col = spriteSkin.rootBone.GetComponent<PolygonCollider2D>();
        if (col)
            col.points = limbSettings.ReturnCollider(limbType);
        else
            Debug.LogError("Limb bone doesn't have a polygon collider 2D attached");
    }


#if (UNITY_EDITOR)

    // retrieve all limbs from the limb collection and update them. Runs:
    // 1) when this limb is updated in the editor and needs to be retrieved (ex. left arm changed to right arm)
    // 2) when the editor scene is first loaded so all limbs need to be retrieved. Note: the 1 sec delay ensures 
    // the creature type was registered first as the limb collection is dependent on the creature type
    async void OnValidate()
    {
        // don't do anything in play mode
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        // don't do anything if in prefab-mode
        PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        bool isValidPrefabStage = prefabStage != null && prefabStage.stageHandle.IsValid();
        bool prefabConnected = PrefabUtility.GetPrefabInstanceStatus(gameObject) == PrefabInstanceStatus.Connected;
        if (isValidPrefabStage || !prefabConnected)
            return;

        await Task.Delay(10);
        if (!findlimbCollection())
            return;

        if (!limbSettings || !limbSettings.transform.parent)
            return;

        initializeComponents();
        updateSpriteAndBoneTransforms();

        Transform creature = limbSettings.transform.parent.parent;
        Orderer.UpdateLimbOrder(limbType, sR, creature);
        Colorer.UpdateLimbColor(limbType, sR, creature);
    }
    
    /* Retrieve all limbs from the limb collection and update them. Runs whenever
    // whnever the editor detects a change was made in the hierarchy (ie. creature type was changed)
    // Only updates limbs that were actually changed 
    void Update()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;
        
        if (!limbCollection) 
            Debug.LogError(("Limb Collection has not been set for " + transform.name));

        initializeComponents();
        if (sR.sprite != limbCollection.ReturnLimb(limbType))
            updateSpriteAndBoneTransforms();
    }*/
    

    // Automates the task of manually dragging in the limb collection to this object's hierarchy. 
    // Call at the top of OnValidate to run
    private bool findlimbCollection()
    {
        counter =  0;
        armBones = transform;
 
        while (!limbSettings && counter <= 8)
        {
            counter++;

            if (armBones != null)
                armBones = armBones.parent;

            if (armBones != null && armBones.GetComponent<LimbSettings>())
                limbSettings = armBones.GetComponent<LimbSettings>();
            else
                return false;
        }

        UnityEditor.EditorUtility.SetDirty(this);
        return true;
    }

#endif

}

