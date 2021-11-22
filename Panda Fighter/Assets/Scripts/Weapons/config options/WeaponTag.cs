using UnityEngine;
using System.Collections;
using System.Text;

public class WeaponTag : MonoBehaviour {

    public WeaponTags tag;

    public string Tag => tag.ToString();
    
    void OnValidate() 
    {
        StringBuilder newTag = new StringBuilder("");
        foreach (char c in tag.ToString()) {
            newTag.Append(c >= 'A' && c <= 'Z' ? " " + c : c.ToString());
        }

        gameObject.name = newTag.ToString();
    }
}