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


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
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

}
