using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件中心（单例）- 解耦各模块通信
/// </summary>
public class EventCenter : SingletonBase<EventCenter>
{
    // 事件字典：key=事件名，value=事件回调（支持带参数）
    private Dictionary<string, Action<object>> eventDic;

    protected override void Awake()
    {
        eventDic = new Dictionary<string, Action<object>>();
        base.Awake();
    }

    /// <summary>
    /// 注册事件
    /// </summary>
    /// <param name="eventName">事件名（建议用常量定义）</param>
    /// <param name="callback">回调函数（参数为object，可传任意类型）</param>
    public void RegisterEvent(string eventName, Action<object> callback)
    {
        if (!eventDic.ContainsKey(eventName))
        {
            eventDic.Add(eventName, null);
        }
        eventDic[eventName] += callback;
    }

    /// <summary>
    /// 移除事件（避免内存泄漏）
    /// </summary>
    public void RemoveEvent(string eventName, Action<object> callback)
    {
        if (eventDic.ContainsKey(eventName))
        {
            eventDic[eventName] -= callback;
            if (eventDic[eventName] == null)
            {
                eventDic.Remove(eventName);
            }
        }
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="data">传递的数据（可选）</param>
    public void TriggerEvent(string eventName, object data = null)
    {
        if (eventDic.ContainsKey(eventName) && eventDic[eventName] != null)
        {
            eventDic[eventName].Invoke(data);
        }
        else
        {
            DebugHelper.Instance.LogWarning($"事件[{eventName}]未注册或无回调！");
        }
    }

    // 清空所有事件（场景切换时调用）
    public void ClearAllEvents()
    {
        eventDic.Clear();
    }
}

// 事件名常量定义（避免拼写错误）
public static class EventDefine
{
    public const string AddStudent = "AddStudent";       // 添加学生
    public const string DeleteStudent = "DeleteStudent"; // 删除学生
    public const string UpdateStudentList = "UpdateStudentList"; // 学生列表更新
    public const string LoadDataSuccess = "LoadDataSuccess"; // 数据加载成功
    public const string LoadData = "LoadData"; // 重新加载数据

}