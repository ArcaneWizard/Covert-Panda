using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class houses the different arms the creature has, one of which is enabled
// depending on which type of weapon the creature is holding

public class WeaponSpecificArm : MonoBehaviour
{
    public List<GameObject> Hands; // Refactor note: should't have a seperate arm just for grenades (since they can be thrown while holding a weapon)
    public List<GameObject> Short_barrel;
    public List<GameObject> Middle_barrel;
    public List<GameObject> Long_barrel;
    public List<GameObject> Meelee_grip;
    public List<GameObject> Pistol_grip;
    public List<GameObject> Shoulder_rest;

    private AimTargets Targets;

    public Transform GetIK_WeaponAimTracker(List<GameObject> arm)
    {
        if (!Targets)
            Targets = transform.GetComponent<AimTargets>();

        if (arm == Short_barrel)
            return Targets.ShortBarrelAim;
        else if (arm == Middle_barrel)
            return Targets.MediumBarrelAim;
        else if (arm == Long_barrel)
            return Targets.LongBarrelAim;
        else if (arm == Meelee_grip)
            return Targets.MeeleePoleAim;
        else if (arm == Pistol_grip)
            return Targets.PistolGripAim;
        else if (arm == Shoulder_rest)
            return Targets.ShoulderRestAim;

        return null;
    }

    public List<Vector2> GetIK_WeaponCoordinates(List<GameObject> arm)
    {
        if (!Targets)
            Targets = transform.GetComponent<AimTargets>();

        if (arm == Short_barrel)
            return IKAimingCoordinates.ShortBarrelCoordinates;
        else if (arm == Meelee_grip)
            return IKAimingCoordinates.MeeleeGripCoordinates;
        else if (arm == Pistol_grip)
            return IKAimingCoordinates.PistolGripCoordinates;
        else if (arm == Shoulder_rest)
            return IKAimingCoordinates.ShoulderRestCoordinates;

        return IKAimingCoordinates.DefaultCoordinates;
    }
}

