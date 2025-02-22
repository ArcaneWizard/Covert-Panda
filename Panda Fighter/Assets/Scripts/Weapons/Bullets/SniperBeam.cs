using System.Collections.Generic;

using MEC;

using UnityEngine;

public class SniperBeam : StaticBullet
{
    private LineRenderer beam;
    private BoxCollider2D collider;
    private Color color;

    private Animator impactExplosion;
    private int explosionCounter;

    private Vector2 initialColliderSize;
    private float beamLength;
    private float beamDistance = 60f;

    public override void OnFire(Vector2 aim)
    {
        base.OnFire(aim);
        beam.startColor = new Color(color.r, color.g, color.b, 1f);
        beam.endColor = new Color(color.r, color.g, color.b, 1f);
    }

    public void ShowBeam(Vector2 aim)
    {
        beam.SetPosition(0, transform.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 100f, LayerMasks.MapOrTarget(transform));

        if (hit.collider != null) {
            beam.SetPosition(1, hit.point);
            beamLength = (hit.point - (Vector2)transform.position).magnitude / transform.localScale.x;
            initiateExplosionAt(hit.point);
        } else {
            beam.SetPosition(1, transform.position + transform.right * beamDistance);
            beamLength = beamDistance / transform.localScale.x;
        }

        collider.size = initialColliderSize + new Vector2(beamLength, 0);
        collider.offset = new Vector2(beamLength / 2, 0);
        Timing.RunSafeCoroutine(fadeBeam(), gameObject);
        OnFire(aim);
    }

    protected override void Awake()
    {
        base.Awake();
        beam = transform.GetComponent<LineRenderer>();
        collider = transform.GetComponent<BoxCollider2D>();

        explosionCounter = 0;
        impactExplosion = transform.GetChild(explosionCounter).transform.GetComponent<Animator>();

        initialColliderSize = collider.size;
        color = beam.startColor;
    }


    protected override void OnMapCollision(CollisionInfo info) { }
    protected override void OnCreatureCollision(CollisionInfo info, Transform creature) { }

    private IEnumerator<float> fadeBeam()
    {
        yield return Timing.WaitForSeconds(0.41f);
        impactExplosion.SetBool("impactExplosion", false);

        while (beam.startColor.r > 0.6f) {
            alterBeamColor(0.08f, 0.088f, 0.08f, 0.05f);
            yield return Timing.WaitForSeconds(0.04f);
        }

        while (beam.startColor.a > 0f) {
            alterBeamColor(0.08f, 0.088f, 0.08f, 0.08f);
            yield return Timing.WaitForSeconds(0.04f);
        }

        gameObject.SetActive(false);
    }

    private void alterBeamColor(float r, float g, float b, float a)
    {
        beam.startColor = new Color(
               beam.startColor.r - r, beam.startColor.g - g,
               beam.startColor.r - b, beam.startColor.a - a
           );
        beam.endColor = new Color(
            beam.startColor.r, beam.startColor.g,
            beam.startColor.b, beam.startColor.a
        );
    }

    private void initiateExplosionAt(Vector3 location)
    {
        impactExplosion.transform.position = location;
        impactExplosion.SetBool("impactExplosion", true);
    }
}
