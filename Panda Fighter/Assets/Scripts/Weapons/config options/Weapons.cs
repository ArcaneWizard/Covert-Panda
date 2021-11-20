using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    public GameObject GrenadeHands;
    public GameObject LavaOrbLauncher;
    public GameObject PlasmaOrbLauncher;
    public GameObject Shielder;
    public GameObject Sniper;
    public GameObject LeafScythe;
    public GameObject GoldenShotgun;
    public GameObject ArcticCannon;
    public GameObject Needler;
}

//combat Modes
public class Mode
{
    public const string gun = "gun";
    public const string meelee = "meelee";
    public const string handheld = "handheld";
}

//gun types
public class Type
{
    public const string singleFire = "single fire";
    public const string spamFire = "spam fire";
    public const string holdFire = "hold fire";
}

