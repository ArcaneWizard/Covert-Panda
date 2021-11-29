using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDir : MonoBehaviour
{

    public Vector3 contactPoint;


    private void OnCollisionStay2D(Collision2D col)
    {
        contactPoint = col.GetContact(0).point;
    }
}
