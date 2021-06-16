using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    private string weapon;

    void Awake()
    {
        weapon = gameObject.tag;
        transform.GetComponent<Rigidbody2D>().isKinematic = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision);
    }
}
