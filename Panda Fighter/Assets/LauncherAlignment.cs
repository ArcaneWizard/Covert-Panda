using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LauncherAlignment : MonoBehaviour
{
    public Transform gun;

    private void Update()
    {
        transform.rotation = gun.rotation;
    }
}
