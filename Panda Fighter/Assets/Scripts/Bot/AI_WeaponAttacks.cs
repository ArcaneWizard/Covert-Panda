
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_WeaponAttacks : MonoBehaviour
{
    private float grenadeThrowForce = 2200;
    private float grenadeYForce = -20;
    private float boomerangSpeed = 61;
    private Vector2 boomerangSpinSpeed = new Vector2(600, 750);
    private float sniperBulletSpeed = 100;
    private float shielderBulletSpeed = 45;
    private float shotgunBulletSpeed = 52;

    private float scytheThrowSpeed = 60;
    private Vector2 scytheSpinSpeed = new Vector2(1200, 1400);

    public List<Transform> goldenShotgunBits = new List<Transform>();
    private int shotgunBitCounter = 0;
    private Vector2 goldenBitsForceX = new Vector2(-50, 70), goldenBitsForceY = new Vector2(0, 290);
    private float goldenShotgunSpread = 22;

    private List<GameObject> bullets = new List<GameObject>();
    private List<Rigidbody2D> objectRigs = new List<Rigidbody2D>();

    [HideInInspector]
    public Animator armAnimator;
    public GameObject scytheTracker;

    [HideInInspector]
    public bool disableAiming, shouldThrow, attackAnimationPlaying;

    private AI_Shooting shooting;
    private AI_WeaponSystem weaponSystem;

    private Rigidbody2D objectRig;
    private GameObject ammunition = null;

    void Awake()
    {
        shooting = transform.GetComponent<AI_Shooting>();
        weaponSystem = transform.GetComponent<AI_WeaponSystem>();
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
        StartCoroutine(shooting.aimWithHands(0.1f, 0.22f));
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
        StartCoroutine(shooting.aimWithHands(0.1f, 0.22f));
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
        shooting.aimWithGun();

        //spawn and orient the bullet correctly
        ammunition.transform.right = shooting.aimDir;
        objectRig.velocity = shooting.aimDir * speed;
    }

    private IEnumerator throwBoomerang()
    {
        //get aim direction from mouse input
        StartCoroutine(shooting.aimWithHands(0.22f, 0.22f));
        armAnimator.SetInteger("Arms Phase", 1);

        while (!shouldThrow)
            yield return null;

        ammunition.transform.GetComponent<Animator>().SetBool("glare", false);
        objectRig.velocity = shooting.aimDir * boomerangSpeed;
        objectRig.angularVelocity = Random.Range(boomerangSpinSpeed.x, boomerangSpinSpeed.y) * (Random.Range(0, 2) * 2 - 1);
    }

    private void shootBoomerang()
    {
        //get aim direction from mouse input
        ammunition.transform.GetComponent<Animator>().SetBool("glare", false);
        shooting.aimWithGun();

        //set the boomerang's velocity really high
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
            shooting.shootAnotherBullet();
            StartCoroutine(shooting.aimThrowableWeapon(0.04f, 0.22f));
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
        shooting.aimWithGun();
        ammunition.transform.right = shooting.aimDir;
        objectRig.velocity = shooting.aimDir * shotgunBulletSpeed;

        shooting.shootAnotherBullet();
        shooting.aimWithGun();
        ammunition.transform.right = Quaternion.AngleAxis(goldenShotgunSpread, Vector3.forward) * shooting.aimDir;
        objectRig.velocity = Quaternion.AngleAxis(goldenShotgunSpread, Vector3.forward) * shooting.aimDir * shotgunBulletSpeed;

        shooting.shootAnotherBullet();
        shooting.aimWithGun();
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
