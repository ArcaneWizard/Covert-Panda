using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Health : MonoBehaviour
{
    public int maxHP { get; protected set; }
    public int currentHP { get; protected set; }
    public bool isDead { get; private set; }
    private int paddingHP = 15; //means the bar for hp should always "appear" as if 15/300 hp or more is left 

    protected float respawnTime = 4.22f;
    private Transform respawnLocations;

    protected Image hpBar;
    protected Vector2 hpBarOffset;

    protected int bulletLayer;

    protected CentralWeaponSystem weaponSystem;
    protected CentralController controller;
    protected Ragdolling ragdolling;
    protected CentralAbilityHandler abilityHandler;
    protected Side side;

    protected Rigidbody2D rig;
    protected BoxCollider2D mainCollider;
    protected BoxCollider2D hitBox;

    public virtual void Awake()
    {
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        controller = transform.GetComponent<CentralController>();
        ragdolling = transform.GetComponent<Ragdolling>();
        abilityHandler = transform.GetComponent<CentralAbilityHandler>();

        rig = transform.GetComponent<Rigidbody2D>();
        mainCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();
        hitBox = transform.GetChild(1).GetComponent<BoxCollider2D>();

        side = transform.parent.GetComponent<Role>().side;
        hitBox.gameObject.layer = (side == Side.Friendly) ? Layers.FriendlyHitBox : Layers.EnemyHitBox;

        hpBar = transform.parent.GetChild(2).GetChild(0).GetChild(1).GetComponent<Image>();
        hpBarOffset = hpBar.transform.parent.GetComponent<RectTransform>().position - transform.position;

        respawnLocations = (side == Side.Friendly)
            ? transform.parent.parent.parent.GetComponent<References>().FriendRespawnPoints
            : transform.parent.parent.parent.GetComponent<References>().EnemyRespawnPoints;
    }

    private void Start()
    {
        currentHP = maxHP;
        isDead = false;
        hpBar.color = (side == Side.Friendly) ? new Color32(0, 166, 255, 255) : new Color32(204, 57, 62, 255);

        hitBox.offset = new Vector2(0, -0.15f);
        hitBox.size = new Vector2(0.13f, 2.48f);

        StartCoroutine(abilityHandler.enableSpawnProtection());
    }

    // checks for when the entity collides with a bullet or explosion. apply dmg
    // and update health bar correspondingly
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (isDead || abilityHandler.IsInvulnerable)
            return;

        if (col.gameObject.layer == bulletLayer)
            hitByBullet(col.transform);

        else if (col.gameObject.layer == Layers.Explosion)
            hitByExplosion(col.transform);
    }

    // Helper method. If this entity collides with a bullet and that bullet hasn't made contact with 
    // anyone yet, reduce this entity's hp by the dmg that the particular bullet does. Also register that  
    // the bullet has smade contact / done damage to someone + trigger any bullet action if needbe (ex. explosion) 
    private void hitByBullet(Transform physicalBullet)
    {
        Bullet bullet = physicalBullet.GetComponent<Bullet>();

        if (!bullet.disableCollisionDetection)
        {
            bullet.disableCollisionDetection = true;
            currentHP -= bullet.Damage();
            bullet.OnCreatureEnter(transform);

            if (currentHP <= 0)
            {
                Transform killer = physicalBullet.parent.parent.parent.parent;
                Stats.confirmKillFor(killer);
                StartCoroutine(startDeathSequence());
            }
        }
    }

    // Helper method for when this entity collides with an explosion. Check if 
    // this enemy has already been hurt by said explosion. If not, only then take damage
    // and that too based off the distance from the center of the explosion
    private void hitByExplosion(Transform explosionCollider)
    {
        Explosion explosion = explosionCollider.parent.transform.GetComponent<Explosion>();
        int id = gameObject.GetHashCode();

        if (!explosion.wasEntityAlreadyHurt(id))
        {
            explosion.updateEntitiesHurt(id);
            currentHP -= explosion.damageBasedOffDistance(transform);

            if (currentHP <= 0)
            {
                Transform killer = explosion.transform.parent.parent.parent.parent;
                Stats.confirmKillFor(killer);
                StartCoroutine(startDeathSequence());
            }
        }
    }

    // every frame, update the hp bar to reflect the entity's current hp. also, fix the hp
    // bar position above the entity's head as it moves
    private void Update()
    {
        if (isDead)
            return;

        // for debugging purposes (auto kills everyone)
        if (Input.GetKeyDown(KeyCode.X))
        {
            currentHP = 0;
            StartCoroutine(startDeathSequence());
        }

        hpBar.fillAmount = (float)(currentHP + paddingHP) / (float)maxHP;
        hpBar.transform.parent.GetComponent<RectTransform>().position = hpBarOffset + new Vector2(transform.position.x, transform.position.y);
    }

    private IEnumerator startDeathSequence()
    {
        UponDying();

        yield return new WaitForSeconds(respawnTime);
        BeforeRespawning();

        yield return new WaitForSeconds(Time.deltaTime);
        hpBar.transform.parent.gameObject.SetActive(true);
        controller.UpdateTiltInstantly();

        StartCoroutine(abilityHandler.enableSpawnProtection());
    }

    protected virtual void UponDying()
    {
        isDead = true;
        currentHP = -paddingHP;
        hpBar.transform.parent.gameObject.SetActive(false);

        hitBox.enabled = false;
        mainCollider.enabled = false;

        StartCoroutine(ragdolling.Enable());
        Stats.confirmDeathFor(transform.parent);
    }

    protected virtual void BeforeRespawning()
    {
        currentHP = maxHP;
        isDead = false;

        hitBox.enabled = true;
        mainCollider.enabled = true;

        ragdolling.Disable();
        weaponSystem.InitializeWeaponSystem();

        Transform respawnLocation = respawnLocations.GetChild(
            UnityEngine.Random.Range(0, respawnLocations.childCount));
        transform.position = respawnLocation.position;

        transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    public void TakeDamageFromPredictedFastBulletCollision(int damage, Transform physicalBullet)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Transform killer = physicalBullet.parent.parent.parent.parent;
            Stats.confirmKillFor(killer);
            StartCoroutine(startDeathSequence());
        }
    }
}
