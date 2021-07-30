
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponConfig : MonoBehaviour
{
    private float grenadeThrowForce = 1800;
    private float grenadeYForce = -20;
    private float boomerangSpeed = 41;
    private float plasmaBulletSpeed = 30;
    private float plasmaFireRate = 0.16f;
    private Vector2 boomerangSpinSpeed = new Vector2(180, 250);

    private Shooting shooting;
    private WeaponSystem weaponSystem;

    private Rigidbody2D objectRig;
    private GameObject ammunition = null;

    void Awake()
    {
        shooting = transform.GetComponent<Shooting>();
        weaponSystem = transform.GetComponent<WeaponSystem>();
    }

    public void singleFireAttack(string weapon)
    {
        if (weapon == "Grenade")
            StartCoroutine(throwGrenade());
        else if (weapon == "Boomerang")
            shootBoomerang();
        else if (weapon == "Plasma Orb")
            StartCoroutine(throwPlasmaOrb());
        else
            Debug.LogError("You haven't specified how to shoot this particular object");
    }

    public void spamFireAttack(string weapon)
    {
        if (weapon == "Pistol")
            shootPlasmaBullet();
        else
            Debug.LogError("You haven't specified how to shoot this particular object");
    }

    public void meeleeAttack(string weapon)
    {
        if (weapon == "Scythe")
            attackWithScythe();
        else
            Debug.LogError("You haven't specified how to shoot this particular object");
    }

    private IEnumerator throwGrenade()
    {
        //get aim direction from mouse input
        StartCoroutine(shooting.aimWithHands());
        yield return new WaitForSeconds(0.3f);

        //apply a large force to throw the grenadeaa
        Vector2 unadjustedForce = grenadeThrowForce * shooting.aimDir * new Vector2(1.2f, 1) + new Vector2(0, grenadeYForce);
        objectRig.velocity = new Vector2(0, 0);
        objectRig.AddForce(unadjustedForce * objectRig.mass);
    }

    private IEnumerator throwPlasmaOrb()
    {
        //get aim direction from mouse input
        StartCoroutine(shooting.aimWithHands());
        yield return new WaitForSeconds(0.3f);

        //apply a large force to throw the grenade
        Vector2 unadjustedForce = grenadeThrowForce * shooting.aimDir * new Vector2(1.2f, 1) + new Vector2(0, grenadeYForce);
        objectRig.velocity = new Vector2(0, 0);
        objectRig.AddForce(unadjustedForce * objectRig.mass);
    }

    private void shootPlasmaBullet()
    {
        //get aim direction from mouse input
        shooting.aimWithGun();
        shooting.timeLeftBtwnShots = plasmaFireRate;

        //spawn and orient the bullet correctly
        ammunition.transform.right = shooting.aimDir;
        objectRig.velocity = shooting.aimDir * plasmaBulletSpeed;
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
        if (weaponSystem.weaponSelected == "Boomerang" && Input.GetMouseButtonDown(1) && ammunition && ammunition.activeSelf)
        {
            if (shooting.aimDir.x >= 0)
                objectRig.velocity = Quaternion.Euler(0, 0, -90) * shooting.aimDir * boomerangSpeed;
            else
                objectRig.velocity = Quaternion.Euler(0, 0, 90) * shooting.aimDir * boomerangSpeed;

            objectRig.angularVelocity = Random.Range(boomerangSpinSpeed.x, boomerangSpinSpeed.y) * (Random.Range(0, 2) * 2 - 1);
            ammunition.transform.GetComponent<Animator>().SetBool("glare", true);
        }
    }

    private void attackWithScythe()
    {

    }

    public void updateEntities(GameObject ammunition, Rigidbody2D objectRig)
    {
        this.objectRig = objectRig;
        this.ammunition = ammunition;
    }
}
