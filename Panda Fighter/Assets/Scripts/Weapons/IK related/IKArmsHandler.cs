using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class houses the different arms the creature has to hold weapons. It configures the inverse
// kinematic settings to rotate the arm + bend the elbow as the weapon is aimed in different directions

public class IKArmsHandler : MonoBehaviour
{
    public GameObject ShortBarrel;
    public GameObject MiddleBarrel;
    public GameObject LongBarrel;
    public GameObject MeeleGrip;
    public GameObject PistolGrip;
    public GameObject ShoulderRest;

    private IKTargets targets;
    private Dictionary<GameObject, Transform> TargetForMainArm;
    private Dictionary<GameObject, Transform> TargetForOtherArm;

    // Return the IK target for a specified arm. Returns null if IK target doesn't
    // exist for that arm
    public Transform GetIKTarget(GameObject arm, bool forMainArm)
    {
        if (!targets)
            targets = transform.GetComponent<IKTargets>();

        if (TargetForMainArm == null)
        {
            TargetForMainArm = new Dictionary<GameObject, Transform>();
            TargetForMainArm[ShortBarrel] = targets.ShortBarrelMainAim;
            TargetForMainArm[MiddleBarrel] = targets.MediumBarrelAim;
            TargetForMainArm[LongBarrel] = targets.LongBarrelAim;
            TargetForMainArm[MeeleGrip] = targets.MeeleePoleAim;
            TargetForMainArm[PistolGrip] = targets.PistolGripAim;
            TargetForMainArm[ShoulderRest] = targets.ShoulderRestAim;
        }

        if (TargetForOtherArm == null)
        {
            TargetForOtherArm = new Dictionary<GameObject, Transform>();
            TargetForOtherArm[ShortBarrel] = targets.ShortBarrelOtherAim;
        }

        if (forMainArm)
        {
            if (TargetForMainArm.TryGetValue(arm, out Transform target))
                return target;
            else
                return null;
        }
        else
        {
            if (TargetForOtherArm.TryGetValue(arm, out Transform target))
                return target;
            else
                return null;
        }
    }

    // Return the IK Coordinates for a specified arm. Returns null if IK coordinates
    // don't exist for that arm.
    public List<Vector2> GetIKCoordinates(GameObject arms, bool getMainArm)
    {
        if (!targets)
            targets = transform.GetComponent<IKTargets>();

        if (getMainArm)
        {
            if (arms == ShortBarrel)
                return IKAimingCoordinates.ShortBarrelMainArmCoordinates;
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
                return IKAimingCoordinates.ShortBarrelOtherArmCoordinates;
        }

        return IKAimingCoordinates.DefaultCoordinates;
    }


    private Dictionary<GameObject, Vector3> InitialPositionOfArm;
    private const float armTranslationSpeed = 0.12f;

    private int childCount;
    private CentralLookAround lookAround;
    private CentralWeaponSystem weaponSystem;

    void Awake()
    {
        childCount = transform.childCount;
        lookAround = transform.parent.parent.GetComponent<CentralLookAround>();
        weaponSystem = transform.parent.parent.GetComponent<CentralWeaponSystem>();
    }

    void Start()
    {
        InitialPositionOfArm = new Dictionary<GameObject, Vector3>();

        InitialPositionOfArm[ShortBarrel] = ShortBarrel.transform.localPosition;
        InitialPositionOfArm[MiddleBarrel] = MiddleBarrel.transform.localPosition;
        InitialPositionOfArm[LongBarrel] = LongBarrel.transform.localPosition;
        InitialPositionOfArm[MeeleGrip] = MeeleGrip.transform.localPosition;
        InitialPositionOfArm[PistolGrip] = PistolGrip.transform.localPosition;
        InitialPositionOfArm[ShoulderRest] = ShoulderRest.transform.localPosition;
    }

    void Update()
    {
        // To prevent arms flipping upside down, limbs should always have 0 local rotation around x and y axes
        for (int i = 0; i < childCount; i++) 
        {
            Vector3 localAngle = transform.GetChild(i).localEulerAngles;
            transform.GetChild(i).localEulerAngles = new Vector3(0, 0, localAngle.z);
        }

        // Arm pads should move forward when aiming down. Otherwise creature arms don't look right when
        // the arm pad always rotates around a single pivot point
        float xOffset = 0f;
        if (lookAround.directionToLook.y < 0)
            xOffset += armTranslationSpeed * Mathf.Abs(lookAround.directionToLook.y);

        GameObject currentArm = weaponSystem.CurrentWeaponConfiguration.Arms;
        currentArm.transform.localPosition = InitialPositionOfArm[currentArm] + new Vector3(xOffset, 0, 0);
    }
}

