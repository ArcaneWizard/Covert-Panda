using System.Collections;

using MEC;

using UnityEngine;

// THIS CLASS IS NOT CURRENTLY BEING USED
public class FragGrenade : MovingBullet
{
    private int surfacesTouched = 0;
    private float explosionTimer = 0;
    private float timeTillExplosion = 1.2f;

    private SpriteRenderer sR;
    private GameObject animatedExplosion;
    private Explosion explosion;

    public void StartExplosionTimer() => StartCoroutine(eStartExplosionTimer());

    protected override void Awake()
    {
        base.Awake();
        sR = transform.GetComponent<SpriteRenderer>();
        animatedExplosion = transform.GetChild(0).gameObject;
        explosion = transform.GetComponent<Explosion>();

        explosion.Radius = 12;
    }

    protected override void OnCreatureCollision(CollisionInfo info, Transform creature) { }
    protected override void OnMapCollision(CollisionInfo info) { }

    private IEnumerator eStartExplosionTimer()
    {
        yield return new WaitForSeconds(timeTillExplosion);

        rig.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.localEulerAngles = new Vector3(0, 0, 0);

        sR.enabled = false;
        animatedExplosion.SetActive(true);
        explosionTimer = 1.4f;

        Timing.RunSafeCoroutine(explosion.EnableExplosion(weaponConfiguration.Creature, weaponConfiguration.Damage), gameObject);
    }

    void Update()
    {
        if (explosionTimer > 0f)
            explosionTimer -= Time.deltaTime;

        else if (animatedExplosion.activeSelf) {
            rig.constraints = RigidbodyConstraints2D.None;

            sR.enabled = true;
            animatedExplosion.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
