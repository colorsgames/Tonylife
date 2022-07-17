using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotTrail : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    [SerializeField] private float speed;

    Vector3 target;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        Vector3 offset = target - transform.position;
        Vector3 dir = Vector3.ClampMagnitude(offset, 1);
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
        if (offset.magnitude < 0.1)
        {
            Destroy(gameObject);
        }
    }

    public void SetTarget(Vector3 _target)
    {
        target = _target;
    }
}