using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ICentralAbilityHandler : MonoBehaviour
{
    // whether or not the creature is invulnerable to damage
    public bool IsInvulnerable { get; protected set; }

    // whether ot not the creature looks invisible (cannot be detected/shot at by others)
    public bool IsInvisible { get; protected set; }

}
