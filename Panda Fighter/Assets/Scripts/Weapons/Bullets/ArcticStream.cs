using MEC;

using UnityEngine;

public class ArcticStream : MovingBullet
{
    private float explosionTimer = 0;

    private GameObject animatedExplosion;
    private Explosion explosion;

    protected override void Awake()
    {
        base.Awake();
        animatedExplosion = transform.GetChild(0).gameObject;
        explosion = transform.GetComponent<Explosion>();
        explosion.Radius = 13;
    }

    protected override void OnMapCollision(CollisionInfo info) => startExplosion();
    protected override void OnCreatureCollision(CollisionInfo info, Transform creature) => startExplosion();

    void Update()
    {
        if (explosionTimer > 0f)
            explosionTimer -= Time.deltaTime;

        else if (animatedExplosion.activeSelf) {
            rig.constraints = RigidbodyConstraints2D.None;
            animatedExplosion.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    private void startExplosion()
    {
        if (explosionTimer <= 0f && rig.constraints != RigidbodyConstraints2D.FreezeAll) {
            rig.constraints = RigidbodyConstraints2D.FreezeAll;
            transform.localEulerAngles = new Vector3(0, 0, 0);
            animatedExplosion.SetActive(true);
            explosionTimer = 1.4f;

            Timing.RunSafeCoroutine(explosion.EnableExplosion(weaponConfiguration.Creature, weaponConfiguration.ExplosionDmg), gameObject);
        }
    }
}
