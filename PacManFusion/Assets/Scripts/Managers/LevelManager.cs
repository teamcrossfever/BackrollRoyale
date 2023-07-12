using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

public class LevelManager : NetworkSceneManagerBase
{
    public static string LOBBY_SCENE = "Lobby";
    public static string BATTLE_SCENE = "Battle";

    public const int LOBBY_NUM = 0;

    [Networked]
    public NetworkString<_32> Destination { get; set; }

    Scene _loadedScene;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    /*
    protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
    {
        Debug.Log("NETWORK SWITCH");
        Debug.Log($"Switching Scene From {prevScene} to {newScene}");

        
        if (newScene <= 0)
        {
            finished(new List<NetworkObject>());
            yield break;
        }

        yield return new WaitForSeconds(1);

        yield return null;

        Debug.Log($"Start loading scene {newScene} in single peer mode");

        if (_loadedScene != default)
        {
            Debug.Log($"Unloading Scene {_loadedScene.buildIndex}");
            yield return SceneManager.UnloadSceneAsync(_loadedScene);
        }

        _loadedScene = default;
        Debug.Log($"Loading Scene {newScene}: {Destination}");

        List<NetworkObject> sceneObjects = new List<NetworkObject>();
        if (newScene != prevScene)
        {
            yield return SceneManager.LoadSceneAsync(Destination.ToString());
            Debug.Log($"ACTIVE SCENE: {SceneManager.GetActiveScene().name}");
            _loadedScene = SceneManager.GetActiveScene();
            sceneObjects = FindNetworkObjects(_loadedScene, disable: false);
        }

        finished(sceneObjects);
        yield return null;

        if(GameManager.CurrentStage != null && newScene!= prevScene)
        {
            Debug.Log($"STAGE GO: {GameManager.CurrentStage.stageName}");
            if(Runner.GameMode == GameMode.Host)
            {
                foreach(var player in RoomPlayer.Players){
                    GameManager.CurrentStage.SpawnPlayer(Runner, player);
                }
            }
        }

        yield return new WaitForSeconds(1);
    }
    */

    protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
    {
        Debug.Log($"Loading scene {newScene}");

        List<NetworkObject> sceneObjects = new List<NetworkObject>();

        if (newScene >= 0)
        {
            yield return SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Single);
            Scene loadedScene = SceneManager.GetSceneByBuildIndex(newScene);
            Debug.Log($"Loaded scene {newScene}: {loadedScene}");
            sceneObjects = FindNetworkObjects(loadedScene, disable: false);
        }

        finished(sceneObjects);

        // Delay one frame, so we're sure level objects has spawned locally
        yield return null;

        // Now we can safely spawn karts
        if (GameManager.CurrentStage != null && newScene > 0)
        {
            if (Runner.GameMode == GameMode.Host)
            {
                foreach (var player in RoomPlayer.Players)
                {
                    //player.GameState = RoomPlayer.EGameState.GameCutscene;
                    GameManager.CurrentStage.SpawnPlayer(Runner, player);
                }
            }
        }
    }

    /*
    protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
    {
        List<NetworkObject> sceneObjects = new List<NetworkObject>();

        if (newScene >= LOBBY_NUM)
        {
            yield return SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Single);
            Scene loadedScene = SceneManager.GetSceneByBuildIndex(newScene);
            Debug.Log($"Loaded scene {newScene} : {loadedScene}");
            sceneObjects = FindNetworkObjects(loadedScene, disable: false);

        }

        finished(sceneObjects);


    }
    */

}
