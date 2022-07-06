using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Fusion;
using UnityEngine;


public class RoomPlayer : NetworkBehaviour
{
    public enum EGameState{
        Lobby,
        GameCutScene,
        GameReady
    }

    public static readonly List<RoomPlayer> Players = new List<RoomPlayer>();

    public static Action<RoomPlayer> PlayerJoined;
    public static Action<RoomPlayer> PlayerLeft;
    public static Action<RoomPlayer> PlayerChanged;

    public static RoomPlayer Local;

    [Networked(OnChanged = nameof(OnStateChanged))] public NetworkBool IsReady { get; set; }
    [Networked(OnChanged = nameof(OnStateChanged))] public NetworkString<_32> Username { get; set; }
    [Networked] public NetworkBool HasFinished { get; set; }



    private static void OnStateChanged(Changed<RoomPlayer> changed) => PlayerChanged?.Invoke(changed.Behaviour);
}
