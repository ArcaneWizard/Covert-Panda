using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusBeam : MonoBehaviour
{
    private LineRenderer beam;
    private BoxCollider2D collider;

    private Vector2 initialColliderSize;
    private float beamLength;

    private float beamDistance = 60f;
    private LayerMask map = 1 << 11 | 1 << 6;

    private float timerStayAlive;

    void Awake()
    {
        beam = transform.GetComponent<LineRenderer>();
        collider = transform.GetComponent<BoxCollider2D>();

        initialColliderSize = collider.size;
    }

    private void LateUpdate()
    {
        if (!Input.GetMouseButton(0))
            gameObject.SetActive(false);
    }

    public void Beam(Transform bulletSpawnPoint)
    {
        timerStayAlive = Time.deltaTime;

        beam.SetPosition(0, bulletSpawnPoint.position);
        RaycastHit2D hit = Physics2D.Raycast(bulletSpawnPoint.position, transform.right, 100f, map);

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
}