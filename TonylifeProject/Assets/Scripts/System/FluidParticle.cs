using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidParticle : MonoBehaviour
{
    [SerializeField] private GameObject fluidPrefab;

    private ParticleSystem part;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    void Start()
    {
        part = GetComponent<ParticleSystem>();
    }

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

        int i = 0;

        while (i < numCollisionEvents)
        {
            Vector3 pos = collisionEvents[i].intersection - collisionEvents[i].normal * Random.Range(0.3f, 0.8f);
            GameObject gb = Instantiate(fluidPrefab, pos, Quaternion.identity);
            if(other.GetComponent<Rigidbody2D>())
                gb.transform.parent = other.transform;
            i++;
        }
    }
}
