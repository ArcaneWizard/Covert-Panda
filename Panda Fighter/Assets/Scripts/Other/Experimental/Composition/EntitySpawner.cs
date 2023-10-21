using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntitySpawner : MonoBehaviour {

    public enum Creature
    {
        Bob
    }

    public bool isPlayer;

    private void Start()
    {
        Creature[] creatures = (Creature[])Enum.GetValues(typeof(Creature));
        Character[] characters = (Character[])Enum.GetValues(typeof(Character));

        var creatureToLimb = new Dictionary<Creature, int>();
        foreach (Creature c in creatures) {

        }
     }
    /*
     * 15 options in dict -> up to 12 -> up to 2
(creature)    (character)    (AI)

//15
foreach (Creatu
creature_dict -> map creature to limb structure

 

skin dict-> map Amphelot, Reptidon and Caellimander to 
  skin2 dict -> map names to skin/colliders
  key DNE -> default skin/collider
key DNE -> (skin3 dict: map creatures to skin/colliders)


// 15 + 12 = 27 worst case. on average, ~15 
if (Amphelot)
   if (Akabe): skin/colliders
   if (Raivou): skin/colliders
   if (You): skin/colliders
if (Reptidon)
   if (Raivou): skin/colliders
if (Caellimander):
   if (Akabe): skin/colliders
   if (Raivou): skin/colliders
   if (You): skin/colliders

for rest of creatures:
if (character): skin/colliders

// 2
if (Akabe && Amphelot)
   use Akabe's backpack
else if (Amphelot)
   use default backpack

// 16
if (player)
   depending on creature type, behaviour: ...

if (Amphelot)
   if (Akabe): AkabeBehaviour
   if (Raivou): RaivourBehaviuor
if (Reptidon)
   if (Raivou): ReptidonBehaviour
if (Caellimander):
   if (Akabe): AkabeBehaviour
   if (Raivou): RaivourBehaviuor

for rest of creature types if behaviour still not found:

if (creature type): behaviour

//27
if (Reptidon):
   if (raivou):
   if (gargan):
if (Amphelot)
   if (...):
if (Caellimander):
   if

if (other creature type)
   return size/colliders + sizes

//27
if (Reptidon):
   if (raivou): weapons
   if (gargan): weapons
if (Amphelot)
   if (...): weapons
if (Caellimander):
   if (..): weapons

if (other creature type)
   weapons




if (Akabe && Amphelot)
   use Akabe's backpack
else if (Amphelot)
   use default backpack

but u want all dogs to call a diff modified bark
// 
     * 
     */
}
