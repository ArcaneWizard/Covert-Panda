using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CentralAbilityHandler : MonoBehaviour
{
    public bool isInvulnerable { get; private set; }

    private bool spawnProtectionEnabled;
    private float spawnProtectionDuration = 1.7f;

    public bool invisibilityEnabled { get; private set; }
    private bool canTurnInvisible;
    private float invisibleDuration = 3f;
    private float invisibleReloadTime = 15f;

    private Collider2D hitBox;
    private Health health;
    private Image effectIndicator;

    void Awake()
    {
        invisibilityEnabled = false;
        canTurnInvisible = true;

        hitBox = transform.GetChild(1).GetComponent<Collider2D>();
        health = transform.GetComponent<Health>();
        effectIndicator = transform.parent.GetChild(2).GetChild(0).GetComponent<Image>();
    }

    void Update()
    {
        if (health.isDead)
            return;

        if (Input.GetKey(KeyCode.T) && canTurnInvisible && transform.parent.tag == "Player")
            StartCoroutine(turnInvisible());

        isInvulnerable = (invisibilityEnabled || spawnProtectionEnabled);

        hitBox.enabled = !isInvulnerable;
        effectIndicator.color = spawnProtectionEnabled ? new Color32(0, 255, 227, 255) : new Color32(0, 0, 0, 255);
    }

    private IEnumerator turnInvisible()
    {
        invisibilityEnabled = true;
        canTurnInvisible = false;

        yield return new WaitForSeconds(invisibleDuration);
        invisibilityEnabled = false;

        yield return new WaitForSeconds(invisibleReloadTime);
        canTurnInvisible = true;
    }

    public IEnumerator spawnProtection()
    {
        spawnProtectionEnabled = true;
        yield return new WaitForSeconds(spawnProtectionDuration);
        spawnProtectionEnabled = false;
    }
}
