using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreatureType : MonoBehaviour
{
    public Creature creature;
    public CreatureLimbCollections creatureLimbCollections;


    void Awake() => updateCreature();

#if (UNITY_EDITOR)
    void OnValidate()
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
            updateCreature();
    }
#endif

    private void updateCreature()
    {

    }
}

public enum Creature
{
    Amphibow,
    Reptidon,
    Caellimander,
    Angelfish,
    FuturisticMeerkat
}


