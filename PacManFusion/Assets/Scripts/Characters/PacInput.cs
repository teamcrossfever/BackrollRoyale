using System.Collections;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using Rewired;

public class PacInput : PacComponent, INetworkRunnerCallbacks
{
    public struct NetworkInputData : INetworkInput
    {
        public const uint ButtonUp = 1 << 0;
        public const uint ButtonDown = 1 << 1;
        public const uint ButtonLeft = 1 << 2;
        public const uint ButtonRight = 1 << 3;

        public uint Buttons;
        public uint OneShots;
        public bool IsUp(uint button) => IsDown(button) == false;
        public bool IsDown(uint button) => (Buttons & button) == button;
        public bool IsDownThisFrame(uint button) => (OneShots & button) == button;

        public bool IsButtonUp => IsDown(ButtonUp);
        public bool IsButtonDown => IsDown(ButtonDown);
        public bool IsButtonLeft => IsDown(ButtonLeft);
        public bool IsButtonRight => IsDown(ButtonRight);
    }

    Player pInput => ReInput.players.GetPlayer(0);

    public override void Spawned()
    {
        base.Spawned();

        Runner.AddCallbacks(this); 
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }
    
    public void OnConnectedToServer(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        throw new NotImplementedException();
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        throw new NotImplementedException();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new NotImplementedException();
    }
}
