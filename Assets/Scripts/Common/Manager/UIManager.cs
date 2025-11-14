using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// View打开模式（跳转规则）
/// </summary>
public enum ViewOpenMode
{
    Overlay,        // 叠加模式（保留当前面板，新面板在上方）
    ReplaceCurrent, // 替换模式（关闭当前面板，打开新面板）
    SingleTop,      // 单例置顶（如已存在，直接置顶显示，不重复创建）
    ClearStack      // 清空栈模式（关闭所有面板，打开新面板）
}

/// <summary>
/// UI管理器（升级版）- 负责View层级、跳转、栈管理、防重复
/// </summary>
public class UIManager : SingletonBase<UIManager>
{
    // 1. 面板存储（键：面板名称，值：面板实例）
    private Dictionary<string, UIBase> _viewDic = new Dictionary<string, UIBase>();
    // 2. 面板栈（记录打开顺序，支持返回上一级）
    private Stack<UIBase> _viewStack = new Stack<UIBase>();
    // 3. UI根节点（Canvas）
    private Transform _uiRoot;

    protected override void Awake()
    {
        base.Awake();
        _uiRoot = GameObject.Find("Canvas")?.transform;
        if (_uiRoot == null)
        {
            Debug.LogError("场景中未找到Canvas！请先创建Canvas");
            return;
        }

        // 初始化层级根节点（可选：给不同层级创建单独父节点，管理更清晰）
        InitLayerRoots();
    }

    /// <summary>
    /// 初始化层级根节点（Canvas下创建不同层级的父物体，方便管理）
    /// 例：Canvas -> BackgroundRoot -> 背景面板
    ///     Canvas -> NormalRoot -> 普通面板
    /// </summary>
    private void InitLayerRoots()
    {
        foreach (ViewLayer layer in Enum.GetValues(typeof(ViewLayer)))
        {
            string rootName = $"{layer}Root";
            if (_uiRoot.Find(rootName) == null)
            {
                GameObject rootObj = new GameObject(rootName);
                rootObj.transform.SetParent(_uiRoot);
                rootObj.transform.localScale = Vector3.one;
                // 层级根节点的SiblingIndex与层级对应（确保上层根节点在上面）
                rootObj.transform.SetSiblingIndex((int)layer);
            }
        }
    }

    /// <summary>
    /// 打开View面板（核心跳转方法）
    /// </summary>
    /// <typeparam name="T">View类型（继承UIBase）</typeparam>
    /// <param name="viewName">面板预制体名称（Resources/UIPanels/下）</param>
    /// <param name="openMode">打开模式</param>
    /// <param name="initAction">面板初始化后执行的回调（如传参）</param>
    /// <param name="onComplete">打开完成回调</param>
    public void OpenView<T>(string viewName, ViewOpenMode openMode = ViewOpenMode.Overlay, 
        Action<T> initAction = null, Action onComplete = null) where T : UIBase
    {
        // 1. 防重复创建：如果面板已存在，根据模式处理
        if (_viewDic.ContainsKey(viewName))
        {
            UIBase existingView = _viewDic[viewName];
            // 单例层级面板：直接置顶显示
            if (LayerConfig.IsSingleInstanceLayer(existingView.ViewLayer))
            {
                existingView.RefreshLayer(); // 刷新层级置顶
                existingView.Show(onComplete);
                return;
            }

            // 非单例层级：根据打开模式处理
            switch (openMode)
            {
                case ViewOpenMode.SingleTop:
                    existingView.RefreshLayer();
                    existingView.Show(onComplete);
                    return;
                case ViewOpenMode.Overlay:
                case ViewOpenMode.ReplaceCurrent:
                case ViewOpenMode.ClearStack:
                    // 已存在但模式需要重新打开，先销毁旧面板
                    existingView.DestroySelf();
                    break;
            }
        }

        // 2. 加载面板预制体
        GameObject viewPrefab = Resources.Load<GameObject>($"UIPanels/{viewName}");
        if (viewPrefab == null)
        {
            Debug.LogError($"未找到面板预制体：Resources/UIPanels/{viewName}");
            return;
        }

        // 3. 获取面板对应的层级根节点（如Normal面板放到NormalRoot下）
        Transform layerRoot = GetLayerRoot(viewPrefab.GetComponent<UIBase>().ViewLayer);
        // 4. 实例化面板
        GameObject viewObj = Instantiate(viewPrefab, layerRoot);
        viewObj.name = viewName; // 去掉克隆后缀
        T viewInstance = viewObj.GetComponent<T>();
        if (viewInstance == null)
        {
            Debug.LogError($"面板{viewName}未挂载{typeof(T)}脚本！");
            Destroy(viewObj);
            return;
        }

        // 5. 初始化面板
        viewInstance.Init();
        // 执行初始化回调（如传参）
        initAction?.Invoke(viewInstance);
        // 添加到字典和栈
        _viewDic.Add(viewName, viewInstance);

        // 6. 根据打开模式处理面板栈
        HandleViewStack(openMode, viewInstance);

        // 7. 显示面板
        viewInstance.Show(onComplete);
    }

