using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideCollider : MonoBehaviour
{
    public Sideview_Controller sideview_Controller;

    void Awake()
    {
        if (gameObject.name != "Forward" && gameObject.name != "Backward")
            Debug.LogError("Don't change this gameObject's name as it is used to identify it in the sideview_controller script");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 11)
            sideview_Controller.nextToWall = gameObject.name;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == 11)
            sideview_Controller.nextToWall = gameObject.name;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 11)
            sideview_Controller.nextToWall = "";
    }
}
