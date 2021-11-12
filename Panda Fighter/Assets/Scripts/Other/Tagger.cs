using UnityEngine;
using System.Collections.Generic;

public class Tagger : MonoBehaviour
{
    public static Dictionary<string, List<Transform>> tags = new Dictionary<string, List<Transform>>();
    public string tag;
    public int uniqueCharacterNumber = 0;

    void OnAwake() => addTag();
    void OnDestroy() => tags[tag].Remove(transform);

    private void addTag()
    {
        tag = uniqueCharacterNumber + "_" + tag;

        if (tags.ContainsKey(tag) && tags[tag].Count >= 4)
        {
            uniqueCharacterNumber++;
            addTag();
            return;
        }

        else
            tags[tag] = new List<Transform>();

        tags[tag].Add(transform);
    }

}