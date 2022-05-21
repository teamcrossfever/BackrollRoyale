using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace Sinistral.GGPO
{
    public class MultiplayerScreen : MonoBehaviour
    {
        NetworkGameManager networkGameManager => NetworkGameManager.Instance;


        public Transform sideContent;
        public PlayerConnectPannel playerConnectPrefab;
        public PlayerConnectPannel[] playerConnectPannels;


        public Button btnConnect;


        void CreatePlayerPanels()
        {
            for(int i= 0; i<playerConnectPannels.Length; i++)
            {
                var panel = Instantiate(playerConnectPrefab, sideContent) as PlayerConnectPannel;
                panel.playerLbl.text = $"Player {i}";
                panel.ip.text = "127.0.0.1";
                panel.port.text = (7000 + i).ToString();
                panel.name = $"Player Panel {i}";

                playerConnectPannels[i] = panel; //Set new panel in array
            }
        }

        private void Awake()
        {
            CreatePlayerPanels();
            btnConnect.onClick.AddListener(OnConnect);
        }

        private void OnConnect()
        {
            //Online
            if (true)
            {
                //Get connection list of all users
                var connectionInfo = GetConnectionInfo();


            }
        }

        public IList<Connections> GetConnectionInfo()
        {
            var connections = new List<Connections>(playerConnectPannels.Length);

            for(int i=0; i<playerConnectPannels.Length; i++)
            {
                //Add to connection list
                connections.Add(playerConnectPannels[i].GetConnectionInfo());
            }

            return connections;
        }
    }
}