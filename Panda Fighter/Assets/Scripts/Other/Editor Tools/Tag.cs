using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Tag : MonoBehaviour
{
    public List<string> Tags = new List<string>();
    public bool AddName;

    void OnValidate()
    {
        if (AddName)
        {
            AddName = false;
            Tags.Add(gameObject.name);
        }

        Tag[] tags = GetComponents<Tag>();
        if (tags.Length > 1)
            Debug.LogError($"{gameObject.name} has more than 1 tag script. Please remove duplicates");
    }
}