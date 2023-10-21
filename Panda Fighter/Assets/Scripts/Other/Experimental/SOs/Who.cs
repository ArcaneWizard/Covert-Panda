using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Who", menuName = "Test/Who")]
public class Who : ScriptableObject
{
    public RequiredSprites sprites;
    public RequiredColliders colliders;
}
