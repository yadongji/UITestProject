using UnityEngine;
using System;
using UnityEngine.UI;

/// <summary>
/// View基类（所有UI面板的父类，封装核心能力）
/// </summary>
public class UIBase : MonoBehaviour
{
    [Header("View基础配置")]
    [Tooltip("面板层级（决定显示优先级）")]
    [SerializeField] protected ViewLayer _viewLayer = ViewLayer.Normal;
    [Tooltip("是否显示遮罩（弹窗层建议开启）")]
    [SerializeField] protected bool _needMask = false;
    [Tooltip("遮罩透明度")]
    [SerializeField] protected float _maskAlpha = 0.5f;
    [Tooltip("显示/隐藏是否需要动画过渡")]
    [SerializeField] protected bool _needAnimation = true;
    [Tooltip("动画过渡时间（秒）")]
    [SerializeField] protected float _animationDuration = 0.3f;

    // 面板状态（避免重复操作）
    public enum ViewState
    {
        Uninitialized, // 未初始化
        Initializing,  // 初始化中
        Ready,         // 就绪（已加载未显示）
        Showing,       // 显示中
        Shown,         // 已显示
        Hiding,        // 隐藏中
        Hidden         // 已隐藏
    }

    // 公共属性
    public string ViewName => gameObject.name; // 面板名称（与预制体名一致）
    public ViewLayer ViewLayer => _viewLayer;
    public ViewState CurrentState { get; private set; } = ViewState.Uninitialized;
    protected GameObject MaskObj { get; private set; } // 遮罩对象

    /// <summary>
    /// 初始化（仅调用一次，UIManager加载时触发）
    /// </summary>
    public virtual void Init()
    {
        if (CurrentState != ViewState.Uninitialized) return;

        CurrentState = ViewState.Initializing;
        
        // 1. 初始化层级（设置Canvas下的渲染顺序）
        SetLayerSiblingIndex();
        
        // 2. 初始化遮罩（如需）
        if (_needMask)
        {
            CreateMask();
        }
        
        // 3. 绑定基础事件（子类可重写扩展）
        BindBaseEvents();
        
        // 4. 初始状态设为隐藏
        gameObject.SetActive(false);
        CurrentState = ViewState.Ready;
        
        DebugHelper.Instance.Log($"[{ViewName}] 初始化完成，层级：{_viewLayer}");
    }

    /// <summary>
    /// 设置面板层级对应的渲染顺序（确保上层覆盖下层）
    /// </summary>
    private void SetLayerSiblingIndex()
    {
        int baseIndex = LayerConfig.GetLayerSiblingIndex(_viewLayer);
        // 同层级面板叠加时，新面板置顶（+1确保在同层级最上面）
        transform.SetSiblingIndex(baseIndex + UIManager.Instance.GetSameLayerViewCount(_viewLayer));
    }

    /// <summary>
    /// 创建遮罩（父节点为Canvas，层级在面板之下）
    /// </summary>
    private void CreateMask()
    {
        if (MaskObj != null) return;

        // 1. 创建遮罩对象（父节点为Canvas）
        MaskObj = new GameObject($"{ViewName}_Mask");
        MaskObj.transform.SetParent(transform.parent); // 与面板同层级（Canvas下）
        MaskObj.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1); // 遮罩在面板之下

        // 2. 添加遮罩组件
        Image maskImage = MaskObj.AddComponent<Image>();
        maskImage.color = new Color(0, 0, 0, _maskAlpha);
        maskImage.raycastTarget = true; // 拦截点击（下层面板不可操作）

        // 3. 设置遮罩大小（适配Canvas）
        RectTransform maskRect = MaskObj.GetComponent<RectTransform>();
        maskRect.anchorMin = Vector2.zero;
        maskRect.anchorMax = Vector2.one;
        maskRect.offsetMin = Vector2.zero;
        maskRect.offsetMax = Vector2.zero;

