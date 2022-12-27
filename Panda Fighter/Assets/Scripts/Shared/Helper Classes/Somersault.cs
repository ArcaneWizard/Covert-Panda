using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Somersault
{
    public SomersaultState state {get; private set;}

    private const float initSomersaultSpeed = 1000f;
    private const float endSomersaultSpeed = 600f;
    private const float somersaultDuration = 0.35f;
    protected int somersaultDirection;

    private Transform transform;
    private CentralPhaseManager phaseManager;
    private Collider2D mainCollider;
    private Collider2D somersaultCollider;
    private Animator animator;

    public Somersault(Transform transform, CentralPhaseManager phaseManager, Collider2D mainCollider,
        Collider2D somersaultCollider, Animator animator) 
    {
        this.transform = transform;
        this.phaseManager = phaseManager;
        this.mainCollider = mainCollider;
        this.somersaultCollider = somersaultCollider;
        this.animator = animator;
    }

    public enum SomersaultState
    {
        Started,
        UpsideDown,
        NearFinished,
        Exited
    }

    public void Initialize() => state = SomersaultState.Exited;

    public void Run()
    {
        // if creature isn't double jumping, reset the somersault direction 
        if (!phaseManager.IsDoubleJumping) 
        {
            animator.SetBool("somersault forwards", true);
            return;
        }

        if (state == SomersaultState.Started)
        {
            doSomersault();

            if (transform.localEulerAngles.z > 150 && transform.localEulerAngles.z < 250)
                state = SomersaultState.UpsideDown;
        }

        else if (state == SomersaultState.UpsideDown)
        {
            doSomersault();

            bool spunBackUpright = (somersaultDirection == -1 && transform.localEulerAngles.z < 30)
                || (somersaultDirection == 1 && (transform.localEulerAngles.z > 0 && transform.localEulerAngles.z < 40))
                || (somersaultDirection == 1 && transform.localEulerAngles.z > 345);

            if (spunBackUpright)
                state = SomersaultState.NearFinished;
        }

        else if (state == SomersaultState.NearFinished)
        {
            endSomersault();

            if (PhaseMidAir != PhasesMidAir.DoubleJumping)
            {
                state = SomersaultState.Exited;
                doubleJumpCollider.enabled = false;
                controller.mainCollider.enabled = true;
            }
        }
    }

    public void Reset() => state = SomersaultState.Exited;

    // Setup creature to do a double jump somersault. Factor in the direction to somersault in, 
    // disable limb movement during the somersault (ex. can't control head movement with cursor), 
    // and give the creature a smaller collider than normal during the somersault
    public void StartSomersault()
    {
        // setup 
        somersaultDirection = controller.dirX != 0 ? -controller.dirX : ((body.localEulerAngles.y == 0) ? -1 : 1);
        DisableLimbsDuringDoubleJump = true;

        if (controller.dirX == -1 && body.localEulerAngles.y == 0)
            animator.SetBool("somersault forwards", false);
        else if (controller.dirX == 1 && body.localEulerAngles.y == 180)
            animator.SetBool("somersault forwards", false);
        else
            animator.SetBool("somersault forwards", true);

        controller.mainCollider.enabled = false;
        doubleJumpCollider.enabled = true;

        // start somersault
        state = SomersaultState.Started;
    }

    // To double jump, the creature somersaults at a specified speed for a specified duration, 
    // then slows down rotating to a slower speed. The creature's rotation is synced to the progress of 
    // the double jump animation.
    private void doSomersault()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.65f && !animator.IsInTransition(0))
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
        else
            state = SomersaultState.NearFinished;
    }

    private void endSomersault()
    {
        float z = (transform.localEulerAngles.z + 360) % 360;

        if (Mathf.Abs(z - 360) < 2 || Mathf.Abs(z) < 2)
        {
            doubleJumpCollider.enabled = false;
            controller.mainCollider.enabled = true;
        }
        else
        {
            z = (z < 180) ? transform.localEulerAngles.z - 2f : transform.localEulerAngles.z + 2f;
            transform.localEulerAngles = new Vector3(0, 0, z);
        }
    }
}
