using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHelper:SingletonBase<DebugHelper>
{
    public void Log(string msg)
    {
        #region PC端 
        Debug.Log(msg);
        #endregion

        #region 安卓端
        Debug.Log(msg);
        #endregion
        
        #region 苹果端
        Debug.Log(msg);
        #endregion
    }

    public void LogWarning(string msg)
    {
        #region PC端
        Debug.LogWarning(msg);
        #endregion
    }

    public void LogError(string msg)
    {
        Debug.LogError(msg);
    }
}
