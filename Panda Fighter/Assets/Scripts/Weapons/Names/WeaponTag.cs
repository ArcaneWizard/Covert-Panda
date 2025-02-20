using System.Text;

using UnityEngine;

public class WeaponTag : MonoBehaviour
{
    public Weapon Tag;

    void OnValidate()
    {
        StringBuilder newTag = new StringBuilder("");
        foreach (char c in Tag.ToString()) {
            newTag.Append(c >= 'A' && c <= 'Z' ? " " + c : c.ToString());
        }

        gameObject.name = newTag.ToString();
    }
}
