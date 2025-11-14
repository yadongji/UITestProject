using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 场景控制器 - 随场景加载/销毁
/// </summary>
public class SceneControllerManager : MonoBehaviour
{
    private Dictionary<string, BaseController> sceneControllers;

    void Start()
    {
        sceneControllers = new Dictionary<string, BaseController>();
        InitializeSceneControllers();
    }

    void InitializeSceneControllers()
    {
        // 创建场景专属控制器
        var uiController = new UIController();

        // 注册到MVCManager
        MVCManager.Instance.RegisterController("UI", uiController);

        sceneControllers.Add("UI", uiController);
    }

    void OnDestroy()
    {
        // 场景销毁时清理控制器
        foreach (var controller in sceneControllers.Values)
        {
            MVCManager.Instance.RemoveController(GetControllerName(controller));
            controller.Dispose();
        }
        sceneControllers.Clear();
    }

    private string GetControllerName(BaseController controller)
    {
        foreach (var pair in sceneControllers)
        {
            if (pair.Value == controller) return pair.Key;
        }
        return null;
    }
}