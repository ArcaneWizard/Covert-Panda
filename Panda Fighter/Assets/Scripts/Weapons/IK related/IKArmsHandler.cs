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
    private Dictionary<GameObject, Transform> targetForMainArm;
    private Dictionary<GameObject, Transform> targetForOtherArm;

    // Return the IK target for a specified arm. Returns null if IK target doesn't
    // exist for that arm
    public Transform GetIKTarget(GameObject arm, bool forMainArm)
    {
        if (!targets)
            targets = transform.GetComponent<IKTargets>();

        if (targetForMainArm == null) {
            targetForMainArm = new Dictionary<GameObject, Transform>();
            targetForMainArm[ShortBarrel] = targets.ShortBarrelMainAim;
            targetForMainArm[MiddleBarrel] = targets.MediumBarrelAim;
            targetForMainArm[LongBarrel] = targets.LongBarrelAim;
            targetForMainArm[MeeleGrip] = targets.MeeleePoleAim;
            targetForMainArm[PistolGrip] = targets.PistolGripAim;
            targetForMainArm[ShoulderRest] = targets.ShoulderRestAim;
        }

        if (targetForOtherArm == null) {
            targetForOtherArm = new Dictionary<GameObject, Transform>();
            targetForOtherArm[ShortBarrel] = targets.ShortBarrelOtherAim;
        }

        if (forMainArm) {
            return targetForMainArm.TryGetValue(arm, out Transform target) ? target : null;
        } else {
            return targetForOtherArm.TryGetValue(arm, out Transform target) ? target : null;
        }
    }

    // Return the IK Coordinates for a specified arm. Returns null if IK coordinates
    // don't exist for that arm.
    public List<Vector2> GetIKCoordinates(GameObject arms, bool getMainArm)
    {
        if (!targets)
            targets = transform.GetComponent<IKTargets>();

        if (getMainArm) {
            if (arms == ShortBarrel)
                return IKAimingCoordinates.ShortBarrelMainArmCoordinates;
            else if (arms == MeeleGrip)
                return IKAimingCoordinates.MeeleeGripCoordinates;
            else if (arms == PistolGrip)
                return IKAimingCoordinates.PistolGripCoordinates;
            else if (arms == ShoulderRest)
                return IKAimingCoordinates.ShoulderRestCoordinates;
        } else {
            if (arms == ShortBarrel)
                return IKAimingCoordinates.ShortBarrelOtherArmCoordinates;
        }

        return IKAimingCoordinates.DefaultCoordinates;
    }

    private Dictionary<GameObject, Vector3> initialPositionOfArm;
    public Vector2 ArmTranslationSpeed = new Vector2(0.15f, 0.12f);

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
        initialPositionOfArm = new Dictionary<GameObject, Vector3>();

        initialPositionOfArm[ShortBarrel] = ShortBarrel.transform.localPosition;
        initialPositionOfArm[MiddleBarrel] = MiddleBarrel.transform.localPosition;
        initialPositionOfArm[LongBarrel] = LongBarrel.transform.localPosition;
        initialPositionOfArm[MeeleGrip] = MeeleGrip.transform.localPosition;
        initialPositionOfArm[PistolGrip] = PistolGrip.transform.localPosition;
        initialPositionOfArm[ShoulderRest] = ShoulderRest.transform.localPosition;
    }

    void Update()
    {
        // To prevent arms flipping upside down, limbs should always have 0 local rotation around x and y axes
        for (int i = 0; i < childCount; i++) {
            Vector3 localAngle = transform.GetChild(i).localEulerAngles;
            transform.GetChild(i).localEulerAngles = new Vector3(0, 0, localAngle.z);
        }

        // Arms should adjust their position a little when the creature is aiming a gun towards the ground
        // This is because the arms don't always look right when the arm pad solely rotates around a single pivot point
        Vector3 offset = Vector3.zero;
        if (lookAround.DirectionToLook.y < 0) {
            offset = new Vector3(ArmTranslationSpeed.x * Mathf.Abs(lookAround.DirectionToLook.y),
                ArmTranslationSpeed.y * Mathf.Abs(lookAround.DirectionToLook.y), 0);
        }

        GameObject currentArm = weaponSystem.CurrentWeaponConfiguration.Arms;
        currentArm.transform.localPosition = initialPositionOfArm[currentArm] + offset;
    }
}

