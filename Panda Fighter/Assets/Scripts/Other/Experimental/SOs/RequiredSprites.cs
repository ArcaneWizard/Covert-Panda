using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RequiredSprites", menuName = "Test/RequiredSprites")]
public class RequiredSprites : ScriptableObject
{
    public List<LimbTypes> limbs;

    public List<Collider2D> sprites;
}
