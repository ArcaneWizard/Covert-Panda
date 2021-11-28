using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_Foot : MonoBehaviour
{
    public bool stickToGround { get; private set; }
    private Animator animator;
    private LayerMask map = 1 << 11;

    public Transform frontLeg, backLeg;
    public Transform frontThigh, backThigh;
    public Transform upperBody;

    [Range(0, 4)]
    public float thighYOffset;
    [Range(-1, 1)]
    public float frontThighXOffset;
    [Range(-1, 1)]
    public float backThighXOffset;

    public Vector3 upperBodyOffset = new Vector2(0.1f, 0.2f);

    private void Awake()
    {
        animator = transform.GetComponent<Animator>();
    }

    public void StickToGround() => stickToGround = true;
    public void UnstickToGround() => stickToGround = false;

    private void Update()
    {
        if (stickToGround)
        {
            legSticksToGround(frontLeg);
            legSticksToGround(backLeg);

            float maxYPos_Thigh = Mathf.Min(frontLeg.position.y, backLeg.position.y) + thighYOffset;
            int dir = (transform.localEulerAngles.y == 0) ? 1 : -1;

            frontThigh.position = new Vector3(transform.position.x - frontThighXOffset * dir, maxYPos_Thigh, frontThigh.position.z);
            backThigh.position = new Vector3(transform.position.x + backThighXOffset * dir, maxYPos_Thigh, backThigh.position.z);
            upperBody.position = new Vector3(transform.position.x, maxYPos_Thigh, upperBody.position.z)
                + new Vector3(upperBodyOffset.x * dir, upperBodyOffset.y);
        }

    }
    private void legSticksToGround(Transform leg)
    {
        RaycastHit2D hit2D = Physics2D.Raycast(new Vector2(leg.position.x, leg.position.y) + Vector2.up, Vector2.down, 5f, map);

        if (hit2D.collider != null)
            leg.position = new Vector2(leg.position.x, hit2D.point.y);
    }
}
