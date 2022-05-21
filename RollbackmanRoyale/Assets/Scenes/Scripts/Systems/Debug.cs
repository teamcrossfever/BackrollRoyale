using System.Collections;
using System;

using UnityEngine;

public static class Debug
{
    public static event Action<object> OnLog;
    public static void Log(object message)
   {
        UnityEngine.Debug.Log(message);
        OnLog.Invoke(message);
   }
}