    /// <summary>
    /// 获取层级对应的根节点
    /// </summary>
    private Transform GetLayerRoot(ViewLayer layer)
    {
        string rootName = $"{layer}Root";
        Transform root = _uiRoot.Find(rootName);
        return root ?? _uiRoot; // 找不到则直接挂在Canvas下
    }

    /// <summary>
    /// 处理面板栈（根据打开模式管理栈结构）
    /// </summary>
    private void HandleViewStack(ViewOpenMode openMode, UIBase newView)
    {
        switch (openMode)
        {
            case ViewOpenMode.Overlay:
                // 叠加模式：压入栈，当前面板保持显示
                _viewStack.Push(newView);
                break;
            case ViewOpenMode.ReplaceCurrent:
                // 替换模式：弹出栈顶面板并关闭，新面板压入栈
                if (_viewStack.Count > 0)
                {
                    UIBase topView = _viewStack.Pop();
                    topView.Hide(isDestroy: true); // 关闭并销毁旧面板
                }
                _viewStack.Push(newView);
                break;
            case ViewOpenMode.ClearStack:
                // 清空栈模式：关闭所有面板，新面板作为栈底
                while (_viewStack.Count > 0)
                {
                    UIBase view = _viewStack.Pop();
                    view.Hide(isDestroy: true);
                }
                _viewStack.Push(newView);
                break;
            case ViewOpenMode.SingleTop:
                // 单例置顶：如果已在栈中，移到栈顶；否则压入栈
                if (_viewStack.Contains(newView))
                {
                    Stack<UIBase> tempStack = new Stack<UIBase>();
                    while (_viewStack.Count > 0)
                    {
                        UIBase view = _viewStack.Pop();
                        if (view != newView)
                        {
                            tempStack.Push(view);
                        }
                    }
                    while (tempStack.Count > 0)
                    {
                        _viewStack.Push(tempStack.Pop());
                    }
                }
                _viewStack.Push(newView);
                break;
        }
    }

    /// <summary>
    /// 关闭指定面板
    /// </summary>
    /// <param name="viewName">面板名称</param>
    /// <param name="isDestroy">是否销毁（默认true）</param>
    public void CloseView(string viewName, bool isDestroy = true)
    {
        if (!_viewDic.ContainsKey(viewName))
        {
            Debug.LogWarning($"面板{viewName}未加载，无法关闭！");
            return;
        }

        UIBase view = _viewDic[viewName];
        // 从栈中移除
        if (_viewStack.Contains(view))
        {
            Stack<UIBase> tempStack = new Stack<UIBase>();
            while (_viewStack.Count > 0)
            {
                UIBase tempView = _viewStack.Pop();
                if (tempView != view)
                {
                    tempStack.Push(tempView);
                }
            }
            while (tempStack.Count > 0)
            {
                _viewStack.Push(tempStack.Pop());
            }
        }

        // 关闭面板
        view.Hide(() =>
        {
            if (isDestroy)
            {
                view.DestroySelf();
            }
        }, isDestroy);
    }

    /// <summary>
    /// 返回上一级面板
    /// </summary>
    public void GoBack()
    {
        if (_viewStack.Count < 2)
        {
            Debug.LogWarning("已经是最顶层面板，无法返回！");
            return;
        }

        // 1. 关闭当前栈顶面板
        UIBase currentView = _viewStack.Pop();
        currentView.Hide(isDestroy: true);

        // 2. 显示新的栈顶面板（上一级）
        UIBase prevView = _viewStack.Peek();
        prevView.Show();
        prevView.RefreshLayer(); // 置顶上一级面板
    }

    /// <summary>
    /// 清空所有面板
    /// </summary>
    public void ClearAllViews()
    {
        foreach (var kvp in _viewDic)
        {
            kvp.Value.Hide(isDestroy: true);
        }
        _viewDic.Clear();
        _viewStack.Clear();
        Debug.Log("所有面板已清空！");
    }

    /// <summary>
    /// 获取已加载的面板
    /// </summary>
    public T GetView<T>(string viewName) where T : UIBase
    {
        if (_viewDic.ContainsKey(viewName))
        {
            return _viewDic[viewName] as T;
        }
        Debug.LogWarning($"面板{viewName}未加载！");
        return null;
    }

    /// <summary>
    /// 获取同层级已加载的面板数量（用于同层级置顶）
    /// </summary>
    public int GetSameLayerViewCount(ViewLayer layer)
    {
        int count = 0;
        foreach (var kvp in _viewDic)
        {
            if (kvp.Value.ViewLayer == layer)
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// 从字典中移除面板（面板销毁时调用）
    /// </summary>
    public void RemoveViewFromList(string viewName)
    {
        if (_viewDic.ContainsKey(viewName))
        {
            _viewDic.Remove(viewName);
        }
    }
}