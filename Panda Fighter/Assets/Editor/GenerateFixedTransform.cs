using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System;

/*
/// <summary> Auto generates a fixed transform script for a gameObject, so one can fix the transform values in place and
/// discard accidental future changes in the editor </summary>
/// 
[CustomEditor(typeof(Transform))]
public class GeneratedFixedTransform : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Fix Transform"))
        {
            Transform selectedTransform = (Transform)target;

            // generate unique name for the name of the class
            string uniqueName = $"{selectedTransform.name}_{selectedTransform.GetInstanceID()}";
            string validUniqueName = Regex.Replace(uniqueName, "[^a-zA-Z0-9_]", "");

            string scriptText = "using UnityEngine;\n\n";
            scriptText += $"public class {validUniqueName} : FixedTransform\n";
            scriptText += "{\n";
            scriptText += "    void Awake()\n";
            scriptText += "    {\n";
            scriptText += "        transform.position = new Vector3(" + selectedTransform.position.x + "f, " + selectedTransform.position.y + "f, " + selectedTransform.position.z + "f);\n";
            scriptText += "        transform.rotation = Quaternion.Euler(" + selectedTransform.eulerAngles.x + "f, " + selectedTransform.eulerAngles.y + "f, " + selectedTransform.eulerAngles.z + "f);\n";
            scriptText += "        transform.localScale = new Vector3(" + selectedTransform.localScale.x + "f, " + selectedTransform.localScale.y + "f, " + selectedTransform.localScale.z + "f);\n";
            scriptText += "    }\n";
            scriptText += "}\n";

            string dirPath = "Assets/Scripts/Custom Tools/Fixed Transforms/";
            string path = dirPath + $"{validUniqueName}.cs";
            System.IO.File.WriteAllText(path, scriptText);
            AssetDatabase.ImportAsset(path);

            Type componentType = Type.GetType(validUniqueName);
            selectedTransform.gameObject.AddComponent(componentType);
        }
    }
}


*/
