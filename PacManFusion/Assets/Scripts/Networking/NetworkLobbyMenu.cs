using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using Fusion;

using TMPro;

public class NetworkLobbyMenu : MonoBehaviour
{
    [SerializeField]
    NetworkManager networkManager;

    //User UI
    [SerializeField]
    TMP_InputField playerNameInput;


    [Tooltip("1 - Single\n2 - Shared\n3 - Server\n4 - Host\n5 - Client\n6 - AutoHostOrClient")]
    [SerializeField]
    Button[] networkButtons;

    private void Start()
    {
        if(string.IsNullOrEmpty(ClientInfo.Username))
            playerNameInput.text = $"Player{Random.Range(0, 9999)}";

        playerNameInput.onEndEdit.AddListener(delegate
        {
            ClientInfo.Username = playerNameInput.text;
        });

        playerNameInput.onSubmit.AddListener(delegate
        {
            ClientInfo.Username = playerNameInput.text;
        });

        playerNameInput.onValueChanged.AddListener(delegate
        {
            ClientInfo.Username = playerNameInput.text;
        });

        for (int i=0; i< networkButtons.Length; i++)
        {
            var b = networkButtons[i]; //Network button;

            if (!b) //No network button found 
            {
                Debug.LogWarning($"No net button found on index {i}:{((GameMode)i).ToString()}"); 
                continue;
            }

            b.GetComponentInChildren<TextMeshProUGUI>().text = ((GameMode)i).ToString();
            b.onClick.AddListener(delegate
            {
                networkManager.StartGame((GameMode)i);
            });
        }
    }
}
