using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProceduralAnimation : MonoBehaviour
{
    //just fell off a platform
    public bool slipped { get; protected set; }

    protected Rigidbody2D rig;
    protected CentralController controller;
    protected LayerMask map = 1 << 11;
    protected Transform entity, body;

    protected Transform frontLeg, backLeg;
    protected Transform frontThigh, backThigh;
    protected Transform frontFoot, backFoot;
    protected Transform upperBody, head;

    public virtual void OnEnter() { }
    public abstract void Tick();

    public void PretendConstructor(Bones bones)
    {
        entity = transform.parent.parent;
        body = transform.parent;

        controller = entity.GetComponent<CentralController>();
        rig = entity.GetComponent<Rigidbody2D>();

        frontLeg = bones.frontLeg;
        backLeg = bones.backLeg;
        frontThigh = bones.frontThigh;
        backThigh = bones.backThigh;
        frontFoot = bones.frontFoot;
        backFoot = bones.backFoot;
        upperBody = bones.upperBody;
        head = bones.head;
    }

    protected int directionFacing => body.localEulerAngles.y == 0 ? 1 : -1;
    protected int directionAngle => body.localEulerAngles.y == 0 ? 0 : 180;
    protected bool facingRight => body.localEulerAngles.y == 0;
}
