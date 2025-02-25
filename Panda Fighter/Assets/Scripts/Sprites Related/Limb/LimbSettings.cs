using System.Collections.Generic;

using UnityEngine;

/* Can return any sprite or Polygon2D collider points associated with a specified limb of
 * a creature. Ex. retrieve the front shoulder sprite + collider points/shape of an Amphelot creature. */

public class LimbSettings : MonoBehaviour
{
    private Creatures sprite;
    private new CreatureColliders collider;

    private Dictionary<LimbTypes, Sprite> sprites = new Dictionary<LimbTypes, Sprite>();
    private Dictionary<LimbTypes, Vector2[]> colliders = new Dictionary<LimbTypes, Vector2[]>();

    // Initialize storing the limb sprites and limb collider coordinates for this creature
    private void initialize()
    {
        sprite = transform.parent.parent.GetComponent<Role>().Sprites;
        collider = transform.parent.parent.GetComponent<Role>().Colliders;

        // Quick error check
#if (UNITY_EDITOR)
        if (!sprite || !collider)
            Debug.LogError("This creature is missing some specified limb Sprites or collider coordinates");

        else if (sprite.RightLeg == null || collider.FrontUpperArm == null || collider.FrontUpperArm.Length == 0) {
            Debug.LogError("This creature is missing some specified limb Sprites or collider coordinates");
        }
#endif

        sprites.Clear();
        colliders.Clear();

        sprites.Add(LimbTypes.Chest, sprite.Chest);
        sprites.Add(LimbTypes.Head, sprite.Head);
        sprites.Add(LimbTypes.FrontThigh, sprite.LeftThigh);
        sprites.Add(LimbTypes.BackThigh, sprite.RightThigh);
        sprites.Add(LimbTypes.FrontFoot, sprite.LeftFoot);
        sprites.Add(LimbTypes.BackFoot, sprite.RightFoot);
        sprites.Add(LimbTypes.FrontLeg, sprite.LeftLeg);
        sprites.Add(LimbTypes.BackLeg, sprite.RightLeg);
        sprites.Add(LimbTypes.FrontLowerArm, sprite.MainArm);
        sprites.Add(LimbTypes.BackLowerArm, sprite.BackArm);
        sprites.Add(LimbTypes.FrontUpperArm, sprite.MainArmPad);
        sprites.Add(LimbTypes.BackUpperArm, sprite.BackArmPad);
        sprites.Add(LimbTypes.FrontHand, sprite.MainHand);
        sprites.Add(LimbTypes.BackHand, sprite.BackHand);

        colliders.Add(LimbTypes.Chest, collider.Chest);
        colliders.Add(LimbTypes.Head, collider.Head);
        colliders.Add(LimbTypes.FrontThigh, collider.FrontThigh);
        colliders.Add(LimbTypes.BackThigh, collider.BackThigh);
        colliders.Add(LimbTypes.FrontFoot, collider.FrontFoot);
        colliders.Add(LimbTypes.BackFoot, collider.BackFoot);
        colliders.Add(LimbTypes.FrontLeg, collider.FrontLeg);
        colliders.Add(LimbTypes.BackLeg, collider.BackLeg);
        colliders.Add(LimbTypes.FrontLowerArm, collider.FrontLowerArm);
        colliders.Add(LimbTypes.BackLowerArm, collider.BackLowerArm);
        colliders.Add(LimbTypes.FrontUpperArm, collider.FrontUpperArm);
        colliders.Add(LimbTypes.BackUpperArm, collider.BackUpperArm);
        colliders.Add(LimbTypes.FrontHand, collider.FrontHand);
        colliders.Add(LimbTypes.BackHand, collider.BackHand);
    }

    // Return a limb based off limb type
    public Sprite ReturnLimb(LimbTypes limbType, bool forceRefresh)
    {
        if (forceRefresh || sprites.Count == 0)
            initialize();

        return sprites[limbType];
    }

    // Return a limb's Polygon2D collider points based off limb type
    public Vector2[] ReturnCollider(LimbTypes limbType, bool forceRefresh)
    {
        if (forceRefresh || colliders.Count == 0)
            initialize();

        return colliders[limbType];
    }
}

public enum LimbTypes
{
    Chest,
    FrontUpperArm,
    FrontLowerArm,
    FrontHand,
    BackUpperArm,
    BackLowerArm,
    BackHand,
    FrontThigh,
    FrontLeg,
    FrontFoot,
    BackThigh,
    BackLeg,
    BackFoot,
    Head
}