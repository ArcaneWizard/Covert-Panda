using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RequiredColliders", menuName = "Test/RequiredColliders")]
public class RequiredColliders : ScriptableObject
{
    public List<LimbTypes> limbs;

    public List<Collider2D> colliders;
}
