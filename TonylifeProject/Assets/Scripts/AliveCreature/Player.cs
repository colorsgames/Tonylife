using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : AliveCreature
{
    private InteractiveObject closeIntObj;

    private float hor;

    private bool canUse;

    protected override void Update()
    {
        if (!Alive) return;

        base.Update();

        hor = Input.GetAxis("Horizontal");

        running = Input.GetButton("Run");
        desiredJump |= Input.GetButtonDown("Jump");

        if (Input.GetButtonDown("Use") && canUse)
        {
            closeIntObj.Use();
            if (closeIntObj.GetComponent<Item>())
            {
                if (inventory.Busy)
                {
                    inventory.DropItem(dropTarget.position, MyDirection(), dropTarget.rotation, weaklyDropItemForce, 0);
                }
                inventory.TakeItem(closeIntObj.GetComponent<Item>().ItemInfo.Id);
                curretWeapon = inventory.GetCurretWeapon();
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (curretWeapon)
            {
                curretWeapon.Attack();
            }
        }

        if (Input.GetButtonDown("Fire2"))
        {
            if (inventory.Busy)
            {
                inventory.DropItem(dropTarget.position, MyDirection(), dropTarget.rotation, stronglyDropItemForce, rotationForce);
                curretWeapon = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Dead();
        }

        DesignateInteractivity();
    }

    private void FixedUpdate()
    {
        Movement(hor);
    }

    private void DesignateInteractivity()
    {
        closeIntObj = GetCloseInterective();
        if (closeIntObj)
        {
            GameManager.instance.IndicatorWakeUp(closeIntObj.transform.GetChild(0));
            canUse = true;
        }
        else
        {
            GameManager.instance.IndicatorSleep();
            canUse = false;
        }
    }

    protected override void OnDrawGizmos()
    {
        if (canUse)
        {
            Vector3 dir = closeIntObj.transform.position - transform.position;
            Gizmos.DrawRay(transform.position, dir.normalized * dir.magnitude);
        }

        base.OnDrawGizmos();
    }
}
