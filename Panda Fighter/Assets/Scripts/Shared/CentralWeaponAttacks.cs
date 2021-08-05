using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralWeaponAttacks : MonoBehaviour
{
    protected float grenadeThrowForce = 2200;
    protected float grenadeYForce = -20;
    protected float boomerangSpeed = 45;
    protected Vector2 boomerangSpinSpeed = new Vector2(600, 750);
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
    protected List<Rigidbody2D> objectRigs = new List<Rigidbody2D>();

    [HideInInspector]
    public Animator armAnimator;
    public GameObject scytheTracker;

    protected CentralShooting shooting;
    protected CentralWeaponSystem weaponSystem;

    [HideInInspector]
    public bool disableAiming, shouldThrow, attackAnimationPlaying;

    protected Rigidbody2D objectRig;
    protected GameObject ammunition = null;

    public void Awake()
    {
        shooting = transform.GetComponent<CentralShooting>();
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        armAnimator = transform.GetComponent<Animator>();
    }

    public void singleFireAttack(string weapon)
    {
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

    public void spamFireAttack(string weapon)
    {

    }

    public void meeleeAttack(string weapon)
    {
        if (weapon == "Scythe")
            StartCoroutine(attackWithScythe());
        else
            Debug.LogError("You haven't specified how to shoot this particular object");
    }

    private IEnumerator throwGrenade()
    {
        StartCoroutine(shooting.SetupThrowingObject(0.1f, 0.22f));
        armAnimator.SetInteger("Arms Phase", 1);

        while (!shouldThrow)
            yield return null;

        //apply a large force to throw the grenadeaa
        Vector2 unadjustedForce = grenadeThrowForce * shooting.aimDir * new Vector2(1.2f, 1) + new Vector2(0, grenadeYForce);
        objectRig.velocity = new Vector2(0, 0);
        objectRig.AddForce(unadjustedForce * objectRig.mass);
    }

    private IEnumerator throwPlasmaOrb()
    {
        //get aim direction from mouse input
        StartCoroutine(shooting.SetupThrowingObject(0.1f, 0.22f));
        armAnimator.SetInteger("Arms Phase", 1);

        while (!shouldThrow)
            yield return null;

        //apply a large force to throw the grenade
        Vector2 unadjustedForce = grenadeThrowForce * shooting.aimDir * new Vector2(1.2f, 1) + new Vector2(0, grenadeYForce);
        objectRig.velocity = new Vector2(0, 0);
        objectRig.AddForce(unadjustedForce * objectRig.mass);

        //start timed plasma explosion
        StartCoroutine(ammunition.transform.GetComponent<PlasmaOrb>().startTimedPlasmaExplosion());
    }

    private void shootBulletInStraightLine(float speed)
    {
        //get aim direction from mouse input
        shooting.SetupGunBullet();

        //spawn and orient the bullet correctly
        ammunition.transform.right = shooting.aimDir;
        objectRig.velocity = shooting.aimDir * speed;
    }

    private IEnumerator throwBoomerang()
    {
        //get aim direction from mouse input
        StartCoroutine(shooting.SetupThrowingObject(0.22f, 0.22f));
        armAnimator.SetInteger("Arms Phase", 1);

        while (!shouldThrow)
            yield return null;

        ammunition.transform.GetComponent<Animator>().SetBool("glare", false);
        objectRig.velocity = shooting.aimDir * boomerangSpeed;
        objectRig.angularVelocity = Random.Range(boomerangSpinSpeed.x, boomerangSpinSpeed.y) * (Random.Range(0, 2) * 2 - 1);
    }

    public void curveBoomerang()
    {
        if (weaponSystem.weaponSelected == "Boomerang" && ammunition && ammunition.activeSelf)
        {
            if (shooting.aimDir.x >= 0)
                objectRig.velocity = Quaternion.Euler(0, 0, -90) * shooting.aimDir * boomerangSpeed;
            else
                objectRig.velocity = Quaternion.Euler(0, 0, 90) * shooting.aimDir * boomerangSpeed;

            objectRig.angularVelocity = Random.Range(boomerangSpinSpeed.x, boomerangSpinSpeed.y) * (Random.Range(0, 2) * 2 - 1);
            ammunition.transform.GetComponent<Animator>().SetBool("glare", true);
        }
    }

    public IEnumerator throwScythe()
    {
        if (weaponSystem.weaponSelected == "Scythe")
        {
            shooting.retrieveWeaponAmmunition();
            StartCoroutine(shooting.SetupThrowableBlade(0.04f, 0.22f));
            armAnimator.SetInteger("Arms Phase", 11);

            while (!shouldThrow)
                yield return null;

            objectRig.velocity = shooting.aimDir * scytheThrowSpeed;
            objectRig.angularVelocity = Random.Range(scytheSpinSpeed.x, scytheSpinSpeed.y) * Mathf.Sign(-shooting.aimDir.x);
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
        shooting.SetupGunBullet();
        ammunition.transform.right = shooting.aimDir;
        objectRig.velocity = shooting.aimDir * shotgunBulletSpeed;

        shooting.retrieveWeaponAmmunition();
        shooting.SetupGunBullet();
        ammunition.transform.right = Quaternion.AngleAxis(goldenShotgunSpread, Vector3.forward) * shooting.aimDir;
        objectRig.velocity = Quaternion.AngleAxis(goldenShotgunSpread, Vector3.forward) * shooting.aimDir * shotgunBulletSpeed;

        shooting.retrieveWeaponAmmunition();
        shooting.SetupGunBullet();
        ammunition.transform.right = Quaternion.AngleAxis(-goldenShotgunSpread, Vector3.forward) * shooting.aimDir;
        objectRig.velocity = Quaternion.AngleAxis(-goldenShotgunSpread, Vector3.forward) * shooting.aimDir * shotgunBulletSpeed;

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

    public void updateEntities(GameObject ammunition, Rigidbody2D objectRig)
    {
        this.objectRig = objectRig;
        this.ammunition = ammunition;
    }
}
