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

    }
}
