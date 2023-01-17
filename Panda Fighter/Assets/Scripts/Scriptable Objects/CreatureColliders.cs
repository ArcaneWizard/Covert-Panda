using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Limb Colliders", menuName = "Creature Collider")]
public class CreatureColliders : ScriptableObject
{
    public Vector2[] Chest;
    public Vector2[] Head;
    public Vector2[] FrontUpperArm;
    public Vector2[] FrontLowerArm;
    public Vector2[] FrontHand;
    public Vector2[] BackUpperArm;
    public Vector2[] BackLowerArm;
    public Vector2[] BackHand;
    public Vector2[] BackThigh;
    public Vector2[] BackLeg;
    public Vector2[] BackFoot;
    public Vector2[] FrontThigh;
    public Vector2[] FrontLeg;
    public Vector2[] FrontFoot;
}