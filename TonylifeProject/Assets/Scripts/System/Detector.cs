using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Detector : MonoBehaviour
{
    [SerializeField] protected Transform rayTarget;
    [SerializeField] protected LayerMask obstacles;

    public RaycastHit2D GetRaycastHit2D(Vector3 target)
    {
        Vector3 offset = target - rayTarget.position;
        return Physics2D.Raycast(rayTarget.position, offset.normalized, offset.magnitude, obstacles);
    }
}
