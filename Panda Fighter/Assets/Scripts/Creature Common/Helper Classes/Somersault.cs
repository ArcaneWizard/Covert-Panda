using System.Collections;

using UnityEngine;

// Handle the execution + animation of a mid-air somersault (when the creature double-jumps)

public class Somersault
{
    public SomersaultState State { get; private set; }

    private const float INIT_SOMERSAULT_SPEED = 1000f;
    private const float END_SOMERSAULT_SPEED = 800f;
    private const float SOMERSAULT_DURATION = 0.4f;
    protected int somersaultDirection;

    private float startTime;
    private float zAngle;

    private Transform transform;
    private CentralController controller;
    private CentralLookAround lookAround;
    private CentralPhaseTracker phaseTracker;
    private Animator animator;

    public Somersault(Transform transform, CentralPhaseTracker phaseTracker, Animator animator,
        CentralController controller, CentralLookAround lookAround)
    {
        this.transform = transform;
        this.phaseTracker = phaseTracker;
        this.animator = animator;
        this.controller = controller;
        this.lookAround = lookAround;
    }

    public void Reset() => State = SomersaultState.Exited;

    // Setup creature to do a double jump somersault. Factor in the direction to somersault in, 
    // disable limb movement during the somersault (ex. can't control head movement with cursor), 
    // and give the creature a smaller collider than normal during the somersault
    public IEnumerator Begin()
    {
        // setup 
        somersaultDirection = controller.DirX != 0 ? -controller.DirX : (lookAround.IsFacingRight ? -1 : 1);
        startTime = Time.time;

        /* if (controller.DirX == -1 && lookAround.IsFacingRight)
             animator.SetBool("somersault forwards", false);
         else if (controller.DirX == 1 && !lookAround.IsFacingRight)
             animator.SetBool("somersault forwards", false);
         else
             animator.SetBool("somersault forwards", true);*/

        bool isForwardSomersault = (controller.DirX == -1 && lookAround.IsFacingRight) || (controller.DirX == 1 && !lookAround.IsFacingRight);
        phaseTracker.SwapSomersaultAnimation(isForwardSomersault);

        // start somersault
        State = SomersaultState.Started;
        yield return new WaitForSeconds(0.2f);
        State = SomersaultState.MidWay;
    }

    public void Tick()
    {
        // if creature isn't double jumping, reset the somersault direction 
        /* if (!phaseTracker.Is(Phase.DoubleJumping))
         {
             state = SomersaultState.Exited;
             animator.SetBool("somersault forwards", true);
             return;
         }*/

        if (State == SomersaultState.Started)
            doSomersault();

        else if (State == SomersaultState.MidWay) {
            doSomersault();

            float z = MathX.ClampAngleTo360(zAngle);
            bool spunBackUpright = (somersaultDirection == -1 && z < 40)
                || (somersaultDirection == 1 && z > 320);

            if (spunBackUpright)
                State = SomersaultState.NearFinished;
        } else if (State == SomersaultState.NearFinished)
            endSomersault();
    }

    // To double jump, the creature somersaults at a specified speed for a specified duration, 
    // then slows down rotating to a slower speed. The creature's rotation is synced to the progress of 
    // the double jump animation.
    private void doSomersault()
    {
        float timeElapsed = Time.time - startTime;

        if (timeElapsed <= SOMERSAULT_DURATION) {
            zAngle = timeElapsed * INIT_SOMERSAULT_SPEED * somersaultDirection;
            transform.eulerAngles = new Vector3(0, 0, zAngle);
        } else {
            zAngle = SOMERSAULT_DURATION * INIT_SOMERSAULT_SPEED * somersaultDirection
                    + (timeElapsed - SOMERSAULT_DURATION) * END_SOMERSAULT_SPEED * somersaultDirection;
            transform.eulerAngles = new Vector3(0, 0, zAngle);
        }
    }

    private void endSomersault()
    {
        float z = MathX.ClampAngleTo180(zAngle);

        if (Mathf.Abs(z) <= 8) {
            State = SomersaultState.Exited;
            animator.SetBool("somersault forwards", true);
            transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        } else {
            zAngle = (z > 0) ? z - 15f : z + 15f;
            transform.localEulerAngles = new Vector3(0f, 0f, zAngle);
        }
    }
}