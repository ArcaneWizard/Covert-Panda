using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponConfig : MonoBehaviour
{
    public List<GameObject> limbs = new List<GameObject>();
    public Transform bulletSpawnPoint;
    public GameObject weapon;
    public Transform aimTarget;
}
