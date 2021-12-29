using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Health : MonoBehaviour
{
    public int maxHP { get; protected set; }
    public int currentHP { get; protected set; }

    protected int friendlyBulletLayer = 13;
    protected int friendlyExplosionLayer = 10;
    protected int enemyBulletLayer = 17;
    protected int enemyExplosionLayer = 15;

    protected int bulletLayer;
    protected int explosionLayer;

    private CentralWeaponSystem weaponSystem;

    public virtual void Awake() => transform.GetComponent<CentralWeaponSystem>();

    // if this entity collides with a bullet and that bullet hasn't made contact with anything, 
    // reduce hp by the amount of dmg that particular bullet does. Also register that the bullet 
    // has made contact / done damage to this entity
    private void OnTriggerEnter2D(Collider2D col)
    {
        Bullet bullet = col.transform.GetComponent<Bullet>();

        if (col.gameObject.layer == bulletLayer && !bullet.madeContact)
        {
            string weaponID = col.transform.parent.GetComponent<WeaponTag>().Tag;
            currentHP -= weaponSystem.getWeaponConfiguration(weaponID).bulletDmg;
            bullet.OnEntityEnter(transform);
            bullet.madeContact = true;
        }

        if (col.gameObject.layer == explosionLayer)
        {

        }
    }
}
