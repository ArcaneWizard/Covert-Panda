using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experiment : MonoBehaviour
{
    public int Health { get; private set; }
     
    public void Bob()
    {
        Experiment a = new Experiment();
        a.Health = 2;
    }
}
