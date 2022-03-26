using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralAbilityHandler : MonoBehaviour
{
    public bool isInvisible { private set; get; }
    private bool canTurnInvisible;

    private Collider2D collider;

    private float invisibleDuration = 3f;
    private float invisibleReloadTime = 15f;

    void Awake()
    {
        isInvisible = false;
        canTurnInvisible = true;

        collider = transform.GetChild(1).GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.T) && canTurnInvisible && transform.parent.tag == "Player")
            StartCoroutine(turnInvisible());
    }

    private IEnumerator turnInvisible()
    {
        isInvisible = true;
        collider.enabled = false; 
        canTurnInvisible = false;

        yield return new WaitForSeconds(invisibleDuration);
        isInvisible = false;
        collider.enabled = true; 

        yield return new WaitForSeconds(invisibleReloadTime);
        canTurnInvisible = true;
    }
}
