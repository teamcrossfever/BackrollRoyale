using System.Collections;
using System.Linq;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

using Rewired;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    NetworkRunner _runner;

    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    //Inputs
    bool _mouseButton0;
    bool _mouseButton1;

    Player cInput;

    void Start()
    {
        cInput = Rewired.ReInput.players.GetPlayer(1);
        Screen.SetResolution(640, 480, false);
    }
    private void OnGUI()
    {
        if (_runner == null)
        {
            int yloc = 0;
            if(GUI.Button(new Rect(0,yloc,200,40), "Server Dedicate"))
            {
                StartGame(GameMode.Server);
            }

            yloc += 40;

            if (GUI.Button(new Rect(0, yloc, 200, 40), "Host"))
            {
                StartGame(GameMode.Host);
            }

            yloc += 40;

            if (GUI.Button(new Rect(0, yloc, 200, 40), "Shared Host"))
            {
                StartGame(GameMode.Host);
            }

            yloc += 40;

            if (GUI.Button(new Rect(0, yloc, 200, 40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }

        if (_runner!=null && _runner.IsRunning)
        {
            GUI.Label(new Rect(10, 10, 100, 200), "Player Count: " + _runner.ActivePlayers.Count());
        }
    }

    async void StartGame(GameMode mode)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = 50,
             
        });
    }

    private void Update()
    {
        _mouseButton0 = _mouseButton0 | Input.GetMouseButton(0);
        _mouseButton1 = _mouseButton1 | Input.GetMouseButton(1);
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        
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
       Vector3 spawnPosition = new Vector3 ((player.RawEncoded%runner.Config.Simulation.DefaultPlayers)*3,1,0);
        NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);

        _spawnedCharacters.Add(player, networkPlayerObject);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
      if(_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
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
       
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }
}
