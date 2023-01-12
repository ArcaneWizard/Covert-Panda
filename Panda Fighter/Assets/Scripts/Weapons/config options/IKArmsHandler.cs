using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class houses the different arms the creature has to hold weapons. It configures the inverse
// kinematic settings to sync any arm's elbow/shoulder rotation with weapon aim

public class IKArmsHandler : MonoBehaviour
{
    public List<GameObject> Hands; // Refactor note: should't have a seperate arm just for grenades (since they can be thrown while holding a weapon)
    public List<GameObject> ShortBarrel;
    public List<GameObject> MiddleBarrel;
    public List<GameObject> LongBarrel;
    public List<GameObject> MeeleGrip;
    public List<GameObject> PistolGrip;
    public List<GameObject> ShoulderRest;

    private IKTargets Targets;

    // Return the IK target for a specified arm. Returns null if IK target doesn't
    // exist for that arm
    public Transform GetIKTarget(List<GameObject> arms, bool getMainArm)
    {
        if (!Targets)
            Targets = transform.GetComponent<IKTargets>();

        if (getMainArm)
        {
            if (arms == ShortBarrel)
                return Targets.ShortBarrelMainArm;
            else if (arms == MiddleBarrel)
                return Targets.MediumBarrelAim;
            else if (arms == LongBarrel)
                return Targets.LongBarrelAim;
            else if (arms == MeeleGrip)
                return Targets.MeeleePoleAim;
            else if (arms == PistolGrip)
                return Targets.PistolGripAim;
            else if (arms == ShoulderRest)
                return Targets.ShoulderRestAim;
        }

        else
        {
            if (arms == ShortBarrel)
                return Targets.ShortBarrelOtherArm;
        }

        return null;
    }

    // Return the IK Coordinates for a specified arm. Returns null if IK coordinates
    // don't exist for that arm.
    public List<Vector2> GetIKCoordinates(List<GameObject> arms, bool getMainArm)
    {
        if (!Targets)
            Targets = transform.GetComponent<IKTargets>();

        if (getMainArm)
        {
            if (arms == ShortBarrel)
                return IKAimingCoordinates.ShortBarrelMainArm;
            else if (arms == MeeleGrip)
                return IKAimingCoordinates.MeeleeGripCoordinates;
            else if (arms == PistolGrip)
                return IKAimingCoordinates.PistolGripCoordinates;
            else if (arms == ShoulderRest)
                return IKAimingCoordinates.ShoulderRestCoordinates;
        }
        else
        {
            if (arms == ShortBarrel)
                return IKAimingCoordinates.ShortBarrelOtherArm;
        }

        return IKAimingCoordinates.DefaultCoordinates;
    }
}

