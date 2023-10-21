
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TaggedAttribute))]
public class TaggedDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        Debug.Log(position.ToString());
        string tooltip = "This attribute helps specify what reference should be dragged in. " +
               "However, this attribute only works for Object references.";
        Rect toolTipPosition = position;
        toolTipPosition.x -= (position.width/2 - 45);
        EditorGUI.LabelField(toolTipPosition, new GUIContent(label.text, tooltip));

        // Ensure the property is of the correct type (TaggedObject).
        if (property.propertyType == SerializedPropertyType.ObjectReference)
        {
            // Draw the default object field.
            EditorGUI.PropertyField(position, property, label);
            string desiredTag = (attribute as TaggedAttribute).tagFieldName;

            // Get the assigned object
            GameObject obj = null;
            if (property.objectReferenceValue is GameObject)
                obj = property.objectReferenceValue as GameObject; 

            else if (property.objectReferenceValue is Transform)
                obj = (property.objectReferenceValue as Transform).gameObject;

            else if (property.objectReferenceValue is Component)
                obj = (property.objectReferenceValue as Component).gameObject;

            if (obj != null)
            {
                // Check if the object has a Tag script
                Tag tagScript = obj.GetComponent<Tag>();

                bool foundTag = false;
                if (tagScript != null)
                {
                    foreach (string tag in tagScript.Tags)
                    {
                        if (tag.Equals(desiredTag))
                        {
                            foundTag = true;
                            break;
                        }
                    }
                }

                Rect errorRect = position;
                errorRect.x += position.width;
                errorRect.x -= 200;
                if (!foundTag)
                  EditorGUI.HelpBox(errorRect, $"Missing tag [{desiredTag}]", MessageType.Error);

                if (!foundTag && Application.isPlaying) {
                    string scriptName = property.serializedObject.targetObject.GetType().Name;
                    Debug.LogError($"<color=green><b>{obj.name}</b></color> doesn't have the required tag <color=green><b>{desiredTag}</b></color>" +
                        $" on the Script <b>{scriptName}</b>. Double check if the correct object was referenced, and if so, attach a tag to the referenced object.");
                }
            }
        }

        EditorGUI.EndProperty();
    }
}