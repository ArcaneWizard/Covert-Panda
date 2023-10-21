using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class References : MonoBehaviour
{
    public Camera Camera;
    public GameObject InventoryCanvas;
    public GameObject DeathCanvas;
    public RespawningText RespawnText;
    public GameObject Map;
    public GameObject Environment;
    public Transform FriendRespawnPoints;
    public Transform EnemyRespawnPoints;

    private static References instance;
    public static References Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake() =>  instance = this;
}
