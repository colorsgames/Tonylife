using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : NPC
{
    protected override void ACDiscovered(AliveCreature creature)
    {
        if (creature)
        {
            if (creature.GetComponent<Player>())
            {
                if (!curretWeapon) return;
                running = true;
                SetAggressive(true, creature.transform);
            }
        }
        else
        {
            running = false;
            SetAggressive(false, null);
        }
    }
}
