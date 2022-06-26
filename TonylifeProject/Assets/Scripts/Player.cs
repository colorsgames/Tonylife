using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : AliveCreature
{
    private float hor;

    protected override void Update()
    {
        base.Update();

        hor = Input.GetAxis("Horizontal");

        running = Input.GetButton("Run");
        desiredJump |= Input.GetButtonDown("Jump");
    }

    private void FixedUpdate()
    {
        Movement(hor);
    }
}
