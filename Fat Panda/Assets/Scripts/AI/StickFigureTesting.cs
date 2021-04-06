using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickFigureTesting : MonoBehaviour
{
    private Rigidbody2D rig; 
    public float speed = 2;

    void Awake() {
       rig = transform.GetComponent<Rigidbody2D>(); 
    }

    void Start()
    {
       rig.velocity = new Vector2(-1, 0) * speed;
    }
}
