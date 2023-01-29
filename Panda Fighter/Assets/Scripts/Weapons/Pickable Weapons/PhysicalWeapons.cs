using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

