using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Health : MonoBehaviour
{
    public int maxHP { get; protected set; }
    public int currentHP { get; protected set; }

    protected int bulletLayer;
    protected int explosionLayer;

    private CentralWeaponSystem weaponSystem;

    public virtual void Awake() => weaponSystem = transform.GetComponent<CentralWeaponSystem>();

    // checks for when the entity collides with a bullet or explosion
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == bulletLayer)
            bulletCollision(col.transform);
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
}
