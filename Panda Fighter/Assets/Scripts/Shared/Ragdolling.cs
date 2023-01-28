using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdolling : MonoBehaviour
{
    // Note: the hip and chest should always be the 0th and 1st entry
    public Rigidbody2D[] ragdollParts;
    public GameObject ragdollArms;

    private Rigidbody2D playerRig;
    private Animator animator;
    private CentralWeaponSystem weaponSystem;
    private CentralController controller;
    private CentralDeathSequence deathSequence;

    private void Awake()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        playerRig = transform.GetComponent<Rigidbody2D>();
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        controller = transform.GetComponent<CentralController>();
        deathSequence = transform.GetComponent<CentralDeathSequence>();

        DisableRagdolling();
    }

    // Disable ragdolling. Allow animations to play and makes all limbs no longer subject
    // to physic forces and no longer able to collide with platforms.
    public void DisableRagdolling()
    {
        animator.SetInteger("ragdolling", 0);
        animator.enabled = true;
        ragdollArms.SetActive(false);

        foreach (Rigidbody2D rig in ragdollParts)
        {
            rig.isKinematic = true;
            rig.gameObject.layer = Layer.Limb;
        }

        playerRig.isKinematic = false;
    }

    // Enables ragdolling: makes all limbs subject to physics forces and collidable with platforms.
    public void EnableRagdolling() => StartCoroutine(enableRagdolling());

    private IEnumerator enableRagdolling()
    {
        Vector2 velocityBeforeDeath = playerRig.velocity;
        animator.SetInteger("ragdolling", 1);
        yield return null;

        animator.enabled = false;
        ragdollArms.SetActive(true);
        playerRig.isKinematic = true;

        weaponSystem.CurrentWeaponConfiguration.PhysicalWeapon.SetActive(false);
        foreach (GameObject armLimb in weaponSystem.CurrentWeaponConfiguration.Arms) 
            armLimb.SetActive(false);

        foreach (Rigidbody2D rig in ragdollParts)
        {
            rig.velocity = Vector2.zero;
            rig.gravityScale = CentralController.Gravity;
            rig.gameObject.layer = Layer.LimbInRagdoll;
            rig.isKinematic = false;
            rig.velocity = velocityBeforeDeath;
        }

        ragdollParts[0].AddTorque(velocityBeforeDeath.magnitude * Random.Range(-20f, 20f));
        ragdollParts[1].AddTorque(velocityBeforeDeath.magnitude * Random.Range(-20f, 20f));
    }
}