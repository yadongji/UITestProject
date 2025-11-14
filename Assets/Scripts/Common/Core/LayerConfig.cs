/// <summary>
/// View层级枚举（数字越大，显示层级越高）
/// </summary>
public enum ViewLayer
{
    Background = 0,    // 背景层（如游戏背景、底图，唯一）
    Normal = 10,       // 普通面板层（如学生管理主面板、列表面板，可多个叠加）
    Popup = 20,        // 弹窗层（如确认删除、编辑学生弹窗，唯一，带遮罩）
    Toast = 30,        // 提示层（如操作成功提示、错误提示，唯一，无遮罩）
    Top = 99           // 顶层（如加载中、强制更新，永远在最上面）
}

/// <summary>
/// 层级配置类（统一管理各层级的基础设置）
/// </summary>
public static class LayerConfig
{
    /// <summary>
    /// 获取层级对应的默认SiblingIndex（控制Canvas下的渲染顺序）
    /// </summary>
    public static int GetLayerSiblingIndex(ViewLayer layer)
    {
        return (int)layer * 10; // 每个层级预留10个位置，方便同层级内排序
    }

    /// <summary>
    /// 是否为单例层级（同一层级只能有一个面板显示）
    /// </summary>
    public static bool IsSingleInstanceLayer(ViewLayer layer)
    {
        return layer is ViewLayer.Background or ViewLayer.Popup or ViewLayer.Toast or ViewLayer.Top;
    }
}