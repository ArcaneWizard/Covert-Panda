using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployShield : MonoBehaviour
{
    private GameObject shield;
    private Transform player; 

    void Awake()
    {
        shield = transform.GetChild(0).gameObject;
        player = transform.parent;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            toggleShield();

        //transform.position = player.position;
        transform.localEulerAngles = player.transform.GetChild(0).localEulerAngles;
    }

    private void toggleShield()
    {
        if (shield.activeSelf)
            shield.SetActive(false);
        else
            shield.SetActive(true);
    }

}
