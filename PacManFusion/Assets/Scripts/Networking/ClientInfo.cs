using UnityEngine.SceneManagement;
using UnityEngine;

public class ClientInfo : MonoBehaviour
{
    public static string Username
    {
        get => PlayerPrefs.GetString("C_Username", string.Empty);
        set => PlayerPrefs.SetString("C_Username", value);
    }

    public static int PacId
    {
        get => PlayerPrefs.GetInt("C_PacId", 0);
        set => PlayerPrefs.SetInt("C_PacId", value);
    }

    public static string LobbyName
    {
        get => PlayerPrefs.GetString("C_LastLobbyName", "");
        set => PlayerPrefs.SetString("C_LastLobbyName", value);
    }
}
