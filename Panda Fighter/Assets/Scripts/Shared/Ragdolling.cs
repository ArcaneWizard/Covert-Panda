using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdolling : MonoBehaviour
{
    public Rigidbody2D[] ragdollParts;
    public GameObject ragdollArms;

    private Rigidbody2D playerRig;
    private Collider2D mainCollider;
    private Animator animator;
    private CentralWeaponSystem weaponSystem;
    private CentralController controller;

    private void Awake()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        mainCollider = transform.GetChild(0).GetComponent<Collider2D>();
        playerRig = transform.GetComponent<Rigidbody2D>();
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        controller = transform.GetComponent<CentralController>();

        Disable();
    }

    // disable ragdolling. allow animations to play (ex. walking), deactives the ragdoll arms,
    // and makes all limbs no longer subject to physic forces and no longer able to collide with platforms.
    public void Disable()
    {
        animator.SetInteger("ragdolling", 0);
        animator.enabled = true;
        ragdollArms.SetActive(false);

        foreach (Rigidbody2D rig in ragdollParts)
        {
            rig.isKinematic = true;
            rig.gameObject.layer = Layers.collideWithNothing;
        }

        playerRig.isKinematic = false;
    }

    // enable ragdolling. disables animations (ex. walking), deactivates the weapon
    // plus current arms holding that weapon, and enables the ragdoll arms instead. Also 
    // makes all the other limbs subject to physics forces and able to collide with platforms. 
    public IEnumerator Enable()
    {
        animator.SetInteger("ragdolling", 1);
        yield return new WaitForSeconds(Time.deltaTime);
        animator.enabled = false;
        playerRig.isKinematic = true;

        weaponSystem.weaponConfiguration.weapon.SetActive(false);
        foreach (GameObject armLimb in weaponSystem.weaponConfiguration.limbs) 
            armLimb.SetActive(false);

        foreach (Rigidbody2D rig in ragdollParts)
        {
            rig.isKinematic = false;
            rig.gameObject.layer = Layers.collideWithMap;
        }  

        ragdollArms.SetActive(true);
        
       // Sets the velocity of the ragdoll hip equal to the player's velocity b4 they died
        ragdollParts[0].AddForce(playerRig.velocity * UnityEngine.Random.Range(120, 200));

        if (controller.dirX != 0 && controller.isGrounded)
            ragdollParts[7].AddTorque(-Mathf.Sign(playerRig.velocity.x) * UnityEngine.Random.Range(2, 5.4f) * 800f);
    }
}