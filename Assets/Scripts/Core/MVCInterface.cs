using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 视图接口
/// </summary>
public interface IView
{
    /// <summary>
    /// 初始化视图
    /// </summary>
    void Initialize();

    /// <summary>
    /// 显示视图
    /// </summary>
    //void Show();

    /// <summary>
    /// 隐藏视图
    /// </summary>
    //void Hide();

    /// <summary>
    /// 注册事件监听
    /// </summary>
    void RegisterEvents();

    /// <summary>
    /// 移除事件监听
    /// </summary>
    void UnregisterEvents();
}

/// <summary>
/// 控制器接口
/// </summary>
public interface IController
{
    /// <summary>
    /// 初始化控制器
    /// </summary>
    void Initialize();

    /// <summary>
    /// 注册控制器事件
    /// </summary>
    void RegisterControllerEvents();

    /// <summary>
    /// 处理视图事件
    /// </summary>
    void HandleViewEvent(string eventName, object data);

    /// <summary>
    /// 销毁控制器
    /// </summary>
    void Dispose();
}

/// <summary>
/// 模型接口
/// </summary>
public interface IModel
{
    /// <summary>
    /// 初始化模型(直线运动模型)
    /// </summary>
    void Initialize(int modelID, Direction moveDirection, Vector3 initialPosition, bool isParticle, List<MoveStage> moveStages);

    /// <summary>
    /// 清理模型数据
    /// </summary>
    void Clear();
}