using UnityEditor;


using UnityEngine;

[ExecuteAlways]
public class LimbSwapper : MonoBehaviour
{
    // what type of limb to swap in
    [SerializeField] private LimbTypes limbType;

    // can retrieve any limb's sprite and collider settings
    [SerializeField] private LimbSettings limbSettings;

    private SpriteRenderer sR;
    private UnityEngine.U2D.Animation.SpriteSkin spriteSkin;

    void Start()
    {
        if (!Application.isPlaying)
            return;

        this.NullCheck(limbSettings, nameof(limbSettings));

        updateSpriteAndBoneTransforms(true);

        Transform creature = limbSettings.transform.parent.parent;
        Orderer.UpdateLimbOrder(limbType, sR, creature);
        Colorer.UpdateLimbColor(limbType, sR, creature);

        Destroy(this);
    }
    // Updates a given limb to be what it's set to. Ie. update the sprite to be a left arm, or a right foot, or a head, etc.
    // Also reattache the bone rigged to that limb (ex. head bone) to this new sprite 
    private void updateSpriteAndBoneTransforms(bool forceRefresh)
    {
        if (!sR)
            sR = transform.GetComponent<SpriteRenderer>();

        if (!spriteSkin)
            spriteSkin = transform.GetComponent<UnityEngine.U2D.Animation.SpriteSkin>();

        sR.sprite = limbSettings.ReturnLimb(limbType, forceRefresh);
        if (spriteSkin.boneTransforms.Length > 0)
            spriteSkin.boneTransforms[0] = spriteSkin.rootBone;

        PolygonCollider2D col = spriteSkin.rootBone.GetComponent<PolygonCollider2D>();
        if (col)
            col.points = limbSettings.ReturnCollider(limbType, forceRefresh);
        else
            Debug.LogError("Limb bone doesn't have a polygon collider 2D attached");
    }


#if (UNITY_EDITOR)

    // update the limb sprites and colliders in the editor when a limb is changed
    void OnValidate() => updateLimbsInEditor(false);

    // update the limb sprites and colliders in the editor when a creature is toggled on/off
    void OnEnable() => updateLimbsInEditor(true);

    private void updateLimbsInEditor(bool forceRefresh)
    {
        // don't do anything in play mode
        if (Application.isPlaying || !gameObject.activeInHierarchy)
            return;

        // don't do anything if in prefab-mode
        UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();

        if (prefabStage != null) {
            bool isValidPrefabStage = prefabStage != null && prefabStage.stageHandle.IsValid();
            bool prefabConnected = PrefabUtility.GetPrefabInstanceStatus(gameObject) == PrefabInstanceStatus.Connected;
            if (isValidPrefabStage || !prefabConnected)
                return;
        }

        if (!findLimbSettings())
            return;

        updateSpriteAndBoneTransforms(forceRefresh);

        Transform creature = limbSettings.transform.parent.parent;
        Orderer.UpdateLimbOrder(limbType, sR, creature);
        Colorer.UpdateLimbColor(limbType, sR, creature);
    }

    // Automates the task of manually dragging in the limb collection to this object's hierarchy.
    private bool findLimbSettings()
    {
        int counter = 0;
        Transform body = transform;

        while (!limbSettings && body && counter <= 8) {
            body = body.parent;

            if (body && body.GetComponent<LimbSettings>())
                limbSettings = body.GetComponent<LimbSettings>();

            counter++;
        }

        if (!limbSettings)
            return false;

        UnityEditor.EditorUtility.SetDirty(this);
        return true;
    }

#endif

}