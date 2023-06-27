using System.Collections;
using UnityEngine;
using Fusion;

public class PlayerNetwork : NetworkBehaviour
{
    Pacman player;
    // Use this for initialization


    void Start()
    {
        player = GetComponent<Pacman>();
    }

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();

            player.UpdateMovement(data.direction);
        }
    }
}
