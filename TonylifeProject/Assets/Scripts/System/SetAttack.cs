using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAttack : MonoBehaviour
{
    private AliveCreature creature;

    private void Start()
    {
        creature = GetComponentInParent<AliveCreature>();
    }

    public void StartAttacks()
    {
        creature.CurretWeapon.Attacks = true;
    }

    public void EndAttacks()
    {
        creature.CurretWeapon.Attacks = false;
    }
}
