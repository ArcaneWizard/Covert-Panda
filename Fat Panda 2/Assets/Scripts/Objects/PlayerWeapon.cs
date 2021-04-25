using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    private string weapon;

    void Awake()
    {
        weapon = gameObject.tag;
    }
}
