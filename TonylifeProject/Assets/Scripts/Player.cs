using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : AliveCreature
{
    private float hor;

    protected override void Update()
    {
        if (!Alive) return;

        base.Update();

        hor = Input.GetAxis("Horizontal");

        running = Input.GetButton("Run");
        desiredJump |= Input.GetButtonDown("Jump");

        if (Input.GetKeyDown(KeyCode.G))
        {
            Dead();
        }
    }

    private void FixedUpdate()
    {
        Movement(hor);
    }
}
