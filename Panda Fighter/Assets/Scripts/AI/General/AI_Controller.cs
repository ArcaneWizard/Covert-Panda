using System.Collections.Generic;

using UnityEngine;

public class AI_Controller : CentralController
{
    /// <summary> The current action the AI is executing (null means none). </summary>
    [field: SerializeField] public AIAction CurrAction { get; private set; }

    // the decision zone that provided the current action the AI should execute
    private Transform decisionZone;
    private bool hasActionStarted;
    private HashSet<Transform> decisionZonesNearby;
    private CentralDeathSequence deathSequence;

    /// <summary> AI starts executing the specified action.
    /// Proivde the zone the action originated from, if applicable. </summary>
    public void StartAction(AIAction AI_action, Transform zone = null)
    {
        CurrAction = AI_action;
        decisionZone = zone;
        hasActionStarted = false;
    }

    public void EndAction() => CurrAction = null;
    public bool IsActionBeingExecuted(AIAction action) => CurrAction.Equals(action);
    public void SetDirection(int dir) => StartAction(AIAction.ChangeDirection(dir));

    protected override void Awake()
    {
        base.Awake();
        deathSequence = transform.GetComponent<CentralDeathSequence>();
        resetOnSpawn();
    }

    void OnEnable() => deathSequence.UponRespawning += resetOnSpawn;
    void OnDisable() => deathSequence.UponRespawning -= resetOnSpawn;
    void OnDestroy() => deathSequence.UponRespawning -= resetOnSpawn;

    protected override void Update()
    {
        base.Update();

        if (health.IsDead) {
            IsTouchingMap = false;
            CurrAction?.Exit();
            CurrAction = null;
            decisionZone = null;
            decisionZonesNearby.Clear();

            return;
        }

        // AI moves at max speed when not executing an action
        if (CurrAction == null) {
            speed = MAX_SPEED;
            return;
        }

        executeCurrentAction();

        if (CurrAction.Finished) {
            CurrAction.Exit();
            CurrAction = null;
        }

        setAlienVelocity();
    }

    protected override void FixedUpdate()
    {
        if (CurrAction == null || CurrAction.Finished)
            return;

        if (CurrAction.ExecuteNormalJumpNow) {
            StartCoroutine(normalJump());
            CurrAction.ExecuteNormalJumpNow = false;
        }
        if (CurrAction.ExecuteDoubleJumpNow) {
            StartCoroutine(doubleJump());
            CurrAction.ExecuteDoubleJumpNow = false;
        }
        if (CurrAction.ExecuteJumpBoostNow) {
            StartCoroutine(jumpPadBoost());
            CurrAction.ExecuteJumpBoostNow = false;
        }
    }

    private void resetOnSpawn()
    {
        DirX = UnityEngine.Random.Range(0, 2) * 2 - 1;
        speed = MAX_SPEED;
        CurrAction = null;
        decisionZone = null;
        decisionZonesNearby = new HashSet<Transform>();
    }

    private void executeCurrentAction()
    {
        if (CurrAction == null)
            return;

        // When the AI is grounded, start executing the current action
        if (IsGrounded && IsTouchingMap && !hasActionStarted) {
            // Abort action if the creature is too far from the og decision zone
            if (!decisionZonesNearby.Contains(decisionZone)) {
                EndAction();
                return;
            }

            CurrAction.StartExecution(this);
            hasActionStarted = true;
        }

        // Execute the action
        if (hasActionStarted) {
            CurrAction.Execute();
            DirX = CurrAction.DirX;
            speed = CurrAction.Speed;
        }
    }


    // handles setting the alien velocity on slopes, while falling, etc.
    private void setAlienVelocity()
    {
        // nullify the slight bounce on a slope glitch when changing slopes
        if ((!phaseTracker.IsMidAir || phaseTracker.Is(Phase.Falling)) && rig.linearVelocity.y > 0)
            rig.linearVelocity = new Vector2(0, 0);

        // when alien is on the ground, alien velocity is parallel to the slanted ground 
        if (!phaseTracker.IsMidAir && IsGrounded && IsTouchingMap) {
            if (CurrAction == null && wallBehindYou)
                DirX = 1;
            else if (CurrAction == null && wallInFrontOfYou)
                DirX = -1;

            rig.linearVelocity = DirX * speed * groundSlope;
            rig.gravityScale = (DirX == 0) ? 0f : Game.GRAVITY;
        }

        // when alien is not on the ground (falling or midair after a jump)
        else {
            // Stop moving horizontally if the AI is about to crash into a wall after a jump.
            if (rig.linearVelocity.y > 0 && ((DirX == 1 && wallInFrontOfYou) || (DirX == -1 && wallBehindYou)))
                rig.linearVelocity = new Vector2(0, rig.linearVelocity.y);

            // Change directions if the AI is falling left or right and about to crash into a wall.
            else if ((DirX == 1 && wallInFrontOfYou) || (DirX == -1 && wallBehindYou)) {
                rig.linearVelocity = new Vector2(0, rig.linearVelocity.y);
                DirX = (int)-Mathf.Sign(DirX) * UnityEngine.Random.Range(0, 2);
                speed = 22;
                CurrAction = null;
            }

            // Set velocity to the left/right with specified speed and direction. Vertical motion is affected by gravity
            else
                rig.linearVelocity = new Vector2(speed * DirX, rig.linearVelocity.y);

            rig.gravityScale = Game.GRAVITY;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == Layer.AiDecisionZone)
            decisionZonesNearby.Add(col.transform);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.layer == Layer.AiDecisionZone && !decisionZonesNearby.Contains(col.transform))
            decisionZonesNearby.Add(col.transform);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == Layer.AiDecisionZone)
            decisionZonesNearby.Remove(col.transform);
    }
}
