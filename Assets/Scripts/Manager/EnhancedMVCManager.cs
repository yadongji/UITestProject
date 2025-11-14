using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 控制器生命周期类型
/// </summary>
public enum ControllerLifecycle
{
    Global,     // 全局，游戏运行期间一直存在
    Scene,      // 场景级别，随场景销毁
    Temporary   // 临时，手动管理生命周期
}

/// <summary>
/// 增强的MVC管理器
/// </summary>
public class EnhancedMVCManager : MonoBehaviour
{
    private static EnhancedMVCManager _instance;

    public static EnhancedMVCManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("EnhancedMVCManager");
                _instance = go.AddComponent<EnhancedMVCManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private Dictionary<string, BaseController> globalControllers;
    private Dictionary<string, BaseController> sceneControllers;
    private Dictionary<string, BaseController> temporaryControllers;

    void Awake()
    {
        globalControllers = new Dictionary<string, BaseController>();
        sceneControllers = new Dictionary<string, BaseController>();
        temporaryControllers = new Dictionary<string, BaseController>();

        // 监听场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    /// <summary>
    /// 注册控制器并指定生命周期
    /// </summary>
    public void RegisterController(string controllerName, BaseController controller, ControllerLifecycle lifecycle = ControllerLifecycle.Scene)
    {
        switch (lifecycle)
        {
            case ControllerLifecycle.Global:
                globalControllers[controllerName] = controller;
                break;
            case ControllerLifecycle.Scene:
                sceneControllers[controllerName] = controller;
                break;
            case ControllerLifecycle.Temporary:
                temporaryControllers[controllerName] = controller;
                break;
        }

        controller.Initialize();
    }

    /// <summary>
    /// 场景加载时调用
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"场景 {scene.name} 加载完成");

        // 可以在这里初始化场景特定的控制器
        if (scene.name == "MainMenu")
        {
            //RegisterController("Menu", new MenuController(), ControllerLifecycle.Scene);
        }
        else if (scene.name == "Game")
        {
            //RegisterController("Game", new GameController(), ControllerLifecycle.Scene);
        }
    }

    /// <summary>
    /// 场景卸载时调用
    /// </summary>
    private void OnSceneUnloaded(Scene scene)
    {
        Debug.Log($"场景 {scene.name} 卸载");

        // 清理场景级别的控制器
        foreach (var controller in sceneControllers.Values)
        {
            controller.Dispose();
        }
        sceneControllers.Clear();

        // 清理临时控制器
        foreach (var controller in temporaryControllers.Values)
        {
            controller.Dispose();
        }
        temporaryControllers.Clear();
    }

    /// <summary>
    /// 获取控制器
    /// </summary>
    public T GetController<T>(string controllerName) where T : BaseController
    {
        if (globalControllers.ContainsKey(controllerName))
            return globalControllers[controllerName] as T;
        if (sceneControllers.ContainsKey(controllerName))
            return sceneControllers[controllerName] as T;
        if (temporaryControllers.ContainsKey(controllerName))
            return temporaryControllers[controllerName] as T;

        return null;
    }

    /// <summary>
    /// 手动移除控制器
    /// </summary>
    public void RemoveController(string controllerName)
    {
        if (globalControllers.ContainsKey(controllerName))
        {
            globalControllers[controllerName].Dispose();
            globalControllers.Remove(controllerName);
        }
        else if (sceneControllers.ContainsKey(controllerName))
        {
            sceneControllers[controllerName].Dispose();
            sceneControllers.Remove(controllerName);
        }
        else if (temporaryControllers.ContainsKey(controllerName))
        {
            temporaryControllers[controllerName].Dispose();
            temporaryControllers.Remove(controllerName);
        }
    }

    void OnDestroy()
    {
        // 清理所有控制器
        foreach (var controller in globalControllers.Values) controller.Dispose();
        foreach (var controller in sceneControllers.Values) controller.Dispose();
        foreach (var controller in temporaryControllers.Values) controller.Dispose();

        globalControllers.Clear();
        sceneControllers.Clear();
        temporaryControllers.Clear();

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
}