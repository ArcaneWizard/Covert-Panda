using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralShooting : MonoBehaviour
{
    [Header("Entities")]
    public Camera camera;
    [HideInInspector]
    public Transform shootingArm;

    protected Rigidbody2D rig;
    [HideInInspector]
    public Vector2 aimDir;
    protected float wait;

    public GameObject weaponHeld = null;
    protected GameObject weaponThrown;
    protected GameObject lastBoomerangThrown;

    public string combatMode = "gun";

    [HideInInspector]
    public float timeLeftBtwnShots;

    [HideInInspector]
    public Transform bulletSpawnPoint;

    protected CentralWeaponAttacks weaponAttacks;
    protected CentralWeaponSystem weaponSystem;

    public void Awake()
    {
        weaponAttacks = transform.GetComponent<CentralWeaponAttacks>();
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        rig = transform.GetComponent<Rigidbody2D>();
    }

    private void LateUpdate()
    {
        //the grenade or holdable weapon stays in your palm during normal animation cycles until its thrown
        if (combatMode == "handheld")
        {
            if (!GameObject.Equals(weaponHeld, weaponThrown) && weaponSystem.getAmmo() > 0)
            {
                weaponHeld.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                weaponHeld.transform.GetComponent<Collider2D>().isTrigger = true;

                weaponHeld.transform.position = bulletSpawnPoint.position;
                weaponHeld.transform.rotation = bulletSpawnPoint.rotation;
                weaponHeld.SetActive(true);
            }
            else
                weaponHeld = weaponSystem.getWeapon();
        }
    }

    //Get direction to aim in, and spawn the ammunition at the right gun tip
    public void SpawnGunBulletAndAim(GameObject ammunition)
    {
        aimDir = calculateAimDirection();

        ammunition.transform.position = bulletSpawnPoint.position;
        resetAmmunitionSettingsWhenSpawned(ammunition);
    }

    //Get direction to aim in, help track the object on the hand, and confirm when the "ammunition" can be let go off / force applied
    public IEnumerator HandleThrowingMechanics(GameObject ammunition, float trackingMultiplier, float trackingOffset)
    {
        aimDir = calculateAimDirection();
        wait = ((-aimDir.y + 1) * trackingMultiplier + trackingOffset);

        weaponAttacks.shouldLetGoOfObject = false;
        weaponAttacks.attackAnimationPlaying = true;
        yield return new WaitForSeconds(wait);

        ammunition.transform.position = bulletSpawnPoint.position;
        weaponThrown = ammunition;

        resetAmmunitionSettingsWhenSpawned(ammunition);
        weaponAttacks.shouldLetGoOfObject = true;
    }

    //Get direction to aim in, help track the object on the hand, and confirm when the "ammunition" can be let go off / force applied
    public IEnumerator HandleThrowableBladeMechanics(GameObject ammunition, float trackingMultiplier, float trackingOffset)
    {
        aimDir = calculateAimDirection();
        wait = ((-aimDir.y + 1) * trackingMultiplier + trackingOffset);

        weaponAttacks.shouldLetGoOfObject = false;
        weaponAttacks.attackAnimationPlaying = true;
        yield return new WaitForSeconds(wait);

        WeaponConfig config = weaponSystem.weaponConfigurations[weaponSystem.weaponSelected];
        ammunition.transform.position = config.weapon.transform.position;
        ammunition.transform.localEulerAngles = config.weapon.transform.localEulerAngles;
        ammunition.transform.localScale = config.weapon.transform.localScale;
        config.weapon.gameObject.SetActive(false);

        resetAmmunitionSettingsWhenSpawned(ammunition);
        weaponAttacks.shouldLetGoOfObject = true;
    }

    private void resetAmmunitionSettingsWhenSpawned(GameObject ammunition)
    {
        ammunition.transform.GetComponent<Collider2D>().isTrigger = false;
        ammunition.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        ammunition.transform.GetComponent<Rigidbody2D>().angularVelocity = 0;
        ammunition.SetActive(false);
        ammunition.SetActive(true);
        ammunition.transform.localEulerAngles = new Vector3(0, 0, ammunition.transform.localEulerAngles.z);
    }

    private Vector2 calculateAimDirection()
    {
        return (Input.mousePosition - camera.WorldToScreenPoint(shootingArm.position)).normalized;
    }

}
