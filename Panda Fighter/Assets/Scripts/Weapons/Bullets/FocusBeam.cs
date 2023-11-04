using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class FocusBeam : StaticBullet
{
    private LineRenderer beam;
    private BoxCollider2D collider;

    private Vector2 initialColliderSize;
    private float beamLength;
    private float beamDistance = 60f;

    private float timerStayAlive;

    public void ShootBeam(Transform bulletSpawnPoint, Transform weapon, bool inDoubleJump)
    {
        timerStayAlive = Time.deltaTime;

        beam.SetPosition(0, bulletSpawnPoint.position);

        RaycastHit2D hit = Physics2D.Raycast(
            bulletSpawnPoint.position,
            weapon.right,
            100f, LayerMasks.MapOrTarget(transform)
        );

        if (hit.collider != null)
        {
            beam.SetPosition(1, hit.point);
            beamLength = (hit.point - (Vector2)bulletSpawnPoint.position).magnitude / transform.localScale.x;
        }
        else
        {
            beam.SetPosition(1, bulletSpawnPoint.position + transform.right * beamDistance);
            beamLength = beamDistance / transform.localScale.x;
        }

        collider.size = initialColliderSize + new Vector2(beamLength, 0);
        collider.offset = new Vector2(beamLength / 2, 0);
    }

    protected override void Awake()
    {
        base.Awake();

        beam = transform.GetComponent<LineRenderer>();
        collider = transform.GetComponent<BoxCollider2D>();
        initialColliderSize = collider.size;
    }

    protected override void OnCreatureCollision(CollisionInfo info, Transform creature) { }
    protected override void OnMapCollision(CollisionInfo info) { }

    void Update()
    {
        if (timerStayAlive > 0)
            timerStayAlive -= Time.deltaTime;
        else
            gameObject.SetActive(false);
    }
}
