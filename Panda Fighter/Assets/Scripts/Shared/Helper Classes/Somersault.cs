using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handle the execution + animation of a mid-air somersault (when the creature double-jumps)

public class Somersault
{
    public SomersaultState state {get; private set;}

    private const float initSomersaultSpeed = 600f;
    private const float endSomersaultSpeed = 600f;
    private const float somersaultDuration = 0.4f;
    protected int somersaultDirection;

    private Transform transform;
    private Transform body;
    private CentralController controller;
    private CentralPhaseTracker phaseTracker;
    private Animator animator;

    public Somersault(Transform transform, CentralPhaseTracker phaseTracker, 
        Animator animator, CentralController controller) 
    {
        this.transform = transform;
        this.body = transform.GetChild(0);
        this.phaseTracker = phaseTracker;
        this.animator = animator;
        this.controller = controller;
    }

    public void Reset() => state = SomersaultState.Exited;

    // Setup creature to do a double jump somersault. Factor in the direction to somersault in, 
    // disable limb movement during the somersault (ex. can't control head movement with cursor), 
    // and give the creature a smaller collider than normal during the somersault
    public IEnumerator Begin()
    {
        // setup 
        somersaultDirection = controller.DirX != 0 ? -controller.DirX : ((body.localEulerAngles.y == 0) ? -1 : 1);

        if (controller.DirX == -1 && body.localEulerAngles.y == 0)
            animator.SetBool("somersault forwards", false);
        else if (controller.DirX == 1 && body.localEulerAngles.y == 180)
            animator.SetBool("somersault forwards", false);
        else
            animator.SetBool("somersault forwards", true);

        // start somersault
        state = SomersaultState.Started;
        yield return new WaitForSeconds(0.2f);
        state = SomersaultState.MidWay;
    }

    public void Tick()
    {
        // if creature isn't double jumping, reset the somersault direction 
        if (!phaseTracker.Is(Phase.DoubleJumping))
        {
            state = SomersaultState.Exited;
            animator.SetBool("somersault forwards", true);
            return;
        }

        if (state == SomersaultState.Started)
            doSomersault();

        else if (state == SomersaultState.MidWay)
        {
            doSomersault();

            bool spunBackUpright = (somersaultDirection == -1 && transform.localEulerAngles.z < 30)
                || (somersaultDirection == 1 && (transform.localEulerAngles.z > 0 && transform.localEulerAngles.z < 40))
                || (somersaultDirection == 1 && transform.localEulerAngles.z > 345);

            if (spunBackUpright) 
                state = SomersaultState.NearFinished;
        }

        else if (state == SomersaultState.NearFinished)
            endSomersault();
    }

    // To double jump, the creature somersaults at a specified speed for a specified duration, 
    // then slows down rotating to a slower speed. The creature's rotation is synced to the progress of 
    // the double jump animation.
    private void doSomersault()
    {
        float t = ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) + 1) % 1;

        if (t < somersaultDuration)
            transform.eulerAngles = new Vector3(0, 0, t * initSomersaultSpeed * somersaultDirection);
        else
        {
            float zAngle = somersaultDuration * initSomersaultSpeed * somersaultDirection
                    + (t - somersaultDuration) * endSomersaultSpeed * somersaultDirection;
            transform.eulerAngles = new Vector3(0, 0, zAngle);
        }
    }

    private void endSomersault()
    {
        float z = (transform.localEulerAngles.z + 360) % 360;

        if (Mathf.Abs(z - 360) < 2 || Mathf.Abs(z) < 2)
        {
            state = SomersaultState.Exited;
            transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        }
        else
        {
            z = (z < 180) ? transform.localEulerAngles.z - 2f : transform.localEulerAngles.z + 2f;
            transform.localEulerAngles = new Vector3(0f, 0f, z);
        }
    }
}
