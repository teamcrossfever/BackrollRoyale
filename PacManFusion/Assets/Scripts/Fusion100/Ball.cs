using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class Ball : NetworkBehaviour
{
    public float speed =5;

    [Networked]
    private TickTimer life { get; set; }

    public void Init()
    {
        life = TickTimer.CreateFromSeconds(Runner, 5);
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
        {
            Runner.Despawn(Object);
            return;
        }

        transform.position += speed * transform.forward * Runner.DeltaTime;
    }
}
