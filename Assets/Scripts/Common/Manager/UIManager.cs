using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI管理器（单例）- 加载、显示、隐藏UI面板
/// </summary>
public class UIManager : SingletonBase<UIManager>
{
    // 存储已加载的UI面板
    private Dictionary<string, UIBase> _uiPanelDic = new Dictionary<string, UIBase>();
    // UI根节点（Canvas）
    private Transform _uiRoot;

    protected override void Awake()
    {
        base.Awake();
        // 查找场景中的Canvas作为UI根节点
        _uiRoot = GameObject.Find("Canvas").transform;
        if (_uiRoot == null)
        {
            Debug.LogError("场景中未找到Canvas！请先创建Canvas");
        }
    }

    /// <summary>
    /// 加载UI面板（Resources目录下）
    /// </summary>
    /// <typeparam name="T">面板类型（继承UIBase）</typeparam>
    /// <param name="panelName">面板预制体名称</param>
    public T LoadUIPanel<T>(string panelName) where T : UIBase
    {
        if (_uiPanelDic.ContainsKey(panelName))
        {
            return _uiPanelDic[panelName] as T;
        }

        // 从Resources加载预制体
        GameObject panelPrefab = Resources.Load<GameObject>($"UIPanels/{panelName}");
        if (panelPrefab == null)
        {
            Debug.LogError($"未找到UI面板预制体：Resources/UIPanels/{panelName}");
            return null;
        }

        // 实例化面板到Canvas下
        GameObject panelObj = Object.Instantiate(panelPrefab, _uiRoot);
        panelObj.name = panelName; // 去掉克隆后缀
        T panel = panelObj.GetComponent<T>();
        if (panel == null)
        {
            Debug.LogError($"面板{panelName}未挂载UIBase子类脚本！");
            Object.Destroy(panelObj);
            return null;
        }

        // 初始化面板
        panel.Init();
        _uiPanelDic.Add(panelName, panel);
        return panel;
    }

    /// <summary>
    /// 获取已加载的UI面板
    /// </summary>
    public T GetUIPanel<T>(string panelName) where T : UIBase
    {
        if (_uiPanelDic.ContainsKey(panelName))
        {
            return _uiPanelDic[panelName] as T;
        }
        Debug.LogWarning($"面板{panelName}未加载！");
        return null;
    }

    /// <summary>
    /// 显示UI面板
    /// </summary>
    public void ShowUIPanel(string panelName)
    {
        if (_uiPanelDic.ContainsKey(panelName))
        {
            _uiPanelDic[panelName].Show();
        }
        else
        {
            Debug.LogWarning($"面板{panelName}未加载，自动加载并显示");
            LoadUIPanel<UIBase>(panelName);
        }
    }

    /// <summary>
    /// 隐藏UI面板
    /// </summary>
    public void HideUIPanel(string panelName)
    {
        if (_uiPanelDic.ContainsKey(panelName))
        {
            _uiPanelDic[panelName].Hide();
        }
        else
        {
            Debug.LogWarning($"面板{panelName}未加载！");
        }
    }

    /// <summary>
    /// 销毁UI面板
    /// </summary>
    public void DestroyUIPanel(string panelName)
    {
        if (_uiPanelDic.ContainsKey(panelName))
        {
            UIBase panel = _uiPanelDic[panelName];
            Object.Destroy(panel.gameObject);
            _uiPanelDic.Remove(panelName);
        }
        else
        {
            Debug.LogWarning($"面板{panelName}未加载！");
        }
    }
}

/// <summary>
/// UI面板基类（所有UI面板继承此类）
/// </summary>
public class UIBase : MonoBehaviour
{
    // 面板是否显示
    protected bool _isShow = false;

    /// <summary>
    /// 初始化面板（加载时调用）
    /// </summary>
    public virtual void Init()
    {
        // 子类重写：绑定UI组件、注册事件
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    public virtual void Show()
    {
        gameObject.SetActive(true);
        _isShow = true;
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    public virtual void Hide()
    {
        gameObject.SetActive(false);
        _isShow = false;
    }

    /// <summary>
    /// 面板销毁时移除事件监听（避免内存泄漏）
    /// </summary>
    protected virtual void OnDestroy()
    {
        // 子类重写：移除所有注册的事件
    }
}