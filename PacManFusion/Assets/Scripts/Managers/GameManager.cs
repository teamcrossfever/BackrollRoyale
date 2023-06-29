using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    static bool isGMloaded = false;

    //Net Game
    [SerializeField]
    NetworkManager networkManager;

    public static Stage CurrentStage { get; private set; }

    private void Awake()
    {
        //Ensure there is only one per scene
        if (instance == null)
        {
            instance = this;
            FirstTimeRun();
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            //Destroy duplicates
            if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    //Make sure the first Manager only run these
    void FirstTimeRun()
    {

        if (!isGMloaded)
            SceneManager.sceneLoaded += OnSceneLoaded;

        //Set Net Manager
        //networkManager = GetComponent<NetworkManager>();
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        CurrentStage = FindObjectOfType<Stage>();
    }
}
