using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class WeaponHeld : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private Transform body; 

    void Start()
    {
        Orderer.UpdateWeaponOrder(weapon, transform.GetComponent<SpriteRenderer>(), body);
        Destroy(this);
    }

    void OnValidate()
    {
        // don't do anything if in edit-mode
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        // don't do anything if in prefab-mode
        PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        bool isValidPrefabStage = prefabStage != null && prefabStage.stageHandle.IsValid();
        bool prefabConnected = PrefabUtility.GetPrefabInstanceStatus(this.gameObject) == PrefabInstanceStatus.Connected;
        if (isValidPrefabStage || !prefabConnected)
            return;

        renameObjectInEditor();
        Orderer.UpdateWeaponOrder(weapon, transform.GetComponent<SpriteRenderer>(), body);
    }

    private void renameObjectInEditor()
    {
        StringBuilder newTag = new StringBuilder("");
        foreach (char c in weapon.ToString())
        {
            newTag.Append(c >= 'A' && c <= 'Z' ? " " + c : c.ToString());
        }

        gameObject.name = newTag.ToString();
    }
}
