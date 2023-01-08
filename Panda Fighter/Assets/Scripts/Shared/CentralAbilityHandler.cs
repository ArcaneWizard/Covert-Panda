using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Handles the abilities of the creature (ex. invisiblity, spawn protection, etc.)
// NOTE: needs to be refactored with the use of an interface named Ability for defining each ability
// This will improve scalability when tons of complex abilities exist in the future

public class CentralAbilityHandler : MonoBehaviour {

    // whether or not the creature is invulnerable to damage
    public bool IsInvulnerable { get; private set; }

    // whether ot not the creature looks invisible (cannot be detected/shot at by others)
    public bool IsInvisible { get; private set; }

    private bool hasSpawnProtection;
    private float spawnProtectionDuration = 1.7f;

    private bool canTurnInvisible;
    private float invisibilityDuration = 3f;
    private float invisibilityCooldown = 15f;

    private Collider2D hitBox;
    private Health health;
    private Image effectIndicator;

    void Awake()
    {
        IsInvisible = false;
        canTurnInvisible = true;

        hitBox = transform.GetChild(1).GetComponent<Collider2D>();
        health = transform.GetComponent<Health>();
        effectIndicator = transform.parent.GetChild(2).GetChild(0).GetComponent<Image>();

        StartCoroutine(EnableSpawnProtection());
    }

    void Update()
    {
        if (health.isDead)
            return;

        if (Input.GetKey(KeyCode.T) && canTurnInvisible && transform.parent.tag == "Player")
            StartCoroutine(turnInvisible());

        IsInvulnerable = (IsInvisible || hasSpawnProtection);

        hitBox.enabled = !IsInvulnerable;
        effectIndicator.color = hasSpawnProtection ? new Color32(0, 255, 227, 255) : new Color32(0, 0, 0, 255);
    }

    public IEnumerator EnableSpawnProtection()
    {
        hasSpawnProtection = true;
        yield return new WaitForSeconds(spawnProtectionDuration);
        hasSpawnProtection = false;
    }

    private IEnumerator turnInvisible()
    {
        IsInvisible = true;
        canTurnInvisible = false;

        yield return new WaitForSeconds(invisibilityDuration);
        IsInvisible = false;

        yield return new WaitForSeconds(invisibilityCooldown);
        canTurnInvisible = true;
    }
}
