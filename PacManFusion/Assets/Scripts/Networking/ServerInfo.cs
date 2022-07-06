using UnityEngine.SceneManagement;
using UnityEngine;

public static class ServerInfo
{
    public const int UserCapacity = 30;

    public static string LobbyName;
    public static string StageName = "Battle";

    public static int GameMode
    {
        get => PlayerPrefs.GetInt("S_GameMode", 0);
        set => PlayerPrefs.SetInt("S_GameMode", value);
    }

    public static int PacId
    {
        get => PlayerPrefs.GetInt("S_PacId", 0);
        set => PlayerPrefs.SetInt("S_PacId", value);
    }

    public static int MaxUsers
    {
        get => PlayerPrefs.GetInt("S_MaxUsers", 30);
        set => PlayerPrefs.SetInt("S_MaxUsers", Mathf.Clamp(value, 1, UserCapacity));
    }
}
