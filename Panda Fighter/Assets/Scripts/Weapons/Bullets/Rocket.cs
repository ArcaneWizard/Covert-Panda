using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MovingBullet
{
    private SpriteRenderer rocketRenderer;
    private SpriteRenderer rocketGlareRenderer;
    private GameObject physicalExplosion;
    private Explosion explosion;

    protected override void Awake()
    {
        base.Awake();
        rocketRenderer = transform.GetComponent<SpriteRenderer>();
        rocketGlareRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();

        physicalExplosion = transform.GetChild(0).gameObject;
        explosion = transform.GetComponent<Explosion>();
        explosion.Radius = 7f;

        this.NullCheck(explosion, nameof(explosion));
        this.NullCheck(rocketRenderer, nameof(rocketRenderer));
        this.NullCheck(rocketGlareRenderer, nameof(rocketGlareRenderer));
    }

    protected override void OnMapCollision(CollisionInfo info) => 
        Timing.RunSafeCoroutine(setupExplosion(), gameObject);

    protected override void OnCreatureCollision(CollisionInfo info, Transform creature) =>
        Timing.RunSafeCoroutine(setupExplosion(), gameObject);

    private IEnumerator<float> setupExplosion()
    {
        rig.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.localEulerAngles = new Vector3(0, 0, 0);

        physicalExplosion.SetActive(true);
        changeRocketVisibility(false);
        yield return Timing.WaitForSeconds(1.4f);

        Timing.RunSafeCoroutine(explosion.EnableExplosion(), gameObject);
        rig.constraints = RigidbodyConstraints2D.None;
        physicalExplosion.SetActive(false);
        changeRocketVisibility(true);
        gameObject.SetActive(false);
    }

    private void changeRocketVisibility(bool visibility)
    {
        rocketRenderer.enabled = visibility;
        rocketGlareRenderer.enabled = visibility;
    }
}
