using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralAbilityHandler : MonoBehaviour
{
    public bool isInvisible { private set; get; }
    private bool canTurnInvisible;

    private float invisibleDuration = 3f;
    private float invisibleReloadTime = 15f;

    void Awake()
    {
        isInvisible = false;
        canTurnInvisible = true;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.T) && canTurnInvisible)
            StartCoroutine(turnInvisible());
    }

    private IEnumerator turnInvisible()
    {
        isInvisible = true;
        canTurnInvisible = false;

        yield return new WaitForSeconds(invisibleDuration);
        isInvisible = false;

        yield return new WaitForSeconds(invisibleReloadTime);
        canTurnInvisible = true;
    }
}
