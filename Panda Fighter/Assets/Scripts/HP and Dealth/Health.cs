using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Abstract class that manages the creature's health and kicks off actions when
 * the creature dies. Handles respawning too. */

public abstract class Health : MonoBehaviour
{
    public int maxHP { get; protected set; }
    public int currentHP { get; protected set; }
    public bool isDead { get; private set; }

    // pads the hp bar slightly so that low hp doesn't make the hp bar already look empty
    private int paddingHP; 

    protected Image hpBar;
    protected RectTransform hpBarRect;
    protected Vector2 hpBarOffset;

    protected int bulletLayer;
    protected CentralAbilityHandler abilityHandler;
    private CentralDeathSequence deathSequence;
    private Side side;

    protected Rigidbody2D rig;
    protected BoxCollider2D hitBox;

    protected virtual void Awake()
    {
        abilityHandler = transform.GetComponent<CentralAbilityHandler>();
        deathSequence = transform.GetComponent<CentralDeathSequence>();

        rig = transform.GetComponent<Rigidbody2D>();
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
        isDead = false;
        hpBar.color = (side == Side.Friendly) ? new Color32(0, 166, 255, 255) : new Color32(204, 57, 62, 255);

        hitBox.offset = new Vector2(0, -0.15f);
        hitBox.size = new Vector2(0.13f, 2.48f);
    }

    // Damage this creature and kick off the death sequence if required.
    // Optionally takes in the attacker who damaged this creature (if known)
    public void TakeDamage(int damage, Transform attacker = null)
    {
        if (isDead)
            return;

        currentHP -= damage;

        if (currentHP <= 0)
        {
            if (attacker)
                Stats.ConfirmKillFor(attacker);

            isDead = true;
            currentHP = -paddingHP;
            hpBar.transform.parent.gameObject.SetActive(false);
            StartCoroutine(deathSequence.Initiate());
        }
    }

    // Checks if the creature collided with a bullet or explosion
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (isDead || abilityHandler.IsInvulnerable)
            return;

        if (col.gameObject.layer == bulletLayer)
            hitByBullet(col.transform);

        else if (col.gameObject.layer == Layer.Explosion)
            hitByExplosion(col.transform);
    }

    // Takes damage from a bullet
    private void hitByBullet(Transform physicalBullet)
    {
        Bullet bullet = physicalBullet.GetComponent<Bullet>();

        if (!bullet.HasHitCreature)
        {
            bullet.ConfirmCreatureHit(transform);

            Transform attacker = physicalBullet.parent.parent.parent.parent;
            TakeDamage(bullet.Damage(), attacker);
        }
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
    private void Update()
    {
        if (isDead)
            return;

        hpBar.fillAmount = (float)(currentHP + paddingHP) / (float)maxHP;
        hpBarRect.position = hpBarOffset + new Vector2(transform.position.x, transform.position.y);
    }

    void OnEnable()
    {
        deathSequence.ActionsTriggeredWhenRespawning += resetHealthWhenRespawning;
        deathSequence.ActionsTriggeredAfterRespawning += enableHealthBar;
    }

    void OnDisable()
    {
        deathSequence.ActionsTriggeredWhenRespawning -= resetHealthWhenRespawning;
        deathSequence.ActionsTriggeredAfterRespawning -= enableHealthBar;
    }

    private void resetHealthWhenRespawning()
    {
        currentHP = maxHP;
        isDead = false;
    }

    public void enableHealthBar()
    {
        hpBar.transform.parent.gameObject.SetActive(true);
    }

}
