using System.Collections.Generic;
using System.Text;

using UnityEngine;

public class Tag : MonoBehaviour
{
    public List<string> Tags = new List<string>();
    public bool AddName;

    void OnValidate()
    {
        if (AddName) {
            AddName = false;
            Tags.Add(gameObject.name);
        }

        // tags must be simple -> only letters and digits, no spaces
        var sb = new StringBuilder();
        for (int i = 0; i < Tags.Count; i++) {
            sb.Clear();

            foreach (char c in Tags[i]) {
                if (char.IsLetterOrDigit(c)) { sb.Append(c); }
            }

            Tags[i] = sb.ToString();
        }

        Tag[] tags = GetComponents<Tag>();
        if (tags.Length > 1)
            Debug.LogError($"{gameObject.name} has more than 1 tag script. Please remove duplicates");
    }
}
