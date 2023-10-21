using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using static UnityEngine.InputManagerEntry;

public class PhysicalWeapons : MonoBehaviour
{
    public GameObject GrenadeHands; // to be removed
    public GameObject LavaOrbLauncher;
    public GameObject PlasmaOrbLauncher;
    public GameObject Shielder;
    public GameObject Sniper;
    public GameObject LeafScythe;
    public GameObject GoldenShotgun;
    public GameObject ArcticCannon;
    public GameObject Needler;
    public GameObject FocusBeamer;
    public GameObject RocketLauncher;
    public GameObject ArcticSprayer;

    private void Awake()
    {
        /*this.RequireTags(
            new TagRequirement(GrenadeHands, nameof(GrenadeHands)),
            new TagRequirement(LavaOrbLauncher, nameof(LavaOrbLauncher)),
            new TagRequirement(Shielder, nameof(Shielder)),
            new TagRequirement(Sniper, nameof(Sniper)),
            new TagRequirement(LeafScythe, nameof(LeafScythe)),
            new TagRequirement(GoldenShotgun, nameof(GoldenShotgun)),
            new TagRequirement(ArcticCannon, nameof(ArcticCannon)),
            new TagRequirement(Needler, nameof(Needler)),
            new TagRequirement(FocusBeamer, nameof(FocusBeamer)),
            new TagRequirement(RocketLauncher, nameof(RocketLauncher)),
            new TagRequirement(ArcticSprayer, nameof(ArcticSprayer))
        );*/
    }
}

//combat Modes
public enum CombatType
{
    Gun,
    Meelee,
    Handheld
}

//gun types
public enum FiringMode
{
    SingleFire,
    SpamFire,
    ContinousBeam,
    ChargeUpFire
}

