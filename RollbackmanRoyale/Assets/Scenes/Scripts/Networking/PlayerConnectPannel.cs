using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Sinistral.GGPO;

public class PlayerConnectPannel : MonoBehaviour
{
    public TextMeshProUGUI playerLbl;

    [SerializeField]
    public TMP_InputField ip, port;

    public Connections GetConnectionInfo()
    {
        return new Connections
        {
            ip = ip.text,
            port = ushort.Parse(port.text),
            spectator = false
        };
    }
}
