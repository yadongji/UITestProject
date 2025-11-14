using System;
using UnityEngine;

/// <summary>
/// 普通面板基类（Normal层级，如主面板、列表面板）
/// 特性：可叠加显示，无遮罩（默认），支持返回上一级
/// </summary>
public class UINormalBase : UIBase
{
    public override void Init()
    {
        base.Init();
        // 普通面板默认配置（可被子类覆盖）
        _viewLayer = ViewLayer.Normal;
        _needMask = false;
        _needAnimation = true;
    }

    // 普通面板特有方法（如刷新数据）
    public virtual void RefreshData(object data)
    {
        Debug.Log($"[{ViewName}] 刷新普通面板数据");
    }
}

/// <summary>
/// 弹窗面板基类（Popup层级，如确认弹窗、编辑弹窗）
/// 特性：单例显示（同一时间只有一个），带遮罩，拦截下层点击
/// </summary>
public class UIPopupBase : UIBase
{
    public override void Init()
    {
        base.Init();
        // 弹窗面板默认配置（可被子类覆盖）
        _viewLayer = ViewLayer.Popup;
        _needMask = true;
        _maskAlpha = 0.6f;
        _needAnimation = true;
    }

    // 弹窗面板特有方法（如设置弹窗内容）
    public virtual void SetPopupContent(string title, string content)
    {
        Debug.Log($"[{ViewName}] 弹窗标题：{title}，内容：{content}");
    }
}

/// <summary>
/// 提示面板基类（Toast层级，如操作成功、错误提示）
/// 特性：单例显示，自动消失，无遮罩，不拦截点击
/// </summary>
public class UIToastBase : UIBase
{
    [SerializeField] protected float _autoCloseDelay = 2f; // 自动关闭延迟

    public override void Init()
    {
        base.Init();
        // 提示面板默认配置
        _viewLayer = ViewLayer.Toast;
        _needMask = false;
        _needAnimation = true;
    }

    public override void Show(Action onComplete = null)
    {
        base.Show(() =>
        {
            // 显示后自动关闭
            Invoke(nameof(AutoClose), _autoCloseDelay);
            onComplete?.Invoke();
        });
    }

    // 自动关闭
    protected virtual void AutoClose()
    {
        Hide(isDestroy: true); // 提示面板关闭后直接销毁
    }

    // 提示面板特有方法（设置提示内容）
    public virtual void SetToastContent(string content, bool isSuccess = true)
    {
        Debug.Log($"[{ViewName}] 提示内容：{content}，是否成功：{isSuccess}");
    }
}