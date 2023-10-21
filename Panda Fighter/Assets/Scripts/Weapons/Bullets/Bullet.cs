using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Bullet collision logic is handled here, such as whether the bullet collided with a platform or should damage a creature.
// Can take in additional specifiers such as whether the bullet sticks to platforms/creatures.
// Note: slow bullets use trigger collider detection to detect collisions, while fast moving bullets
// using predictive raycast logic to detect collisions. This avoids a bug where fast bullets phase through thin walls. 

public class Bullet : MonoBehaviour
{
    private enum CollisionDetectionMode
    {
        PhysicalColliders,
        PredictiveRaycast,
        None
    }

    // Important internal info about this bullet
    private Vector2 velocityDir;
    private CollisionDetectionMode collisionDetectionMode = CollisionDetectionMode.None;
    private BulletMovementAfterFiring bulletMovement;
    private bool sticksToCreatures;

    // Other
    protected WeaponConfiguration weaponConfiguration;
    protected Rigidbody2D rig;
    private CentralLookAround lookAround;
    private Transform creature;

    // the predicted impact location if known 
    private Vector2 predictedImpactLocation;
    // who or what is predicted to be hit by the bullet
    private Transform predictedColliderHit;
    // the normal of the predicted collider hit
    protected Vector2 predictedNormal;
    // how far the bullet predictively raycasts forward
    private int bulletRaycastDistance = 70;

    protected virtual void Awake()
    {
        weaponConfiguration = transform.parent.GetComponent<WeaponConfiguration>();
        rig = transform.GetComponent<Rigidbody2D>();
        creature = transform.parent.parent.parent.parent;
        lookAround = creature.GetChild(0).GetComponent<CentralLookAround>();    
    }

    // Starts up bullet collision detection (ideally right before the bullet is fired).
    // Takes in the initial aim vector of the gun, whether the bullet travels in an arc motion (vs straight line),
    // and whether the bullet is supposed to stick to enemy creatures.
    public virtual void StartCollisionDetection(Vector2 aim, BulletMovementAfterFiring movementAfterFiring, bool doesBulletStickToCreatures)
    {
        // enable predictive collision logic for fast bullets so they don't accidently phase through walls
        if (weaponConfiguration.Speed > 55)
        {
            collisionDetectionMode = CollisionDetectionMode.PredictiveRaycast;
            predictedImpactLocation = Vector2.zero;
            predictedColliderHit = null;

            bulletMovement = movementAfterFiring;
            bulletRaycastDistance = movementAfterFiring == BulletMovementAfterFiring.Arc ? 15 : 70;

            velocityDir = aim.normalized;
            sticksToCreatures = doesBulletStickToCreatures;

            raycastLogic();
        }
        else
            collisionDetectionMode = CollisionDetectionMode.PhysicalColliders;

        // confirm bullet was fired
        onFire();
    }

    // Invoked at the instant the bullet is fired
    protected virtual void onFire() { }

    // Invoked whenever the bullet hits a creature. By default, deactivates the bullet
    protected virtual void onCreatureEnter(Transform creature) => StartCoroutine(deactivateBullet());

    // Invoked whenever the bullet hits a physical platform/object on the map. By default, deactivates the bullet.
    protected virtual void onMapEnter(Transform map) => StartCoroutine(deactivateBullet());

    // Set the damage this bullet does. By default, use the damage specified for this bullet in all situations.
    protected virtual int damage() => weaponConfiguration.Damage;

    // For sticky bullets, keeps track of the limb that the bullet stuck to 
    protected Transform whatItStuckTo { get; private set; }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // only continue if collision detection is done via physical colliders
        if (collisionDetectionMode != CollisionDetectionMode.PhysicalColliders)
            return;

        // if the bullet hits a creature
        else if (col.gameObject.layer == Layer.GetHitBoxOfOpposition(gameObject))
        {
            // stop collision detection for this bullet unless it is a continuous beam
            if (weaponConfiguration.FiringMode != FiringMode.ContinousBeam)
                collisionDetectionMode = CollisionDetectionMode.None;

            onCreatureEnter(col.transform);
            col.transform.parent.GetComponent<Health>().TakeDamage(damage(), creature);
        }

