using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{   
    // for fast bullets, store the predicted hit location + who or what's gonna be hit (predicted when bullet is about to be shot)
    public Vector2 predictedImpactLocation  {get; private set;}
    public Transform predictedColliderHit {get; private set;}
    private bool executedPredictedImpact;
    private Vector2 aim;
    private Vector2 middleOfWeapon;
    
    //  whether or not the bullet can detect physical collisions (with a creature, platform, etc.)
    public bool disabledImpactDetection;

    // returns the damage the bullet does
    public virtual int Damage() =>  transform.parent.GetComponent<WeaponConfiguration>().bulletDmg;

    // update that the bullet should no longer detect collisions and triggers OnCreatureEnter() 
    public void ConfirmImpactWithCreature(Transform creature) 
    {
        disabledImpactDetection = true;
        OnCreatureEnter(creature);
    }

    public static int raycastsUsed;
    // run predictive collision logic for fast bullets. 
    public void RunPredictiveLogic(Vector2 aim, Vector2 bulletStartPosition) 
    {
       predictedImpactLocation = Vector2.zero;
        predictedColliderHit = null;
        executedPredictedImpact = false;

        disabledImpactDetection = true;
        this.aim = aim;

        StartCoroutine(updatePrediction());
    }

    private IEnumerator updatePrediction() 
    {
        if (executedPredictedImpact)
            yield break;

        WeaponConfiguration weaponConfiguration = transform.parent.GetComponent<WeaponConfiguration>();
       ++raycastsUsed;
        
        checkForPredictedCollision();

        // start the predictive raycast from slightly behind where the bullet actually spawns (to detect collisions on creatures walking the gun)
        middleOfWeapon = new Vector2(transform.position.x - aim.x * 1.2f, transform.position.y - aim.y * 1.2f);
        RaycastHit2D hit = Physics2D.Raycast(middleOfWeapon, aim, 70f, LayerMasks.mapOrTarget(transform));

       // if (hit.collider != null)
       // Debug.DrawLine(new Vector2(transform.position.x- aim.x * 1.5f, transform.position.y - aim.y * 1.5f), hit.point, Color.green, 4);
        predictedImpactLocation = (hit.collider != null) ? hit.point: new Vector2(transform.position.x + aim.x * 70f, transform.position.y + aim.y * 70f);
        predictedColliderHit = (hit.collider != null) ? hit.collider.transform : null;
        
        yield return new WaitForSeconds(Time.deltaTime*2f*(hit.collider!= null ? 1f : 2f));
       
        StartCoroutine(updatePrediction());
    }     

    // if a fast bullet reaches the predicted collision location, damage the predicted enemy hit if applicable, and trigger
    // either onCreatureEnter() or OnMapEnter() if applicable
    private float x, y;
    void FixedUpdate() => checkForPredictedCollision();
    void checkForPredictedCollision() 
    {
         if (executedPredictedImpact || predictedImpactLocation == Vector2.zero)
            return;

        x = Mathf.Sign((transform.position.x - predictedImpactLocation.x + aim.x * 2f));
        y = Mathf.Sign((transform.position.y - predictedImpactLocation.y + aim.y * 2f));

            if ((x == Mathf.Sign(aim.x) || x == 0) && (y == Mathf.Sign(aim.y) || y == 0)) 
            {
                transform.position = predictedImpactLocation; 
                executedPredictedImpact = true;

                if (predictedColliderHit != null && predictedColliderHit.parent.GetComponent<Health>()) {
                    predictedColliderHit.parent.GetComponent<Health>().TakeDamage(Damage());
                    OnCreatureEnter(predictedColliderHit);
                }
                else if (predictedColliderHit != null)
                    OnMapEnter(predictedColliderHit);
            }
    }

    // Called whenever the bullet hits a physical platform/object on the map. By default, deactivate the bullet.
    protected virtual void OnMapEnter(Transform map) => StartCoroutine(delay());

    private IEnumerator delay() 
    {
        transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        yield return new WaitForSeconds(Time.deltaTime);
        gameObject.SetActive(false);
    }

    // Called whenever the bullet hits a creature. By default, deactivate the bullet.
    protected virtual void OnCreatureEnter(Transform creature) => gameObject.SetActive(false);

    // if a bullet hits any part of the map, trigger OnMapEnter() 
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == Layers.map && !disabledImpactDetection) 
        {
            disabledImpactDetection = true;  
            OnMapEnter(col.transform);
        }
    }
}
