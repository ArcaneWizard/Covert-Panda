using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RespawningText : MonoBehaviour
{

    private Text text;

    private void Awake() => text = transform.GetComponent<Text>();

    public void StartRespawnCountdown(float respawnTime) => StartCoroutine(eStartRespawnCountdown(respawnTime));

    private IEnumerator eStartRespawnCountdown(float respawnTime)
    {
        text.text = "";
        text.enabled = true;

        while (respawnTime >= 1)
        {
            text.text = "Respawning in " + ((int)Mathf.Floor(respawnTime)).ToString();
            yield return new WaitForSeconds(1);
            respawnTime--;
        }

        text.text = "Respawning in " + ((int)Mathf.Floor(respawnTime)).ToString();

        if (respawnTime > 0)
            yield return new WaitForSeconds(respawnTime);

        text.enabled = false;
    }

}