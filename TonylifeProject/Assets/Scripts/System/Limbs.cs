using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limbs : MonoBehaviour
{
    [SerializeField] private float damageRatio;
    [SerializeField] private float health = 40;

    private AliveCreature creature;
    private HingeJoint2D joint;

    private void Start()
    {
        creature = GetComponentInParent<AliveCreature>();
        if (GetComponent<HingeJoint2D>())
        {
            joint = GetComponent<HingeJoint2D>();
        }
    }

    public void MakeDamage(float damage)
    {
        creature.MakeDamage(damage * damageRatio);
        health-=damage;
        if(health <= 0)
        {
            Dead();
        }
    }

    private void Dead()
    {
        if (joint)
        {
            Destroy(joint);
        }
    }
}
