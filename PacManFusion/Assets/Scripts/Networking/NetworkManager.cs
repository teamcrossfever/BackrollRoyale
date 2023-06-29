using System.Collections;
using System.Linq;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

using Rewired;

public enum ConnectionStatus
{
    Disconnected,
    Connecting,
    Failed,
    Connected
}

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static ConnectionStatus ConnectionStatus = ConnectionStatus.Disconnected;
    private GameMode _gameMode;
    private NetworkRunner _runner;
    private FusionObjectPoolRoot _pool;
    private LevelManager _levelManager;

    public delegate void NetDelegate();
    public NetDelegate onConnected;
    public NetDelegate onPlayerJoined;

    [SerializeField] private NetworkPrefabRef _playerPrefab;
    [SerializeField] private NetworkPrefabRef _roomPlayerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    public void SetCreateLobby() => _gameMode = GameMode.Host;
    public void SetJoinLobby() => _gameMode = GameMode.Client;

    //Inputs
    bool _mouseButton0;
    bool _mouseButton1;

    Player cInput;

    void Start()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        _levelManager = GetComponent<LevelManager>();

        DontDestroyOnLoad(gameObject);

        cInput = Rewired.ReInput.players.GetPlayer(1);
        //Screen.SetResolution(640, 480, false);
    }
    private void OnGUI()
    {
        if (_runner!=null && _runner.IsRunning)
        {
            GUI.Label(new Rect(10, 10, 100, 200), "Player Count: " + _runner.ActivePlayers.Count());
        }
    }

    public async void StartGame(GameMode mode)
    {
        if (_runner != null)
            return;

        SetConnectionStatus(ConnectionStatus.Connecting);

        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = _gameMode != GameMode.Server;
        _runner.AddCallbacks(this);

        //Put object INetworkpool here

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = _levelManager,
            PlayerCount = ServerInfo.MaxUsers,
             
        });
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

    void SetConnectionStatus(ConnectionStatus status)
    {
        Debug.Log($"Connection Status: {status}");
        ConnectionStatus = status;

        if (!Application.isPlaying)
            return;

        if(ConnectionStatus == ConnectionStatus.Disconnected || ConnectionStatus == ConnectionStatus.Failed)
        {
            SceneManager.LoadScene(LevelManager.LOBBY_NUM);
        }
    }


    private void Update()
    {
        _mouseButton0 = _mouseButton0 | Input.GetMouseButton(0);
        _mouseButton1 = _mouseButton1 | Input.GetMouseButton(1);
    }

    public void SetScene(string sceneName)
    {
#if UNITY_EDITOR
        Debug.Log($"Set Network Scene Name: {sceneName}");
#endif

        _levelManager.Destination = sceneName;
        _levelManager.Runner.SetActiveScene(0);

    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        SetConnectionStatus(ConnectionStatus.Connected);
        if (onConnected != null)
        {
            onConnected.Invoke();
        }
        else{
            Debug.LogWarning($"No Event for {nameof(onConnected)}");
        }
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log($"Connect Failed {reason}");
        LeaveSession();
        SetConnectionStatus(ConnectionStatus.Failed);
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log($"Connect request by: {request.RemoteAddress}");
        request.Accept();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("Disconnected from server");
        LeaveSession();
        SetConnectionStatus(ConnectionStatus.Disconnected);
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
       
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        data.direction = new Vector3(cInput.GetAxisRaw("Horizontal"), cInput.GetAxisRaw("Vertical"), 0);
        data.direction = Vector3.ClampMagnitude(data.direction, 1);

        if (_mouseButton0)
        {
            data.buttons |= NetworkInputData.MOUSEBUTTON1;
            _mouseButton0 = false;
        }

        if (_mouseButton1)
        {
            data.buttons |= NetworkInputData.MOUSEBUTTON2;
            _mouseButton1 = false;
        }

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player} Joined!");

        if (runner.IsServer)
        {
            var roomPlayer = runner.Spawn(_roomPlayerPrefab, Vector3.zero, Quaternion.identity, player);
        }

        SetConnectionStatus(ConnectionStatus.Connected);

        if(onPlayerJoined!= null)
        {
            onPlayerJoined.Invoke();
            /*
            //Spawn Player
            Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 3, 1, 0);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);

            _spawnedCharacters.Add(player, networkPlayerObject);
            */
        }
        else
        {
            Debug.LogWarning($"No Event for {nameof(onPlayerJoined)}");
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"{player.PlayerId} disconnected.");
        RoomPlayer.RemovePlayer(runner, player);
        SetConnectionStatus(ConnectionStatus);
        /*
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
        */
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
       
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
       
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
       
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"On Shutdown {shutdownReason}.");

        if (_runner)
            Destroy(_runner.gameObject);

        RoomPlayer.Players.Clear();

        //Reset the object pools
        _pool.ClearPools();
        _pool = null;

        _runner = null;
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }

    public void LeaveSession()
    {
        if (_runner != null)
        {
            _runner.Shutdown();
        }
        else
        {
            SetConnectionStatus(ConnectionStatus.Disconnected);
        }
    }
}
