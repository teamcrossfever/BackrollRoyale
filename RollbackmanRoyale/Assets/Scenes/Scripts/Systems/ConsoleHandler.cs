using System;
using System.Collections;
using UnityEngine;

using TMPro;
public class ConsoleHandler : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI console;

    private void Awake()
    {
        Debug.OnLog += OnLog;
    }

    void OnLog(object obj)
    {
        console.text=(obj.ToString());
    }

}
