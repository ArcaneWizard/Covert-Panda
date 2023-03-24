using System.Collections;
using System.Collections.Generic;
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
        side = transform.parent.GetComponent<Role>().side;

        resetUponSpawning();
        deathSequence.RightBeforeRespawning += resetUponSpawning;

        StartCoroutine(scanForNearbyEnemies());
        StartCoroutine(sometimesLookBackwards());
    }

    private void resetUponSpawning()
    {
        random = Random.Range(0f, 10f);
        lookBackwards = false;
        lastKnownDirX = -1 + Random.Range(0, 2) * 2; // either -1 or 1
    }  
    
    // Sets the direction the AI should look in. If an enemy has been spotted, the direction to look in
    // is the vector from the AI's shoulder to said enemy. If no enemy is spotted, the AI looks around randomly
    // to give the appearance of scanning the environment 
    protected override void figureOutDirectionToLookIn() 
    {
        if (EnemySpotted)
        {
            directionToLook = EnemySpotted.transform.position - weaponPivot.position;
            lookBackwards = false;
        }

        else if (controller.isGrounded && controller.isTouchingMap)
        {
            // favor looking up more than looking down
            directionToLook = new Vector2(
                Mathf.PerlinNoise(Time.time / 2f, random / 2f) * 2f - 1f,
                Mathf.PerlinNoise(Time.time / 2f, random) * 2f - 0.96f
            );

            // favor looking to the side more than looking up/down
            if (Mathf.Abs(directionToLook.x) < 0.45f)
                directionToLook = new Vector2(Mathf.Sign(directionToLook.x) * 0.45f, directionToLook.y);

            // decide whether AI looks left/right depending on the direction they are moving in
            int dirX = (controller.DirX != 0) ? controller.DirX : lastKnownDirX;
            int sign = dirX * (lookBackwards ? -1 : 1);
            directionToLook = new Vector2(sign * Mathf.Abs(directionToLook.x), directionToLook.y);
        }

        directionToLook = directionToLook.normalized;

        if (controller.DirX != 0)
            lastKnownDirX = controller.DirX;
    }

    // updates the Enemy Spotted GameObject if an enemy creature is spotted in the AI's vision to shoot at
    private IEnumerator scanForNearbyEnemies()
    {
        yield return new WaitForSeconds(0.3f);
        EnemySpotted = null;

        Collider2D[] enemiesWithinRangeOfWeapon = Physics2D.OverlapCircleAll(
                transform.position, 
                weaponSystem.CurrentWeaponConfiguration.WeaponRange,
                LayerMasks.target(side)
        );

        foreach (Collider2D enemy in enemiesWithinRangeOfWeapon) 
        {
            // get a potential enemy creature
            GameObject nearbyEnemy = enemy.transform.parent.gameObject;
            
            // check if there's a barrier in btwn the nearby enemy and this creature's weapon
            RaycastHit2D hit = Physics2D.Raycast(
                weaponPivot.position, 
                nearbyEnemy.transform.position - weaponPivot.position,
                weaponSystem.CurrentWeaponConfiguration.WeaponRange, 
                LayerMasks.mapOrTarget(side)
            );

            // if the enemy creature is in this creature's line of sight, switch focus to it if it's closer than other enemies
            if (hit.collider != null && hit.collider.gameObject.layer == Layer.GetHitBoxOfOpposition(side)) 
            {
                if (EnemySpotted == null || sqrDistance(EnemySpotted.transform.position, weaponPivot.position) 
                    > sqrDistance(nearbyEnemy.transform.position, weaponPivot.position))
                    EnemySpotted = nearbyEnemy;
            }
        }

        StartCoroutine(scanForNearbyEnemies());
    }

    // Decide whether the creature should look backwards (relative to movement) or not
    private IEnumerator sometimesLookBackwards()
    {
        yield return new WaitForSeconds(Random.Range(3f, 5f));

        // 10% chance the creature looks backwards when an enemy isn't spotted
        if (!EnemySpotted && Random.Range(0, 100) < 10)
        {
            lookBackwards = true;
            yield return new WaitForSeconds(Random.Range(2f, 6f));
        }

        lookBackwards = false;
        StartCoroutine(sometimesLookBackwards());
    }

    private float sqrDistance(Vector2 a, Vector2 b) 
    {
        Vector2 diff = a-b;
        return diff.x * diff.x + diff.y * diff.y;
    }
}
