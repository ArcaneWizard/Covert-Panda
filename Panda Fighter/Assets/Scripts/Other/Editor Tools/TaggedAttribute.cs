using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class TaggedAttribute : PropertyAttribute
{
    public string TagFieldName;

    public TaggedAttribute(string tagFieldName)
    {
        TagFieldName = tagFieldName;
    }
}