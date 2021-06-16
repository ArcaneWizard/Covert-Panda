using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTilt : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float angle = transform.eulerAngles.z % 180;
        Debug.Log(angle < Constants.maxPlatformTilt || angle > 180 - Constants.maxPlatformTilt);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
