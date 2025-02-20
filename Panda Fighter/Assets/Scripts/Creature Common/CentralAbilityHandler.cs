using System.Collections;

using UnityEngine;
using UnityEngine.UI;

// Handles the abilities of the creature (ex. invisiblity, spawn protection, etc.)
// NOTE: needs to be refactored with the use of an interface named Ability for defining each ability
// This will improve scalability when tons of complex abilities exist in the future

public class CentralAbilityHandler : ICentralAbilityHandler
{
    private bool hasSpawnProtection;
    private float spawnProtectionDuration = 1.7f;

    private bool canTurnInvisible;
    private float invisibilityDuration = 3f;
    private float invisibilityCooldown = 15f;

    private Collider2D hitBox;
    private Health health;
    private Image effectIndicator;

    public IEnumerator EnableSpawnProtection()
    {
        hasSpawnProtection = true;
        yield return new WaitForSeconds(spawnProtectionDuration);
        hasSpawnProtection = false;
    }


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
        if (health.IsDead)
            return;

        if (Input.GetKey(KeyCode.T) && canTurnInvisible && transform.parent.CompareTag("Player"))
            StartCoroutine(turnInvisible());

        IsInvulnerable = (IsInvisible || hasSpawnProtection);

        hitBox.enabled = !IsInvulnerable;
        effectIndicator.color = hasSpawnProtection ? new Color32(0, 255, 227, 255) : new Color32(0, 0, 0, 255);
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
