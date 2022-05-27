using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using UnityEngine;
using UnityGGPO;


namespace Sinistral.Network
{
    public abstract class NetworkGameManager : MonoBehaviour
    {
        private static NetworkGameManager _instance;
        public static NetworkGameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<NetworkGameManager>();
                }
                return _instance;
            }
        }


        public event Action<bool> OnRunningChanged;

        public abstract void StartLocalGame();
        public abstract void StartGGPOGame(IPerfUpdate perfPanel, IList<Connections> connections, int playerIndex);

    }
}
