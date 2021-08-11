using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralWeaponAttacks : MonoBehaviour
{
    protected float grenadeThrowForce = 2200;
    protected float grenadeYForce = -20;
    protected float boomerangSpeed = 52;
    protected Vector2 boomerangSpinSpeed = new Vector2(600, 1050);
    protected float sniperBulletSpeed = 100;
    protected float shielderBulletSpeed = 45;
    protected float shotgunBulletSpeed = 52;

    protected float scytheThrowSpeed = 60;
    protected Vector2 scytheSpinSpeed = new Vector2(1200, 1400);

    public List<Transform> goldenShotgunBits = new List<Transform>();
    protected int shotgunBitCounter = 0;
    protected Vector2 goldenBitsForceX = new Vector2(-50, 70), goldenBitsForceY = new Vector2(0, 290);
    protected float goldenShotgunSpread = 22;

    protected List<GameObject> bullets = new List<GameObject>();

    [HideInInspector]
    public Animator armAnimator;
    public GameObject scytheTracker;

    protected CentralShooting shooting;
    protected CentralWeaponSystem weaponSystem;

    [HideInInspector]
    public bool disableAiming, shouldLetGoOfObject, attackAnimationPlaying;

    protected Rigidbody2D ammunitionRig;
    protected GameObject ammunition = null;

    public void Awake()
    {
        shooting = transform.GetComponent<CentralShooting>();
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        armAnimator = transform.GetComponent<Animator>();
    }

    public void weaponAttack()
    {
        retrieveSomeAmmunition();
        string weapon = weaponSystem.weaponSelected;

        if (weapon == "Grenade")
            StartCoroutine(throwGrenade());
        else if (weapon == "Boomerang")
            StartCoroutine(throwBoomerang());
        else if (weapon == "Plasma Orb")
            StartCoroutine(throwPlasmaOrb());
        else if (weapon == "Sniper")
            shootBulletInStraightLine(sniperBulletSpeed);
        else if (weapon == "Shielder")
            shootBulletInStraightLine(shielderBulletSpeed);
        else if (weapon == "Shotgun")
            shootBulletInArcWithParticles();
        else
            Debug.LogError("You haven't specified how to shoot this particular object");
    }

    public void meeleeAttack()
    {
        string weapon = weaponSystem.weaponSelected;

        if (weapon == "Scythe")
            StartCoroutine(attackWithScythe());
        else
            Debug.LogError("You haven't specified how to shoot this particular object");
    }

    private IEnumerator throwGrenade()
    {
        //Help track the object on the hand and confirm when the "ammunition" can be let go off / force applied
        StartCoroutine(shooting.HandleThrowingMechanics(ammunition, 0.1f, 0.22f));
        armAnimator.SetInteger("Arms Phase", 1);

        while (!shouldLetGoOfObject)
            yield return null;

        //apply a large force to throw the grenadeaa
        Vector2 unadjustedForce = grenadeThrowForce * shooting.aimDir * new Vector2(1.2f, 1) + new Vector2(0, grenadeYForce);
        ammunitionRig.velocity = new Vector2(0, 0);
        ammunitionRig.AddForce(unadjustedForce * ammunitionRig.mass);
    }

    private IEnumerator throwPlasmaOrb()
    {
        //Help track the object on the hand, and confirm when the "ammunition" can be let go off / force applied
        StartCoroutine(shooting.HandleThrowingMechanics(ammunition, 0.1f, 0.22f));
        armAnimator.SetInteger("Arms Phase", 1);

        while (!shouldLetGoOfObject)
            yield return null;

        //apply a large force to throw the grenade
        Vector2 unadjustedForce = grenadeThrowForce * shooting.aimDir * new Vector2(1.2f, 1) + new Vector2(0, grenadeYForce);
        ammunitionRig.velocity = new Vector2(0, 0);
        ammunitionRig.AddForce(unadjustedForce * ammunitionRig.mass);
    }

    private void shootBulletInStraightLine(float speed)
    {
        //Get aim direction and spawn ammo at the right gun tip
        shooting.SpawnGunBulletAndAim(ammunition);

        //spawn and orient the bullet correctly
        ammunition.transform.right = shooting.aimDir;
        ammunitionRig.velocity = shooting.aimDir * speed;
    }

    private IEnumerator throwBoomerang()
    {
        //Help track the object on the hand, and confirm when the "ammunition" can be let go off / force applied
        StartCoroutine(shooting.HandleThrowingMechanics(ammunition, 0.1f, 0.2f));
        armAnimator.SetInteger("Arms Phase", 1);

        while (!shouldLetGoOfObject)
            yield return null;

        ammunition.transform.GetComponent<Animator>().SetBool("glare", false);
        ammunitionRig.velocity = shooting.aimDir * boomerangSpeed;
        ammunitionRig.angularVelocity = Random.Range(boomerangSpinSpeed.x, boomerangSpinSpeed.y) * (Random.Range(0, 2) * 2 - 1);
    }

    public void curveBoomerang()
    {
        if (weaponSystem.weaponSelected == "Boomerang" && ammunition && ammunition.activeSelf)
        {
            if (shooting.aimDir.x >= 0)
                ammunitionRig.velocity = Quaternion.Euler(0, 0, -90) * shooting.aimDir * boomerangSpeed;
            else
                ammunitionRig.velocity = Quaternion.Euler(0, 0, 90) * shooting.aimDir * boomerangSpeed;

            ammunitionRig.angularVelocity = Random.Range(boomerangSpinSpeed.x, boomerangSpinSpeed.y) * (Random.Range(0, 2) * 2 - 1);
            ammunition.transform.GetComponent<Animator>().SetBool("glare", true);
        }
    }

    public IEnumerator throwScythe()
    {
        if (weaponSystem.weaponSelected == "Scythe")
        {
            retrieveSomeAmmunition();
            StartCoroutine(shooting.HandleThrowableBladeMechanics(ammunition, 0.04f, 0.22f));
            armAnimator.SetInteger("Arms Phase", 11);

            while (!shouldLetGoOfObject)
                yield return null;

            ammunitionRig.velocity = shooting.aimDir * scytheThrowSpeed;
            ammunitionRig.angularVelocity = Random.Range(scytheSpinSpeed.x, scytheSpinSpeed.y) * Mathf.Sign(-shooting.aimDir.x);
        }
    }


    private IEnumerator attackWithScythe()
    {
        armAnimator.SetInteger("Arms Phase", 11);
        attackAnimationPlaying = true;
        disableAiming = true;

        while (armAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "swinging scythe" ||
        armAnimator.GetInteger("Arms Phase") == 11)
            yield return null;

        disableAiming = false;
        yield return new WaitForSeconds(0.04f);
        scytheTracker.SetActive(true);
    }

    private void shootBulletInArcWithParticles()
    {
        //shoot three golden shotgun bullets with spread
        shooting.SpawnGunBulletAndAim(ammunition);
        ammunition.transform.right = shooting.aimDir;
        ammunitionRig.velocity = shooting.aimDir * shotgunBulletSpeed;

        retrieveSomeAmmunition();
        shooting.SpawnGunBulletAndAim(ammunition);
        ammunition.transform.right = Quaternion.AngleAxis(goldenShotgunSpread, Vector3.forward) * shooting.aimDir;
        ammunitionRig.velocity = Quaternion.AngleAxis(goldenShotgunSpread, Vector3.forward) * shooting.aimDir * shotgunBulletSpeed;

        retrieveSomeAmmunition();
        shooting.SpawnGunBulletAndAim(ammunition);
        ammunition.transform.right = Quaternion.AngleAxis(-goldenShotgunSpread, Vector3.forward) * shooting.aimDir;
        ammunitionRig.velocity = Quaternion.AngleAxis(-goldenShotgunSpread, Vector3.forward) * shooting.aimDir * shotgunBulletSpeed;

        //explode golden shotgun dust everywhere
        for (int i = shotgunBitCounter; i < shotgunBitCounter + goldenShotgunBits.Count / 2; i++)
        {
            goldenShotgunBits[i].gameObject.SetActive(true);
            goldenShotgunBits[i].transform.right = shooting.aimDir;
            goldenShotgunBits[i].position = shooting.bulletSpawnPoint.position;
            goldenShotgunBits[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(
                Random.Range(goldenBitsForceX.x, goldenBitsForceY.y) * Mathf.Sign(shooting.aimDir.x),
                Random.Range(goldenBitsForceY.x, goldenBitsForceY.y))
            );
        }

        shotgunBitCounter = (shotgunBitCounter + goldenShotgunBits.Count / 2) % goldenShotgunBits.Count;
    }

    private void retrieveSomeAmmunition()
    {
        ammunition = weaponSystem.getWeapon();
        weaponSystem.useOneAmmo();
        ammunitionRig = ammunition.transform.GetComponent<Rigidbody2D>();
    }
}
