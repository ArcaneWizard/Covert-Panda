using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_LookAround : CentralLookAround
{
    private AI_Shooting shootingAI;
    private Side side;  

    public GameObject EnemySpotted { get; private set; }
    private float randomizeLookingAround;

    protected override void Awake()
    {
        base.Awake();
        shootingAI = transform.GetComponent<AI_Shooting>();
        side = transform.parent.GetComponent<Role>().side;

        randomizeLookingAround = UnityEngine.Random.Range(0f, 10f);
        StartCoroutine(scanForNearbyEnemies());
    }

    public override bool facingRight() => directionToLook.x >= 0;

    protected override void figureOutDirectionToLookIn() 
    {
        // if an enemy has been spotted, the direction this AI looks is the vector from the weapon to the enemy 
        if (EnemySpotted) 
            directionToLook = EnemySpotted.transform.position - weaponPivot.position; 

        // otherwise the direction this AI looks varies randomly to give the appearance of scanning the environment 
        else if (controller.isGrounded && controller.isTouchingMap) 
        {
            // favor looking up slightly more than looking down
            directionToLook = new Vector2(
                Mathf.PerlinNoise(Time.time/2f, randomizeLookingAround/2f) * 2f - 1f, 
                Mathf.PerlinNoise(Time.time/2f, randomizeLookingAround) * 2f - 0.96f
            );

            // favor looking to the side more than looking up/down
            if (Mathf.Abs(directionToLook.x) < 0.45f) 
                directionToLook = new Vector2(Mathf.Sign(directionToLook.x) * 0.45f, directionToLook.y);
        }
    }

    protected override void updateDirectionCreatureFaces() 
    {
        if (EnemySpotted)
            body.localRotation = (directionToLook.x > 0) ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        else if (controller.dirX >= 0)
            body.localRotation = Quaternion.Euler(0, 0, 0);
        else
            body.localRotation = Quaternion.Euler(0, 180, 0);
    }

    private IEnumerator scanForNearbyEnemies()
    {
        yield return new WaitForSeconds(0.3f);
        EnemySpotted = null;

        Collider2D[] enemiesWithinRangeOfWeapon = Physics2D.OverlapCircleAll(
                transform.position, 
                weaponSystem.CurrentWeaponConfiguration.weaponRange,
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
                weaponSystem.CurrentWeaponConfiguration.weaponRange, 
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

    private float sqrDistance(Vector2 a, Vector2 b) 
    {
        Vector2 diff = a-b;
        return diff.x * diff.x + diff.y * diff.y;
    }
}
