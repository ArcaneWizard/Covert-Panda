using System.Collections.Generic;

using MEC;

using UnityEngine;

public class AI_LookAround : CentralLookAround
{
    private Side side;

    public GameObject EnemySpotted { get; private set; }

    // random value for setting up perlin noise
    private float random;

    // whether or not the creature looks backwards 
    private bool lookBackwards;

    // the most recent direction the creature headed in when it was moving (left = -1, right = 1)
    private int lastKnownDirX;

    protected override void Awake()
    {
        base.Awake();
        side = transform.parent.GetComponent<Role>().Side;

        resetUponSpawning();
        deathSequence.RightBeforeRespawning += resetUponSpawning;

        Timing.RunSafeCoroutine(scanForNearbyEnemies(), gameObject);
        Timing.RunSafeCoroutine(sometimesLookBackwards(), gameObject);
    }

    protected override void figureOutDirectionToLookIn()
    {
        if (EnemySpotted) {
            DirectionToLook = EnemySpotted.transform.position - weaponPivot.position;
            lookBackwards = false;
        } else if (controller.IsGrounded && controller.IsTouchingMap) {
            // favor looking up more than looking down
            DirectionToLook = new Vector2(
                Mathf.PerlinNoise(Time.time / 2f, random / 2f) * 2f - 1f,
                Mathf.PerlinNoise(Time.time / 2f, random) * 2f - 0.96f
            );

            // favor looking to the side more than looking up/down
            if (Mathf.Abs(DirectionToLook.x) < 0.45f)
                DirectionToLook = new Vector2(Mathf.Sign(DirectionToLook.x) * 0.45f, DirectionToLook.y);

            // decide whether AI looks left/right depending on the direction they are moving in
            int dirX = (controller.DirX != 0) ? controller.DirX : lastKnownDirX;
            int sign = dirX * (lookBackwards ? -1 : 1);
            DirectionToLook = new Vector2(sign * Mathf.Abs(DirectionToLook.x), DirectionToLook.y);
        }

        DirectionToLook = DirectionToLook.normalized;

        if (controller.DirX != 0)
            lastKnownDirX = controller.DirX;
    }

    private void resetUponSpawning()
    {
        random = Random.Range(0f, 10f);
        lookBackwards = false;
        lastKnownDirX = -1 + Random.Range(0, 2) * 2; // either -1 or 1
    }

    // updates the EnemySpotted gameobject if an enemy creature is spotted in the AI's vision
    private IEnumerator<float> scanForNearbyEnemies()
    {
        yield return Timing.WaitForSeconds(0.3f);
        EnemySpotted = null;

        Collider2D[] enemiesWithinRangeOfWeapon = Physics2D.OverlapCircleAll(
                transform.position,
                weaponSystem.CurrentWeaponConfiguration.Range,
                LayerMasks.Target(side)
        );

        foreach (Collider2D enemy in enemiesWithinRangeOfWeapon) {
            // get a potential enemy creature
            GameObject nearbyEnemy = enemy.transform.parent.gameObject;

            // check if there's a barrier in btwn the nearby enemy and this creature's weapon
            RaycastHit2D hit = Physics2D.Raycast(
                weaponPivot.position,
                nearbyEnemy.transform.position - weaponPivot.position,
                weaponSystem.CurrentWeaponConfiguration.Range,
                LayerMasks.MapOrTarget(side)
            );

            // if the enemy creature is in this creature's line of sight, switch focus to it if it's closer than other enemies
            if (hit.collider != null && hit.collider.gameObject.layer == Layer.GetHitBoxOfOpposingSide(side)) {
                if (EnemySpotted == null || MathX.GetSquaredDistance(EnemySpotted.transform.position, weaponPivot.position)
                    > MathX.GetSquaredDistance(nearbyEnemy.transform.position, weaponPivot.position))
                    EnemySpotted = nearbyEnemy;
            }
        }

        Timing.RunSafeCoroutine(scanForNearbyEnemies(), gameObject);
    }

    // Decide whether the creature should look backwards (relative to movement) or not
    private IEnumerator<float> sometimesLookBackwards()
    {
        yield return Timing.WaitForSeconds(Random.Range(3f, 5f));

        // 10% chance the creature looks backwards when an enemy isn't spotted
        if (!EnemySpotted && Random.Range(0, 100) < 10) {
            lookBackwards = true;
            yield return Timing.WaitForSeconds(Random.Range(2f, 6f));
        }

        lookBackwards = false;
        StartCoroutine(sometimesLookBackwards());
    }
}
