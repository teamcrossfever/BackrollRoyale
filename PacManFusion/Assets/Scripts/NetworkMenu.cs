using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;
public enum ConnectionStatus
{
    Disconnected,
    Connecting,
    Failed,
    Connected
}
public class NetworkMenu : MonoBehaviour, INetworkRunnerCallbacks
{
    public static ConnectionStatus ConnectionStatus = ConnectionStatus.Disconnected;
    private GameMode _gameMode;
    private NetworkRunner _runner;
    private LevelManager _levelManager;

    // Start is called before the first frame update
    void Start()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        _levelManager = GetComponent<LevelManager>();

        DontDestroyOnLoad(gameObject);
    }

    public void SetCreateLobby() => _gameMode = GameMode.Host;
    public void SetJoinLobby() => _gameMode = GameMode.Client;
    // Update is called once per frame
    void Update()
    {
        
    }

    public void JoinOrCreateLobby()
    {
        SetConnectionStatus(ConnectionStatus.Connecting);

        GameObject go = new GameObject("Session");
        DontDestroyOnLoad(go);

        _runner = go.AddComponent<NetworkRunner>();
        _runner.ProvideInput = _gameMode != GameMode.Server;
        _runner.AddCallbacks(this);

        //Put object INetworkpool here

        Debug.Log($"Create gameObject {go.name} - starting game");
        _runner.StartGame(new StartGameArgs
        {
            GameMode = _gameMode,
            SessionName = _gameMode == GameMode.Host ? ServerInfo.LobbyName : ClientInfo.LobbyName,
            SceneManager = _levelManager,
            PlayerCount = ServerInfo.MaxUsers,
            DisableClientSessionCreation = true
        }); 
    }

    private void SetConnectionStatus(ConnectionStatus status)
    {
        Debug.Log($"Setting connection status to {status}.");

        ConnectionStatus = status;

        if (!Application.isPlaying)
            return;

        if(status == ConnectionStatus.Disconnected || status == ConnectionStatus.Failed)
        {
            SceneManager.LoadScene(LevelManager.LOBBY_SCENE);
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        throw new NotImplementedException();
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        throw new NotImplementedException();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
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
}
