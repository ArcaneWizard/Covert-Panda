using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Abstract class that manages the creature's health. Also tracks whether or not the creature is dead. */

public abstract class Health : MonoBehaviour
{
    public int maxHP { get; protected set; }
    public int currentHP { get; protected set; }
    public bool IsDead { get; private set; }

    // pads the hp bar slightly so that low hp doesn't make the hp bar look empty
    private int paddingHP; 

    protected Image hpBar;
    protected RectTransform hpBarRect;
    protected Vector2 hpBarOffset;

    protected int bulletLayer;
    protected CentralAbilityHandler abilityHandler;
    private CentralDeathSequence deathSequence;
    private Side side;

    protected BoxCollider2D hitBox;

    protected virtual void Awake()
    {
        abilityHandler = transform.GetComponent<CentralAbilityHandler>();
        deathSequence = transform.GetComponent<CentralDeathSequence>();

        hitBox = transform.GetChild(1).GetComponent<BoxCollider2D>();

        side = transform.parent.GetComponent<Role>().side;
        hitBox.gameObject.layer = (side == Side.Friendly) ? Layer.FriendlyHitBox : Layer.EnemyHitBox;

        hpBar = transform.parent.GetChild(2).GetChild(0).GetChild(1).GetComponent<Image>();
        hpBarRect = hpBar.transform.parent.GetComponent<RectTransform>();
        hpBarOffset = hpBar.transform.parent.GetComponent<RectTransform>().position - transform.position;
    }

    protected virtual void Start()
    {
        currentHP = maxHP;
        paddingHP = (int)(2.5f * maxHP / 100f);
        IsDead = false;
     // hpBar.color = (side == Side.Friendly) ? new Color32(0, 166, 255, 255) : new Color32(204, 57, 62, 255);

        hitBox.offset = new Vector2(0, -0.15f);
        hitBox.size = new Vector2(0.13f, 2.48f);
    }

    // Damage this creature
    // Optionally takes in who damaged this creature (if known)
    public void TakeDamage(int damage, Transform attacker = null)
    {
        if (IsDead || currentHP <= 0 || abilityHandler.IsInvulnerable)
            return;

        currentHP -= damage;

        if (currentHP <= 0 && attacker)
            Stats.ConfirmKillFor(attacker);
    }

    // Checks if the creature collided with a bullet or explosion
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (IsDead || abilityHandler.IsInvulnerable)
            return;

        else if (col.gameObject.layer == Layer.Explosion)
            hitByExplosion(col.transform);
    }

    // Takes damage from an explosion
    private void hitByExplosion(Transform explosionCollider)
    {
        Explosion explosion = explosionCollider.parent.transform.GetComponent<Explosion>();
        int id = gameObject.GetHashCode();

        if (!explosion.wasEntityAlreadyHurt(id))
        {
            explosion.updateEntitiesHurt(id);

            Transform attacker = explosion.transform.parent.parent.parent.parent;
            TakeDamage(explosion.GetDamageBasedOffDistance(transform), attacker);
        }
    }

    // Every frame, update the hp bar to reflect the entity's current hp. Also, fix the hp
    // bar position above the entity's head as it moves
    void Update()
    {
        if (IsDead)
            return;

        if (currentHP <= 0 || Input.GetKeyDown(KeyCode.K))
        {
            IsDead = true;
            currentHP = -paddingHP;
            hpBar.transform.parent.gameObject.SetActive(false);
            StartCoroutine(deathSequence.Initiate());
        }

        hpBar.fillAmount = (float)(currentHP + paddingHP) / (float)maxHP;
        hpBarRect.position = hpBarOffset + new Vector2(transform.position.x, transform.position.y);
    }

    void OnEnable()
    {
        deathSequence.RightBeforeRespawning += resetHealthWhenRespawning;
        deathSequence.UponRespawning += enableHealthBar;
    }

    void OnDisable()
    {
        deathSequence.RightBeforeRespawning -= resetHealthWhenRespawning;
        deathSequence.UponRespawning -= enableHealthBar;
    }

    private void resetHealthWhenRespawning()
    {
        currentHP = maxHP;
        IsDead = false;
    }

    public void enableHealthBar()
    {
        hpBar.transform.parent.gameObject.SetActive(true);
    }

}
