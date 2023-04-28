using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Provides functions to enable and disable creature ragdolling. When ragdolling
// is enabled, no manual/procedural animations occur. Instead the creatures' limbs
//  act like free-rigidbodies connected naturally like a body should be
public class Ragdolling : MonoBehaviour
{
    // Note: the hip and chest should always be the 0th and 1st entry
    public Rigidbody2D[] ragdollRigidbodies;
    private Collider2D[] ragdollColliders;
    public GameObject ragdollArms;

    private Rigidbody2D playerRig;
    private Animator animator;
    private CentralWeaponSystem weaponSystem;

    [SerializeField] private PhysicsMaterial2D limbFriction;
    [SerializeField] private PhysicsMaterial2D noFriction;

    private void Awake()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        playerRig = transform.GetComponent<Rigidbody2D>();
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();

        ragdollColliders = new Collider2D[ragdollRigidbodies.Length];
        for (int i = 0; i < ragdollRigidbodies.Length; i++)
            ragdollColliders[i] = ragdollRigidbodies[i].transform.GetComponent<Collider2D>();

        DisableRagdolling();
    }

    // Disable ragdolling. Allow animations to play and makes all limbs no longer subject
    // to physic forces and no longer able to collide with platforms.
    public void DisableRagdolling()
    {
        animator.SetInteger("ragdolling", 0);
        animator.enabled = true;
        ragdollArms.SetActive(false);

        for (int i = 0; i < ragdollRigidbodies.Length; i++)
        {
            ragdollRigidbodies[i].gameObject.layer = Layer.Limb;
            ragdollRigidbodies[i].isKinematic = true;
            ragdollRigidbodies[i].velocity = Vector2.zero;
            ragdollRigidbodies[i].sharedMaterial = noFriction;
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
        weaponSystem.CurrentWeaponConfiguration.Arms.SetActive(false);

        for (int i = 0; i < ragdollRigidbodies.Length; i++)
        {
            ragdollRigidbodies[i].gameObject.layer = Layer.LimbInRagdoll;
            ragdollRigidbodies[i].isKinematic = false;
            ragdollRigidbodies[i].velocity = velocityBeforeDeath;
            ragdollRigidbodies[i].gravityScale = CentralController.GRAVITY;
            ragdollRigidbodies[i].sharedMaterial = limbFriction;
        }

        ragdollRigidbodies[0].AddTorque(velocityBeforeDeath.magnitude * Random.Range(-20f, 20f));
        ragdollRigidbodies[1].AddTorque(velocityBeforeDeath.magnitude * Random.Range(-20f, 20f));
    }
}