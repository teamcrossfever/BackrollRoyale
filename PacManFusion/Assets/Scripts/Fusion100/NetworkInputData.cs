using System.Collections;
using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public const byte MOUSEBUTTON1 = 0x01;

    public byte buttons;
    public Vector3 direction;
}
