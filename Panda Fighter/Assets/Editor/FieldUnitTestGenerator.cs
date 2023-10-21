using UnityEngine;
using UnityEditor;

public class FieldUnitTestGenerator : EditorWindow
{
    [MenuItem("Window/Generate Field Unit Tests")]
    public static void ShowWindow()
    {
        GetWindow<FieldUnitTestGenerator>("Field Unit Test Generator");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Generate Field Unit Tests"))
        {
            GenerateFieldUnitTests();
        }
    }

    private void GenerateFieldUnitTests()
    {
        // Get the selected object in the Hierarchy
        GameObject selectedObject = Selection.activeGameObject;

        if (selectedObject == null)
        {
            Debug.LogError("Select a GameObject in the Hierarchy to generate field unit tests.");
            return;
        }

        // Get all MonoBehaviour components attached to the selected GameObject
        MonoBehaviour[] components = selectedObject.GetComponents<MonoBehaviour>();
        Debug.Log(UnityEditor.AssetDatabase.AssetPathToGUID(UnityEditor.AssetDatabase.GetAssetPath(selectedObject)));

        foreach (MonoBehaviour component in components)
        {
            SerializedObject serializedObject = new SerializedObject(component);
            SerializedProperty property = serializedObject.GetIterator();

            // Iterate through the serialized fields
            while (property.NextVisible(true))
            {
                // Check if the property is a field (not a child property)
                if (property.propertyPath == property.name)
                {
                    // Generate a test case for the field
                    GenerateFieldTestCase(component, property);
                }
            }
        }

        Debug.Log("Field unit tests generated for " + components.Length + " components.");
    }

    private void GenerateFieldTestCase(MonoBehaviour component, SerializedProperty property)
    {
        // Generate a test case based on the field's current value
        string testCase = "[Test]\n";
        testCase += "public void Test" + component.GetType().Name + "_" + property.name + "()\n";
        testCase += "{\n";
        testCase += "    " + component.GetType().Name + " obj = new " + component.GetType().Name + "();\n";
        testCase += "    obj." + property.name + " = " + ValueToString(property) + ";\n";
        testCase += "    Assert.AreEqual(" + ValueToString(property) + ", obj." + property.name + ");\n";
        testCase += "}\n";

        // Log or save the generated test case
        Debug.Log(testCase);
    }

    private string ValueToString(SerializedProperty property)
    {
        // Convert the field value to a string representation
        switch (property.propertyType)
        {
            case SerializedPropertyType.Integer:
                return property.intValue.ToString();
            case SerializedPropertyType.Boolean:
                return property.boolValue ? "true" : "false";
            case SerializedPropertyType.Float:
                return property.floatValue.ToString();
            case SerializedPropertyType.String:
                return "\"" + property.stringValue + "\"";
            default:
                return "null";
        }
    }
}