using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Health : MonoBehaviour
{
    public int maxHP { get; protected set; }
    public int currentHP { get; protected set; }
    public bool isDead { get; protected set; }
    private int paddingHP = 15; //means the bar for hp should always "appear" as if 15/300 hp or more is left 

    protected float respawnTime = 5.22f;
    protected float respawnInvulnerabilityDuration = 2f;
    public Transform respawnLocations;

    protected Image hpBar;
    protected Vector2 hpBarOffset;

    protected int bulletLayer;

    protected CentralWeaponSystem weaponSystem;
    protected CentralController controller;
    protected Ragdolling ragdolling;
    protected Rigidbody2D rig;
    protected Collider2D hitBox;

    public virtual void Awake()
    {
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        controller = transform.GetComponent<CentralController>();
        ragdolling = transform.GetComponent<Ragdolling>();
        rig = transform.GetComponent<Rigidbody2D>();
        hitBox = transform.GetChild(1).GetComponent<Collider2D>();

        Side side = transform.parent.GetComponent<Role>().side;
        hitBox.gameObject.layer = (side == Side.Friendly) ? Layers.friendlyHitBox : Layers.enemyHitBox;

        hpBar = transform.parent.GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>();
        hpBarOffset = hpBar.transform.parent.GetComponent<RectTransform>().position - transform.position;
    }

    private void Start()
    {
        currentHP = maxHP;
        isDead = false;
    }

    // checks for when the entity collides with a bullet or explosion. apply dmg
    // and update health bar correspondingly
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (isDead)
            return;

        if (col.gameObject.layer == bulletLayer)
            hitByBullet(col.transform);

        else if (col.gameObject.layer == Layers.explosion)
            hitByExplosion(col.transform);
    }

    // Helper method. If this entity collides with a bullet and that bullet hasn't made contact with 
    // anyone yet, reduce this entity's hp by the dmg that the particular bullet does. Also register that  
    // the bullet has made contact / done damage to someone + trigger any bullet action if needbe (ex. explosion) 
    public void hitByBullet(Transform physicalBullet)
    {
        Bullet bullet = physicalBullet.GetComponent<Bullet>();

        if (!bullet.disabledImpactDetection) 
        {
            currentHP -= bullet.Damage();
            bullet.ConfirmImpactWithCreature(transform);
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
        }
    }

    // every frame, update the hp bar to reflect the entity's current hp. also, fix the hp
    // bar position above the entity's head as it moves
    private void Update()
    {
        if (isDead)
            return;

        if (Input.GetKeyDown(KeyCode.X))
            currentHP = -20;

        if (currentHP <= 0)
        {
            isDead = true;
            StartCoroutine(ragdolling.Enable());

            currentHP = -paddingHP;
            hpBar.transform.parent.gameObject.SetActive(false);
            hitBox.enabled = false;

            StartCoroutine(CallUponDying());
        }

        hpBar.fillAmount = (float)(currentHP + paddingHP) / (float)maxHP;
        hpBar.transform.parent.GetComponent<RectTransform>().position = hpBarOffset + new Vector2(transform.position.x, transform.position.y);
    }

    public void TakeDamage(int damage) => currentHP -= damage;
    public virtual IEnumerator CallUponDying() => null;
}
