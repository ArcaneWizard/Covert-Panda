using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using static Validation;

/* Abstract class that manages the creature's health. Also tracks whether or not the creature is dead. */

public abstract class Health : MonoBehaviour
{
    public int maxHP { get; protected set; }
    public int currentHP { get; protected set; }
    public bool IsDead { get; private set; }

    // The HP bar is padded slightly so low hp doesn't make the bar look empty
    private const float PERCENT_OF_PADDED_HP = 2.5f;
    private float paddedHp;

    protected Image hpBar;
    protected RectTransform hpBarRect;
    protected Vector2 hpBarOffset;

    protected BoxCollider2D hitBox;
    private CentralDeathSequence deathSequence;
    private Side side;

    ///<summary> Damage this creature. Takes in a base dmg and the attacker responsible (if known) </summary>
    public virtual void InflictDamage(int baseDmg, Transform attacker = null)
    {
        if (IsDead)
            return;

        currentHP -= DamageCalculator.CalculateDmg(baseDmg, transform, attacker);

        if (currentHP <= 0 && attacker && !transform.Equals(attacker))
            Stats.Instance.ConfirmKillFor(attacker);
    }

    protected virtual void Awake()
    {
        deathSequence = transform.GetComponent<CentralDeathSequence>();
        hitBox = transform.GetChild(1).GetComponent<BoxCollider2D>();
        hpBar = transform.parent.GetChild(2).GetChild(0).GetChild(1).GetComponent<Image>();
        hpBarRect = hpBar.transform.parent.GetComponent<RectTransform>();
        var role = transform.parent.GetComponent<Role>();

        this.Validate(
            new NotNull(deathSequence, nameof(deathSequence)),
            new RequiredTag(hitBox, nameof(hitBox), "hitBox"),
            new NotNull(role, nameof(role)),
            new RequiredTag(hpBar, nameof(hpBar), "hpBar")
         );

        hpBarOffset = hpBar.transform.parent.GetComponent<RectTransform>().position - transform.position;
        side = role.side;
    }

    protected virtual void Start()
    {
        currentHP = maxHP;
        paddedHp = currentHP * (1f + PERCENT_OF_PADDED_HP / 100f);
        IsDead = false;
        hpBar.color = (side == Side.Friendly) ? new Color32(0, 166, 255, 255) : new Color32(204, 57, 62, 255);

        hitBox.gameObject.layer = (side == Side.Friendly) ? Layer.EnemyHitBox : Layer.EnemyHitBox;
        hitBox.offset = new Vector2(0, -0.15f);
        hitBox.size = new Vector2(0.13f, 2.48f);
    }

    void Update()
    {
        if (IsDead)
            return;

        if (currentHP <= 0 || Input.GetKeyDown(KeyCode.K))
        {
            IsDead = true;
            currentHP = -((int)paddedHp+1);
            hpBar.transform.parent.gameObject.SetActive(false);
            StartCoroutine(deathSequence.Initiate());
        }

        hpBar.fillAmount = (float)(currentHP + paddedHp) / (float)maxHP;
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

    void OnDestroy()
    {
        deathSequence.RightBeforeRespawning -= resetHealthWhenRespawning;
        deathSequence.UponRespawning -= enableHealthBar;
    }


    private void resetHealthWhenRespawning()
    {
        currentHP = maxHP;
        IsDead = false;
    }

    private void enableHealthBar()
    {
        hpBar.transform.parent.gameObject.SetActive(true);
    }
}
