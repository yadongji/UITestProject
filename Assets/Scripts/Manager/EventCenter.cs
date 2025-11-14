using System;
using System.Collections.Generic;
using UnityEngine;

public class CommonEvent 
{
    public const int testID = 1000;
}

public class SocketEvent
{
    public const int testSocketID = 1000;
}

/// <summary>
/// 简易事件中心 - 单例模式
/// </summary>
public class EventCenter : MonoBehaviour
{
    private static EventCenter _instance;
    private Dictionary<int, Action<object>> eventDictionary;

    public static EventCenter Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("EventCenter");
                _instance = go.AddComponent<EventCenter>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    void Awake()
    {
        eventDictionary = new Dictionary<int, Action<object>>();
    }

    /// <summary>
    /// 添加事件监听
    /// </summary>
    public void AddEventListener(int eventID, Action<object> listener)
    {
        if (eventDictionary.ContainsKey(eventID))
        {
            eventDictionary[eventID] += listener;
        }
        else
        {
            eventDictionary.Add(eventID, listener);
        }
    }

    /// <summary>
    /// 移除事件监听
    /// </summary>
    public void RemoveEventListener(int eventID, Action<object> listener)
    {
        if (eventDictionary.ContainsKey(eventID))
        {
            eventDictionary[eventID] -= listener;

            // 如果没有监听者了，移除这个事件
            if (eventDictionary[eventID] == null)
            {
                eventDictionary.Remove(eventID);
            }
        }
    }

    /// <summary>
    /// 派发事件
    /// </summary>
    public void Dispatch(int eventID, object eventData = null)
    {
        if (eventDictionary.ContainsKey(eventID))
        {
            eventDictionary[eventID]?.Invoke(eventData);
        }
        else
        {
            DebugCenter.Instance.LogWarning($"事件ID: {eventID} 没有监听者");
        }
    }

    /// <summary>
    /// 清空所有事件监听
    /// </summary>
    public void Clear()
    {
        eventDictionary.Clear();
    }
}