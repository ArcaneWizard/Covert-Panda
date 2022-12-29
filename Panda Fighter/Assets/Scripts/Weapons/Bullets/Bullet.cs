using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // returns the damage the bullet does
    public virtual int Damage() => transform.parent.GetComponent<WeaponConfiguration>().bulletDmg;

    // Called whenever the bullet hits a physical platform/object on the map. By default, deactivate the bullet.
    protected virtual void OnMapEnter(Transform map) => StartCoroutine(delay());

    // Called whenever the bullet hits a creature. By default, deactivate the bullet.
    public virtual void OnCreatureEnter(Transform creature) => gameObject.SetActive(false);

    // if a bullet hits any part of the map, trigger OnMapEnter() 
    private void OnTriggerEnter2D(Collider2D col)
    {
        if ((col.gameObject.layer == Layer.DefaultGround || col.gameObject.layer == Layer.OneWayGround) && !disableCollisionDetection)
        {
            disableCollisionDetection = true;
            OnMapEnter(col.transform);
        }
    }

     // for fast bullets, store the predicted hit location + who or what's gonna be hit (predicted when bullet is about to be shot)
    public Vector2 predictedImpactLocation { get; private set; }
    public Transform predictedColliderHit { get; private set; }
    public Vector2 predictedNormal { get; private set; }
    public static int raycastsUsed;

    //  whether or not the bullet can detect physical collisions (with a creature, platform, etc.). Enabled whenever predicting collisions
    public bool disableCollisionDetection;

    // for sticky bullets, keep track of the limb that the bullet stuck to
    protected Transform whatItStuckTo { get; private set; }
    private bool sticksToCreatures;

    private int raycastDistance = 70;
    private bool executedPredictedImpact = true;
    private bool startPredictingBulletImpact = false;
    private bool updateBulletDirContinuously = false;
    private float x, y;

    private Vector2 aim;
    private Vector2 middleOfWeapon;

    // run predictive collision logic for fast bullets. 
    public void RunPredictiveLogic(Vector2 aim, Vector2 bulletStartPosition, bool updateBulletDirContinuously, bool sticksToCreatures)
    {
        predictedImpactLocation = Vector2.zero;
        predictedColliderHit = null;

        executedPredictedImpact = false;
        disableCollisionDetection = true;
        raycastDistance = (updateBulletDirContinuously) ? 15 : 70;
        this.aim = aim;

        this.updateBulletDirContinuously = updateBulletDirContinuously;
        this.sticksToCreatures = sticksToCreatures;
        raycastLogic();
    }

    public virtual void Update()
    {
        if (executedPredictedImpact)
            return;

        raycastLogic();
    }

    void FixedUpdate()
    {
        if (updateBulletDirContinuously && transform.GetComponent<Rigidbody2D>().velocity != Vector2.zero)
            aim = transform.GetComponent<Rigidbody2D>().velocity.normalized;
        
        checkForPredictedCollision();
    }

    private void raycastLogic() 
    {
        WeaponConfiguration weaponConfiguration = transform.parent.GetComponent<WeaponConfiguration>();
        ++raycastsUsed;

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
                            distance = sqrDistance(lowerLimb.transform.position, predictedImpactLocation);
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
                predictedColliderHit.parent.GetComponent<Health>().TakeDamageFromPredictedFastBulletCollision(Damage(), transform);
                OnCreatureEnter(predictedColliderHit.parent);
            }
        
            // if the bullet was predicted to hit part of the map
            else if (predictedColliderHit != null)
            {
                transform.position = predictedImpactLocation;
                OnMapEnter(predictedColliderHit);
            }
        }
    }

    private IEnumerator delay()
    {
        transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        yield return new WaitForSeconds(Time.deltaTime);
        gameObject.SetActive(false);
    }

    protected float sqrDistance(Vector2 a, Vector2 b) {
        Vector2 c = a - b;
        return c.x * c.x + c.y * c.y;
    }
}