        // 4. 初始隐藏遮罩
        MaskObj.SetActive(false);
    }

    /// <summary>
    /// 绑定基础事件（如遮罩点击事件）
    /// </summary>
    protected virtual void BindBaseEvents()
    {
        if (MaskObj != null)
        {
            // 遮罩点击默认关闭面板（子类可重写修改行为）
            MaskObj.AddComponent<Button>().onClick.AddListener(() =>
            {
                UIManager.Instance.CloseView(ViewName);
            });
        }
    }

    /// <summary>
    /// 显示面板（支持动画过渡）
    /// </summary>
    /// <param name="onComplete">显示完成回调</param>
    public virtual void Show(Action onComplete = null)
    {
        if (CurrentState is ViewState.Showing or ViewState.Shown) return;

        CurrentState = ViewState.Showing;
        gameObject.SetActive(true);
        MaskObj?.SetActive(true);

        // 显示动画（淡入+缩放，子类可重写）
        if (_needAnimation)
        {
            transform.localScale = Vector3.zero;
            LeanTween.scale(gameObject, Vector3.one, _animationDuration)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnComplete(() =>
                {
                    CurrentState = ViewState.Shown;
                    onComplete?.Invoke();
                });
        }
        else
        {
            transform.localScale = Vector3.one;
            CurrentState = ViewState.Shown;
            onComplete?.Invoke();
        }

        Debug.Log($"[{ViewName}] 显示面板");
    }

    /// <summary>
    /// 隐藏面板（支持动画过渡）
    /// </summary>
    /// <param name="onComplete">隐藏完成回调</param>
    /// <param name="isDestroy">是否同时销毁面板</param>
    public virtual void Hide(Action onComplete = null, bool isDestroy = false)
    {
        if (CurrentState is ViewState.Hiding or ViewState.Hidden) return;

        CurrentState = ViewState.Hiding;
        MaskObj?.SetActive(false);

        // 隐藏动画（淡出+缩放，子类可重写）
        if (_needAnimation)
        {
            LeanTween.scale(gameObject, Vector3.zero, _animationDuration)
                .setEase(LeanTweenType.easeInQuad)
                .setOnComplete(() =>
                {
                    gameObject.SetActive(false);
                    CurrentState = ViewState.Hidden;
                    if (isDestroy)
                    {
                        DestroySelf();
                    }
                    onComplete?.Invoke();
                });
        }
        else
        {
            gameObject.SetActive(false);
            CurrentState = ViewState.Hidden;
            if (isDestroy)
            {
                DestroySelf();
            }
            onComplete?.Invoke();
        }

        Debug.Log($"[{ViewName}] 隐藏面板，是否销毁：{isDestroy}");
    }

    /// <summary>
    /// 销毁面板（移除事件+清理资源）
    /// </summary>
    public virtual void DestroySelf()
    {
        // 移除所有事件监听（避免内存泄漏）
        UnbindAllEvents();
        
        // 销毁遮罩
        if (MaskObj != null)
        {
            Destroy(MaskObj);
        }
        
        // 从UIManager注册列表中移除
        UIManager.Instance.RemoveViewFromList(ViewName);
        
        // 销毁面板对象
        Destroy(gameObject);
        Debug.Log($"[{ViewName}] 销毁面板");
    }

    /// <summary>
    /// 解绑所有事件（子类必须重写，移除自身注册的事件）
    /// </summary>
    protected virtual void UnbindAllEvents()
    {
        // 示例：移除事件中心的监听（子类需补充自身的事件）
        // EventCenter.Instance.RemoveEvent(EventDefine.UpdateStudentList, OnStudentListUpdate);
    }

    /// <summary>
    /// 强制更新层级（如切换面板时置顶）
    /// </summary>
    public virtual void RefreshLayer()
    {
        SetLayerSiblingIndex();
        Debug.Log($"[{ViewName}] 刷新层级完成");
    }

    /// <summary>
    /// 面板被销毁时的清理（双重保障）
    /// </summary>
    protected virtual void OnDestroy()
    {
        UnbindAllEvents();
        LeanTween.cancel(gameObject); // 取消所有未完成的动画
    }
}