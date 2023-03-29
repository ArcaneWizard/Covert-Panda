using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spawn rain splatter particles when this object's particles (rain drops) hit a surface
public class RainCollision : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private List<ParticleCollisionEvent> collisionEvents;

    private List<ParticleSystem> rainSplatterSystems; // collection of rain splatter particle systems
    private int index; // which rain splatter particle system to spawn in next

    [SerializeField] private int maxRainDrops = 250;
    [SerializeField] private ParticleSystem rainSplatter;
    [SerializeField] private Vector2 offset;

    void Awake()
    {
        particleSystem = transform.GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
        rainSplatterSystems = new List<ParticleSystem>();

        // clone as many rain splatter particle systems as needed
        for (int i = 0; i < maxRainDrops; i++)
        {
            ParticleSystem p = Instantiate(rainSplatter, Vector3.zero, Quaternion.identity, transform);
            rainSplatterSystems.Add(p);
        }
    }

    void Start()
    {
        var module = particleSystem.main;
        module.maxParticles = maxRainDrops;
    }

    void OnParticleCollision(GameObject other)
    {
        // get the number of rain drops that just hit a surface
        int numCollisionEvents = particleSystem.GetCollisionEvents(other, collisionEvents);

        // for each hit, spawn a rain splatter particle system right above where the rain drop hit
        int i = 0;
        while (i < numCollisionEvents)
        {
            rainSplatterSystems[index].transform.position = collisionEvents[i].intersection + new Vector3(offset.x, offset.y, 0);
            rainSplatterSystems[index].Play();
            index = (index + 1) % maxRainDrops;

            i++;
        }
    }
}