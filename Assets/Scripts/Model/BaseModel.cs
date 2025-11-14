using System.Collections.Generic;
using UnityEngine;

public abstract class BaseModel : IModel
{
    protected bool isInitialized = false;

    public virtual void Initialize()
    {
        if (!isInitialized)
        {
            isInitialized = true;
        }
    }

    public virtual void Clear()
    {
        // 清理模型数据
    }

    public void Initialize(int modelID, Direction moveDirection, Vector3 initialPosition, bool isParticle, List<MoveStage> moveStages)
    {
        
    }
}