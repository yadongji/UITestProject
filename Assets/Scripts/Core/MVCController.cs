using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MVC管理器
/// </summary>
public class MVCManager : MonoBehaviour
{
    private static MVCManager _instance;
    private Dictionary<string, BaseController> controllers;

    public static MVCManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("MVCManager");
                _instance = go.AddComponent<MVCManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    void Awake()
    {
        controllers = new Dictionary<string, BaseController>();
    }

    /// <summary>
    /// 注册控制器
    /// </summary>
    public void RegisterController(string controllerName, BaseController controller)
    {
        if (!controllers.ContainsKey(controllerName))
        {
            controllers.Add(controllerName, controller);
            controller.Initialize();
        }
    }

    /// <summary>
    /// 获取控制器
    /// </summary>
    public T GetController<T>(string controllerName) where T : BaseController
    {
        if (controllers.ContainsKey(controllerName))
        {
            return controllers[controllerName] as T;
        }
        return null;
    }

    /// <summary>
    /// 移除控制器
    /// </summary>
    public void RemoveController(string controllerName)
    {
        if (controllers.ContainsKey(controllerName))
        {
            controllers[controllerName].Dispose();
            controllers.Remove(controllerName);
        }
    }

    void OnDestroy()
    {
        foreach (var controller in controllers.Values)
        {
            controller.Dispose();
        }
        controllers.Clear();
    }
}