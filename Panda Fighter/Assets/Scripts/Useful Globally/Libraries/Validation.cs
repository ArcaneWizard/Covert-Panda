using System.Text;
using System.Text.RegularExpressions;

using UnityEditor;

using UnityEngine;

/// <summary> Can perform null checks and require objects to have tags. </summary>
/// Prepare to start validating code LIKE A BOSS *puts on sunglasses*
public static class Validation
{
    private const string SUCCESSFUL_SELECTION = "Selection succeeded";

    public interface IValidator
    {
        public Object Reference { get; }
        public string Name { get; }
    }

    public class NotNull : IValidator
    {
        public Object Reference { get; set; }
        public string Name { get; set; }

        public NotNull(Object reference, string name)
        {
            Reference = reference;
            Name = name;
        }

        public override string ToString() =>
            $"Reference: {Reference}, name: {Name}";
    }

    public class RequiredTag : IValidator
    {
        public Object Reference { get; set; }
        public string Name { get; set; }
        public string DesiredTag { get; set; }

        public RequiredTag(Object reference, string variableName, string desiredTag)
        {
            Reference = reference;
            Name = variableName;
            DesiredTag = desiredTag;
        }

        public override string ToString() =>
            $"Reference: {Reference}, name: {Name}, DesiredTag: {DesiredTag}";
    }

    /// <summary> Confirm specified reference isn't null; reference's name is required for error msgs </summary>
    public static void NullCheck(this MonoBehaviour mb, Object reference, string name)
    {
#if UNITY_EDITOR
        (bool error, string errorMsg) = confirmNotNull(mb, new NotNull(reference, name));

        if (error)
            throw new System.NullReferenceException(errorMsg);
#endif
    }

    /// <summary> Require specified reference to have specified object tag; reference name is needed 
    /// for error msgs. </summary>
    public static void RequireTag(this MonoBehaviour mb, Object reference, string name, string desiredTag)
    {
#if UNITY_EDITOR
        (bool nullError, string nullErrorMsg) = confirmNotNull(mb, new NotNull(reference, name));
        (bool tagError, string tagErrorMsg) = requireTag(mb, name, desiredTag);

        if (nullError)
            throw new System.NullReferenceException(nullErrorMsg);
        else if (tagError)
            throw new UnassignedReferenceException(tagErrorMsg);
        else if (!EditorApplication.isPlaying)
            Debug.Log(SUCCESSFUL_SELECTION);
#endif
    }

    /// <summary> Validate a bunch of references (do null checks, require object tags, etc.) </summary>
    public static void Validate(this MonoBehaviour mb, params IValidator[] validators)
    {
#if UNITY_EDITOR
        (bool error, string errorMsg) nullRefError;
        (bool error, string errorMsg) tagError;

        bool hasError = false;
        StringBuilder finalErrorMsg = new StringBuilder();
        for (int i = 0; i < validators.Length; i++) {
            if (validators[i] is RequiredTag) {
                RequiredTag req = (RequiredTag)(validators[i]);
                nullRefError = confirmNotNull(mb, new NotNull(req.Reference, req.Name));
                tagError = requireTag(mb, req.Name, req.DesiredTag);

                if (nullRefError.error || tagError.error) {
                    hasError = true;
                    finalErrorMsg.Append(nullRefError.errorMsg + tagError.errorMsg + ". \n");
                }
            } else {
                nullRefError = confirmNotNull(mb, (NotNull)validators[i]);

                if (nullRefError.error) {
                    hasError = true;
                    finalErrorMsg.Append(nullRefError.errorMsg + ". \n");
                }
            }
        }

        if (hasError)
            throw new System.Exception(finalErrorMsg.ToString());

        else if (!hasError && !EditorApplication.isPlaying)
            Debug.Log(SUCCESSFUL_SELECTION);
#endif
    }

    /* Null checks for references of type object. Has bugs so not implemented yet
     * public static void ConfirmNotNull(this MonoBehaviour mb, object reference, string name)
    {
        #if UNITY_EDITOR
            if (reference == null)
            {
                throw new System.NullReferenceException($"The variable <color=green><b>[{name}]</b></color> is " +
                    $"null on the Script " +
                    $"<b>{mb.GetType()}</b> on <b>{mb.gameObject.name}</b>");
            }
        #endif
    }*/

    // returns bool error, string errorMsg
    private static (bool, string) requireTag(MonoBehaviour mb, string varName, string desiredTag)
    {
        SerializedObject serializedObj = new SerializedObject(mb);
        SerializedProperty property = serializedObj.FindProperty(varName);

        // Ensure the property is of the correct type (ObjectReference).
        if (property.propertyType == SerializedPropertyType.ObjectReference) {
            // Get the assigned object
            GameObject obj = null;
            if (property.objectReferenceValue is GameObject)
                obj = property.objectReferenceValue as GameObject;
            else if (property.objectReferenceValue is Transform)
                obj = (property.objectReferenceValue as Transform).gameObject;
            else if (property.objectReferenceValue is Component)
                obj = (property.objectReferenceValue as Component).gameObject;

            if (obj == null)
                return (false, "");

            // Check if the object has a Tag script
            Tag tagScript = obj.GetComponent<Tag>();

            bool foundTag = false;
            if (tagScript != null) {
                string desiredTagWithoutWhiteSpace = Regex.Replace(desiredTag, @"\s*", "").ToLower();
                foreach (string tag in tagScript.Tags) {
                    string currentTagWithoutWhiteSpace = Regex.Replace(tag, @"\s*", "").ToLower();
                    if (currentTagWithoutWhiteSpace.Equals(desiredTagWithoutWhiteSpace)) {
                        foundTag = true;
                        break;
                    }
                }
            }

            if (!foundTag) {
                // Handle the error (e.g., display an error message).
                string scriptName = property.serializedObject.targetObject.GetType().Name;

                return (true, $" The reference <color=cyan><b>[{varName}]</b></color> is missing the tag <color=cyan><b>[{desiredTag}]</b></color>" +
                    $" on Script <color=white><b>{scriptName}</b></color>. It's currently set to <color=white><b>[{obj.name}]</b></color>. " +
                    $"Double-check if the correct object was referenced, and if so, attach a tag to the referenced object.");
            }
        }
        return (false, "");
    }

    // returns bool error, string errorMsg
    private static (bool, string) confirmNotNull(MonoBehaviour mb, NotNull validatorInfo)
    {
        if (validatorInfo.Reference == null) {
            return (true, $"The reference <color=cyan><b>[{validatorInfo.Name}]</b></color> is missing on the Script " +
                $"<b>{mb.GetType()}</b> on <b>{mb.gameObject.name}</b>");
        } else
            return (false, "");
    }
}
