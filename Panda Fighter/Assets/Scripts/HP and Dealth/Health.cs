using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Health : MonoBehaviour
{
    public int maxHP { get; protected set; }
    public int currentHP { get; protected set; }

    private Image hpBar;
    private Vector2 hpBarOffset;

    protected int bulletLayer;
    protected int explosionLayer;

    private CentralWeaponSystem weaponSystem;

    public virtual void Awake() 
    {
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        hpBar = transform.parent.GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>();
        hpBarOffset = hpBar.transform.parent.GetComponent<RectTransform>().position - transform.position;
    }

    private void Start() => currentHP = maxHP;

    // checks for when the entity collides with a bullet or explosion. apply dmg
    // and update health bar correspondingly
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == bulletLayer) 
            bulletCollision(col.transform);

        else if (col.gameObject.layer == explosionLayer) 
            explosionCollision(col.transform);
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

    // every frame, update the hp bar to reflect the entity's current hp. also, fix the hp
    // bar position above the entity's head as it moves
    private void Update() 
    {
        if (currentHP < 0)
            currentHP = 0;

        hpBar.fillAmount = (float) currentHP / (float) maxHP;
        hpBar.transform.parent.GetComponent<RectTransform>().position = hpBarOffset + new Vector2(transform.position.x, transform.position.y);
    }
}
