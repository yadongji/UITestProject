using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 基础控制器类
/// </summary>
public abstract class BaseController : IController
{
    protected bool isInitialized = false;

    public BaseController()
    {
        
    }

    public virtual void Initialize(int modelID, Direction moveDirection, Vector3 initialPosition, bool isParticle, List<MoveStage> moveStages)
    {
        if (!isInitialized)
        {
            RegisterControllerEvents();
            RegisterView();
            isInitialized = true;
        }
    }

    public abstract void RegisterControllerEvents();
    public abstract void RegisterView();

    public abstract void HandleViewEvent(string eventName, object data);

    public virtual void Dispose()
    {
        // 清理资源
    }

    public void Initialize()
    {
        throw new System.NotImplementedException();
    }
}