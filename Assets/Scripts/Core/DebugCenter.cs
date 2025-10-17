using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCenter
{
    private static DebugCenter _instance;
    public static DebugCenter Instance {
        get
        {
            if (_instance == null)
            {
                _instance = new DebugCenter();
            }
            return _instance;
        }
    }
    public void LogWarning(string info) 
    {
        Debug.Log($"<color=red>{info}</color>");
    }

    public void Log(string info) 
    {
        Debug.Log($"<color=yellow>{info}</color>");
    }
}
