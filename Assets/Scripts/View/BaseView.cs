using UnityEngine;

/// <summary>
/// ª˘¥° ”Õº¿‡
/// </summary>
public abstract class BaseView : MonoBehaviour, IView
{
    protected bool isInitialized = false;

    public virtual void Initialize()
    {
        if (!isInitialized)
        {
            RegisterEvents();
            isInitialized = true;
        }
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    public abstract void RegisterEvents();

    public abstract void UnregisterEvents();

    protected virtual void OnDestroy()
    {
        UnregisterEvents();
    }
}
