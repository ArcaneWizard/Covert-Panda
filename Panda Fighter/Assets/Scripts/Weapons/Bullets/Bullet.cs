using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector2 aim;
    private bool sticksToCreatures;
    private bool hasArcMotion;
    private bool stopDetectingPhysicalCollisions;

    // Configures the bullet properly the moment before it is fired. Assumes the bullet is already at it's initial spawn position.
    // Takes in the initial aim vector of the shot, whether the bullet has an arc motion (vs a straight line),
    // and whether the bullet is supposed to stick to enemy creatures.
    public virtual void ConfigureBulletBeforeFiring(Vector2 aim, bool doesBulletHaveArcMotion, bool doesBulletStickToCreatures)
    {
        doesBulletHaveArcMotion = false;
        stopDetectingPhysicalCollisions = false;

        // enable predictive collision logic for fast bullets so they don't accidently phase through walls
        if (transform.parent.GetComponent<WeaponConfiguration>().BulletSpeed > 55f)
        {
            stopDetectingPhysicalCollisions = true;
            predictedImpactLocation = Vector2.zero;
            predictedColliderHit = null;

            executedPredictedImpact = false;
            raycastDistance = doesBulletHaveArcMotion ? 15 : 70;

            this.aim = aim;
            hasArcMotion = doesBulletHaveArcMotion;
            sticksToCreatures = doesBulletStickToCreatures;

            raycastLogic();
        }
    }

    // Method invoked whenever the bullet hits a creature. By default, deactivates the bullet
    protected virtual void OnCreatureEnter(Transform creature) => StartCoroutine(deactivateBullet());

    // Method invoked whenever the bullet hits a physical platform/object on the map. By default, deactivates the bullet.
    protected virtual void OnMapEnter(Transform map) => StartCoroutine(deactivateBullet());

    // Returns the square of the distance between two vectors
    protected float squaredDistance(Vector2 a, Vector2 b)
    {
        Vector2 c = a - b;
        return c.x * c.x + c.y * c.y;
    }

    // For sticky bullets, keeps track of the limb that the bullet stuck to 
    protected Transform whatItStuckTo { get; private set; }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (stopDetectingPhysicalCollisions)
            return;

        else if (col.gameObject.layer == Layer.GetHitBoxOfOpposition(gameObject))
        {
            stopDetectingPhysicalCollisions = true;
            OnCreatureEnter(col.transform);

            // damage the collided creature, which requires a reference to who fired this bullet
            Transform attacker = transform.parent.parent.parent.parent;
            col.transform.parent.GetComponent<Health>().TakeDamage(damage(), attacker);
        }

        else if (col.gameObject.layer == Layer.DefaultGround || col.gameObject.layer == Layer.OneWayGround)
        {
            stopDetectingPhysicalCollisions = true;
            OnMapEnter(col.transform);
        }
    }

    private IEnumerator deactivateBullet()
    {
        transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        yield return new WaitForSeconds(Time.deltaTime);
        gameObject.SetActive(false);
    }

    // Get the damage this bullet does
    protected virtual int damage() => transform.parent.GetComponent<WeaponConfiguration>().BulletDmg;


    // ---------------------------------------------------------------------------------------------------------------------
    // Below is the implementation for predictive collision detection logic. For super-fast bullets, collider-based detection
    // won't work as bullets often glitch through walls and platforms
    // ---------------------------------------------------------------------------------------------------------------------

    // the predicted impact location if known 
    private Vector2 predictedImpactLocation;

    // who or what is predicted to be hit by the bullet
    private Transform predictedColliderHit;

    // the normal of the predicted collider hit
    protected Vector2 predictedNormal;

    private Vector2 middleOfWeapon;
    private int raycastDistance = 70;
    private bool executedPredictedImpact = true;
    private float x, y;

    protected virtual void Update()
    {
        if (executedPredictedImpact)
            return;

        raycastLogic();
    }

    protected virtual void FixedUpdate()
    {
        if (hasArcMotion && transform.GetComponent<Rigidbody2D>().velocity != Vector2.zero)
            aim = transform.GetComponent<Rigidbody2D>().velocity.normalized;

        checkForPredictedCollision();
    }


    private void raycastLogic() 
    {
        checkForPredictedCollision();

        // start the predictive raycast from slightly behind where the bullet actually spawns (to detect collisions on creatures walking the gun)
        middleOfWeapon = new Vector2(transform.position.x - aim.x * 0.1f, transform.position.y - aim.y * 0.1f);
        RaycastHit2D hit = Physics2D.Raycast(middleOfWeapon, aim, raycastDistance, LayerMasks.mapOrTarget(transform));

        //if (hit.collider != null)
        //     Debug.DrawLine(new Vector2(transform.position.x, transform.position.y), hit.point, Color.green, 4);
        predictedImpactLocation = (hit.collider != null) ? hit.point : new Vector2(transform.position.x + aim.x * raycastDistance, transform.position.y + aim.y * raycastDistance);
        predictedColliderHit = (hit.collider != null) ? hit.collider.transform : null;

        if (hit.collider != null)
            predictedNormal = hit.normal;
    }

    // if a fast bullet reaches the predicted collision location, damage the predicted enemy hit if applicable and trigger
    // either onCreatureEnter() or OnMapEnter() 
    private void checkForPredictedCollision()
    {
        // if the predicted collision was already executed, or a prediction has not been set, no need to run any predictive impact logic
        if (executedPredictedImpact || predictedImpactLocation == Vector2.zero)
            return;

        // get the diff btwn the x and y coordinates of the bullet's position next frame and the predicted impact location 
        x = Mathf.Sign((transform.position.x + aim.x * 2f) - predictedImpactLocation.x);
        y = Mathf.Sign((transform.position.y + aim.y * 2f) - predictedImpactLocation.y);

        // if the bullet next frame is going to be at the bullet impact location or beyond it (in the direction the bullet is moving),
        // execute the predicted impact
        if ((x == Mathf.Sign(aim.x) || x == 0) && (y == Mathf.Sign(aim.y) || y == 0))
        {
            executedPredictedImpact = true;

            // if the bullet was predicted to hit a creature (as opposed to the environment)
            if (predictedColliderHit != null && predictedColliderHit.parent.GetComponent<Health>())
            {
                // since we were predicted a frame ahead, it would make sense to teleport the bullet ot the impact location
                // however apply an offset PARALLEL to the direction the bullet is traveling, so that the bullet teleports to the 
                // creature instead (who may have moved from the impact location since the last frame)
                /*float xDiff = predictedColliderHit.position.x - predictedImpactLocation.x;
                Vector2 predictedOffset = new Vector2(xDiff + UnityEngine.Random.Range(-0.1f, 0.1f), aim.y / aim.x * xDiff);
                predictedImpactLocation += predictedOffset;*/

                // if the bullet should stick to the creature, it needs to teleport to a leg if it lands near the lower body
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
                            distance = squaredDistance(lowerLimb.transform.position, predictedImpactLocation);
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

                // damage the predicted enemy hit, and register said collision (the bullet may have an explosion or something)
                Transform attacker = transform.parent.parent.parent.parent;
                predictedColliderHit.parent.GetComponent<Health>().TakeDamage(damage(), attacker);
                OnCreatureEnter(predictedColliderHit.parent);
                stopDetectingPhysicalCollisions = true;
            }
        
            // if the bullet was predicted to hit part of the map
            else if (predictedColliderHit != null)
            {
                transform.position = predictedImpactLocation;
                OnMapEnter(predictedColliderHit);
            }
        }
    }
}
