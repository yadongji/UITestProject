using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 全局游戏控制器 - 跨场景持久化
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameManager");
                _instance = go.AddComponent<GameManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private Dictionary<string, BaseController> globalControllers;

    void Awake()
    {
        globalControllers = new Dictionary<string, BaseController>();
        InitializeGlobalControllers();
    }

    void InitializeGlobalControllers()
    {
        // 创建全局控制器
        var audioController = new AudioController();
        var saveController = new SaveDataController();

        // 注册到MVCManager
        MVCManager.Instance.RegisterController("Audio", audioController);
        MVCManager.Instance.RegisterController("SaveData", saveController);

        // 同时在自己这里保存引用
        globalControllers.Add("Audio", audioController);
        globalControllers.Add("SaveData", saveController);
    }
}