        // if the bullet collides with the map
        else if (col.gameObject.layer == Layer.DefaultGround || col.gameObject.layer == Layer.OneWayGround)
        {
            // stop collision detection for this bullet unless it is a continuous beam
            if (weaponConfiguration.FiringMode != FiringMode.ContinousBeam)
                collisionDetectionMode = CollisionDetectionMode.None;

            onMapEnter(col.transform);
        }
    }

    private IEnumerator deactivateBullet()
    {
        rig.velocity = Vector2.zero;
        yield return new WaitForSeconds(Time.deltaTime);
        gameObject.SetActive(false);
    }

    // ---------------------------------------------------------------------------------------------------------------------
    // Below is the implementation for predictive collision detection logic. For super-fast bullets, collider-based detection
    // won't work as bullets often glitch through walls and platforms
    // ---------------------------------------------------------------------------------------------------------------------

    protected virtual void Update()
    {
        if (collisionDetectionMode != CollisionDetectionMode.PredictiveRaycast)
            return;

        raycastLogic();
    }

    protected virtual void FixedUpdate()
    {
        if (collisionDetectionMode != CollisionDetectionMode.PredictiveRaycast)
            return;

        if (bulletMovement == BulletMovementAfterFiring.Arc && rig.velocity != Vector2.zero)
            velocityDir = rig.velocity.normalized;
        else if (bulletMovement == BulletMovementAfterFiring.SyncedWithDirectionAimedIn)
            velocityDir = lookAround.directionToLook;

        checkForPredictedCollision();
    }

    private void raycastLogic() 
    {
        checkForPredictedCollision();

        // start the predictive raycast from slightly behind where the bullet actually spawns (to detect collisions on creatures overlapping with the spawned bullet)
        Vector2 roughBulletSpawnPos = new Vector2(transform.position.x - velocityDir.x * 0.1f, transform.position.y - velocityDir.y * 0.1f);
        RaycastHit2D hit = Physics2D.Raycast(roughBulletSpawnPos, velocityDir, bulletRaycastDistance, LayerMasks.MapOrTarget(transform));

        //if (hit.collider != null)
        //     Debug.DrawLine(new Vector2(transform.position.x, transform.position.y), hit.point, Color.green, 4);
        predictedImpactLocation = (hit.collider != null) ? hit.point : new Vector2(transform.position.x + velocityDir.x * bulletRaycastDistance, transform.position.y + velocityDir.y * bulletRaycastDistance);
        predictedColliderHit = (hit.collider != null) ? hit.collider.transform : null;

        if (hit.collider != null)
            predictedNormal = hit.normal;
    }

    // if a fast bullet reaches the predicted collision location, damage the predicted enemy hit if applicable and trigger
    // either onCreatureEnter() or OnMapEnter() 
    private void checkForPredictedCollision()
    {
        // if the predicted impact location has not been set yet, don't run any predictive impact logic
        if (predictedImpactLocation == Vector2.zero)
            return;

        // get the diff btwn the x and y coordinates of the bullet's position next frame and the predicted impact location 
        float x = Mathf.Sign((transform.position.x + velocityDir.x * 2f) - predictedImpactLocation.x);
        float y = Mathf.Sign((transform.position.y + velocityDir.y * 2f) - predictedImpactLocation.y);

        // if the bullet next frame is going to be at the bullet impact location or beyond it (in the direction the bullet is moving),
        // the bullet has predictively hit something
        if ((x == Mathf.Sign(velocityDir.x) || x == 0) && (y == Mathf.Sign(velocityDir.y) || y == 0))
        {
            // stop collision detection for this bullet unless it is a continuous beam
            if (weaponConfiguration.FiringMode != FiringMode.ContinousBeam)
                collisionDetectionMode = CollisionDetectionMode.None;

            // if the bullet is predictively hitting a creature (as opposed to the environment)
            if (predictedColliderHit != null && predictedColliderHit.parent && predictedColliderHit.parent.GetComponent<Health>())
            {
                // if applicable, the bullet should stick to the creature
                if (sticksToCreatures)
                {
                    if (predictedImpactLocation.y > predictedColliderHit.position.y - 0.5f)
                        whatItStuckTo = predictedColliderHit;
                    else
                    {
                        GameObject closestLowerLimb = null;
                        float closestDistance = -1, distance;

                        foreach (GameObject lowerLimb in predictedColliderHit.parent.GetChild(0).GetComponent<CreatureLimbs>().LowerBody)
                        {
                            distance = MathX.GetSquaredDistance(lowerLimb.transform.position, predictedImpactLocation);
                            if (distance < closestDistance || closestDistance == -1)
                            {
                                closestLowerLimb = lowerLimb;
                                closestDistance = distance;
                            }
                        }

                        if (closestLowerLimb != null)
                        {
                            predictedImpactLocation = closestLowerLimb.transform.position + new Vector3(UnityEngine.Random.Range(-0.07f, 0.07f), 0f, 0f);
                            whatItStuckTo = closestLowerLimb.transform;
                        }
                        else
                            whatItStuckTo = predictedColliderHit;
                    }
                }

                transform.position = predictedImpactLocation;

                // damage the predicted enemy hit
                predictedColliderHit.parent.GetComponent<Health>().TakeDamage(damage(), creature);

                // register a collision was made with a creature
                onCreatureEnter(predictedColliderHit.parent);
            }
        
            // if the bullet was predicted to hit part of the map
            else if (predictedColliderHit != null)
            {
                transform.position = predictedImpactLocation;

                // register a collision was made with a platform
                onMapEnter(predictedColliderHit);
            }
        }
    }
}

public enum BulletMovementAfterFiring
{
    StraightLine, // bullet travels in straight line without deviating
    Arc,          // bullet arcs/curves
    SyncedWithDirectionAimedIn, // bullet travels in straight line synced with the direction the weapon is aimed in (ex. beam weapon)
}