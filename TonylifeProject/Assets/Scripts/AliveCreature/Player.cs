using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : AliveCreature
{
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
            Use();
        }

        if (Input.GetButton("Fire1"))
        {
            if (curretWeapon)
            {
                curretWeapon.StartAttack();
            }
        }

        if (Input.GetButtonDown("Fire2"))
        {
            QuitItem();
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
            if (closeIntObj.GetComponent<Item>().ItemInfo.type == ItemData.ItemType.guns)
            {
                if (closeIntObj.GetComponent<Item>().Ammo > 0)
                {
                    GameManager.Instance.IndicatorWakeUp(closeIntObj.transform);
                    canUse = true;
                }
                else
                {
                    GameManager.Instance.IndicatorSleep();
                    canUse = false;
                }
            }
            else
            {
                GameManager.Instance.IndicatorWakeUp(closeIntObj.transform);
                canUse = true;
            }
        }
        else
        {
            GameManager.Instance.IndicatorSleep();
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
