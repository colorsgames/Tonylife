using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AliveCreatureDetector : Detector
{
    public delegate void AliveCreatureDiscovered(AliveCreature creature);

    public event AliveCreatureDiscovered discovered;

    private AliveCreature creature;

    private void Update()
    {
        if (creature)
        {
            RaycastHit2D hit = GetRaycastHit2D(creature.transform.position);
            if (!hit) return;
            if (hit.collider.GetComponent<AliveCreature>() == creature)
            {
                discovered?.Invoke(creature);
            }
            if (!creature.Alive)
            {
                creature = null;
                discovered?.Invoke(creature);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<AliveCreature>())
        {
            creature = collision.GetComponent<AliveCreature>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<AliveCreature>())
        {
            creature = null;
            discovered?.Invoke(creature);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (creature)
        {
            Vector3 offset = creature.transform.position - rayTarget.position;
            Gizmos.color = Color.red;
            Gizmos.DrawRay(rayTarget.position, offset.normalized * offset.magnitude);
        }
    }
#endif
}
