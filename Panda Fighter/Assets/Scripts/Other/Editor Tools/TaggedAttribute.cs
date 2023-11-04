using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class TaggedAttribute : PropertyAttribute
{
    public string tagFieldName;

    public TaggedAttribute(string tagFieldName)
    {
        this.tagFieldName = tagFieldName;
    }
}