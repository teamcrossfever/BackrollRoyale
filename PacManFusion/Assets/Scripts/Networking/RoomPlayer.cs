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

    [Networked] public int id { get; set; }
    [Networked(OnChanged = nameof(OnStateChanged))] public NetworkBool IsReady { get; set; }
    [Networked(OnChanged = nameof(OnStateChanged))] public NetworkString<_32> Username { get; set; }
    [Networked] public NetworkBool HasFinished { get; set; }

    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasInputAuthority)
        {
            Local = this;
            PlayerChanged?.Invoke(this);
            RPC_SetPlayerStats(ClientInfo.Username, ClientInfo.PacId);
        }

        Players.Add(this);
        PlayerJoined?.Invoke(this);

        DontDestroyOnLoad(gameObject);
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority, InvokeResim =true)]
    private void RPC_SetPlayerStats(NetworkString<_32> username, int id)
    {
        this.id = id;
        this.Username = username;
    }

    private static void OnStateChanged(Changed<RoomPlayer> changed) => PlayerChanged?.Invoke(changed.Behaviour);

    public static void RemovePlayer(NetworkRunner runner, PlayerRef p)
    {
        var roomPlayer = Players.FirstOrDefault(x => x.Object.InputAuthority == p);

        if (roomPlayer != null)
        {
            Players.Remove(roomPlayer);
            runner.Despawn(roomPlayer.Object);
        }
    }
}
