using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Health : MonoBehaviour
{
    public int maxHP { get; protected set; }
    public int currentHP { get; protected set; }

    protected Transform healthBar;
    protected Color normal;
    protected Color injured;

    private SpriteRenderer healthUnit;
    private Queue<GameObject> injuredHealthUnits;
    private float delayTimer;
    private float delayTillFade = 0.6f;
    private float disappearRate = 0.05f;

    protected int bulletLayer;
    protected int explosionLayer;

    private CentralWeaponSystem weaponSystem;

    public virtual void Awake() 
    {
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        healthBar = transform.GetChild(1); 

        injuredHealthUnits = new Queue<GameObject>();
        normal = healthBar.GetChild(1).GetComponent<SpriteRenderer>().color;
        injured = new Color32(255, 0, 0, 199);
    }

    private void Start() => updateHealthBar();

    // checks for when the entity collides with a bullet or explosion. apply dmg
    // and update health bar correspondingly
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == bulletLayer) {
            bulletCollision(col.transform);
            updateHealthBar();
        }

        else if (col.gameObject.layer == explosionLayer) {
            explosionCollision(col.transform);
            updateHealthBar();
        }
    }

    // Helper method. If this entity collides with a bullet and that bullet hasn't made contact with 
    // anyone yet, reduce this entity's hp by the dmg that the particular bullet does. Also register that  
    // the bullet has made contact / done damage to someone + trigger any bullet action if needbe (ex. explosion) 
    private void bulletCollision(Transform physicalBullet)
    {
        Bullet bullet = physicalBullet.GetComponent<Bullet>();

        if (!bullet.madeContact)
        {
            string weaponType = physicalBullet.parent.GetComponent<WeaponTag>().Tag;
            currentHP -= weaponSystem.getWeaponConfiguration(weaponType).bulletDmg;

            bullet.madeContact = true;
            bullet.OnEntityEnter(transform);
        }
    } 

    // Helper method for when this entity collides with an explosion. Check if 
    // this enemy has already been hurt by said explosion. If not, only then take damage
    // and that too based off the distance from the center of the explosion
    private void explosionCollision(Transform explosionCollider)
    {
        Explosion explosion = explosionCollider.parent.transform.GetComponent<Explosion>();
        int id = gameObject.GetHashCode();

        if (!explosion.wasEntityAlreadyHurt(id)) 
        {
            explosion.updateEntitiesHurt(id);
            currentHP -= explosion.damageBasedOffDistance(transform);
        }
    }

    private void updateHealthBar() 
    {
        if (currentHP < 0)
            currentHP = 0;

        int hp = (int) Mathf.Ceil((float) currentHP / maxHP * 11f);
        
        for (int i = hp; i >= 1; i--) 
            healthBar.GetChild(i).gameObject.SetActive(true);

        for (int j = 11; j >= hp + 1; j--) 
        {
            healthUnit = healthBar.GetChild(j).GetComponent<SpriteRenderer>();
            if (healthUnit.color == normal)
            {
                if (injuredHealthUnits.Count == 0)
                    delayTimer = delayTillFade;

                healthUnit.color = injured;
                injuredHealthUnits.Enqueue(healthUnit.gameObject);
            }
        }
    }

    public virtual void Update() 
    {
        if (injuredHealthUnits.Count > 0 && delayTimer <= 0) 
        {
            injuredHealthUnits.Dequeue().SetActive(false);
            delayTimer = disappearRate;
        }

        if (delayTimer > 0)
            delayTimer -= Time.deltaTime;
    }
}
