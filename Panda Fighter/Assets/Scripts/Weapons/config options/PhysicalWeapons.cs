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
public class WeaponTypes
{
    public const string gun = "gun";
    public const string meelee = "meelee";
    public const string handheld = "handheld";
}

//gun types
public class FiringModes
{
    public const string singleFire = "single fire";
    public const string spamFire = "spam fire";
    public const string holdFire = "hold fire";
    public const string chargeUpFire = "charge up fire";
}

