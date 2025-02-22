using UnityEngine;

/// <summary>
/// Moving bullets are bullets that move with a rigidbody. They use predictive raycasts to detect 
/// collisions, so regardless of speed, they won't accidently phase through thin walls in a single frame.
/// </summary>

[RequireComponent(typeof(Rigidbody2D))]
public class MovingBullet : Bullet
{
    protected Rigidbody2D rig;

    private bool isDetectingCollisions;
    private Vector2 velocityDir;

    // Predicted Info
    private Vector2? predictedImpactLocation;
    private RaycastHit2D predictedRaycastHit;
    protected Vector2 predictedNormalOfCollisionSurface;
    private int scanAheadRaycastDistance = 70;

    public override void OnFire(Vector2 aim)
    {
        base.OnFire(aim);
        isDetectingCollisions = true;
        predictedImpactLocation = null;
        predictedRaycastHit = new RaycastHit2D();
        scanAheadRaycastDistance = 15; // make dependent on rigidbody velocity?

        velocityDir = aim.normalized;
        shootRaycastAndScanForObstacles();
    }

    protected override void Awake()
    {
        base.Awake();
        rig = transform.GetComponent<Rigidbody2D>();
    }

    protected virtual void FixedUpdate()
    {
        if (!isDetectingCollisions)
            return;

        if (rig.linearVelocity.magnitude != 0)
            velocityDir = rig.linearVelocity.normalized;

        shootRaycastAndScanForObstacles();
        executePredictedCollisionWhenApplicable();
    }

    private void shootRaycastAndScanForObstacles()
    {
        // start the predictive raycast from slightly behind where the bullet actually spawns
        // helps detect collisions on creatures overlapping with the spawned bullet
        Vector2 roughBulletSpawnPos = new Vector2(
            transform.position.x - velocityDir.x * 0.1f,
            transform.position.y - velocityDir.y * 0.1f);

        RaycastHit2D hit = Physics2D.Raycast(roughBulletSpawnPos, velocityDir, scanAheadRaycastDistance,
            LayerMasks.MapOrTarget(transform));

        predictedRaycastHit = hit;
        predictedImpactLocation = (hit.collider != null)
            ? hit.point
            : new Vector2(transform.position.x + velocityDir.x * scanAheadRaycastDistance,
                          transform.position.y + velocityDir.y * scanAheadRaycastDistance);

        if (hit.collider != null)
            predictedNormalOfCollisionSurface = hit.normal;
    }

    private void executePredictedCollisionWhenApplicable()
    {
        if (predictedImpactLocation == null)
            return;

        // determine bullet's x and y position next frame
        float x = Mathf.Sign((transform.position.x + velocityDir.x * 2f) - predictedImpactLocation.Value.x);
        float y = Mathf.Sign((transform.position.y + velocityDir.y * 2f) - predictedImpactLocation.Value.y);

        // if the bullet next frame is going to be at or beyond the intended impact location, teleport
        // to the collision spot and register a collision
        if ((x == Mathf.Sign(velocityDir.x) || x == 0) && (y == Mathf.Sign(velocityDir.y) || y == 0)) {
            isDetectingCollisions = false;

            if (predictedRaycastHit.collider == null)
                return;

            bool collidedWithCreature = predictedRaycastHit.transform.parent.GetComponent<Health>();
            if (predictedRaycastHit.collider != null && collidedWithCreature) {
                transform.position = predictedImpactLocation.Value;
                predictedRaycastHit.collider.transform.parent.GetComponent<Health>().InflictDamage(runtimeBulletDamage, weaponConfiguration.Creature);

                var collisionInfo = new CollisionInfo(predictedRaycastHit.collider, predictedRaycastHit.point);
                OnCreatureCollision(collisionInfo, predictedRaycastHit.collider.transform.parent);
            } else if (predictedRaycastHit.collider != null) {
                transform.position = predictedImpactLocation.Value;
                var collisionInfo = new CollisionInfo(predictedRaycastHit.collider, predictedRaycastHit.point);
                OnMapCollision(collisionInfo);
            }
        }
    }
}
