using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limbs : MonoBehaviour
{
    public List<GameObject> Hands;
    public List<GameObject> Short_barrel;
    public List<GameObject> Middle_barrel;
    public List<GameObject> Long_barrel;
    public List<GameObject> Meelee_grip;
    public List<GameObject> Pistol_grip;
    public List<GameObject> Shoulder_rest;

    private AimTargets Targets;

    public Transform GetIK_WeaponAimTracker(List<GameObject> limb)
    {
        if (!Targets)
            Targets = transform.GetComponent<AimTargets>();

        if (limb == Short_barrel)
            return Targets.ShortBarrelAim;
        else if (limb == Middle_barrel)
            return Targets.MediumBarrelAim;
        else if (limb == Long_barrel)
            return Targets.LongBarrelAim;
        else if (limb == Meelee_grip)
            return Targets.MeeleePoleAim;
        else if (limb == Pistol_grip)
            return Targets.PistolGripAim;
        else if (limb == Shoulder_rest)
            return Targets.ShoulderRestAim;

        return null;
    }

    public List<Vector2> GetIK_WeaponCoordinates(List<GameObject> limb)
    {
        if (!Targets)
            Targets = transform.GetComponent<AimTargets>();

        if (limb == Short_barrel)
            return IKCoordinatesForAiming.ShortBarrelCoordinates;
        else if (limb == Meelee_grip)
            return IKCoordinatesForAiming.MeeleeGripCoordinates;
        else if (limb == Pistol_grip)
            return IKCoordinatesForAiming.PistolGripCoordinates;
        else if (limb == Shoulder_rest)
            return IKCoordinatesForAiming.ShoulderRestCoordinates;

        return IKCoordinatesForAiming.DefaultCoordinates;
    }
}

