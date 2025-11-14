using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainView : IView
{
    public void RegisterEvents()
    {
        EventCenter.Instance.AddEventListener(CommonEvent.testID, RefreshInfo);
    }

    private void RefreshInfo(object obj)
    {
        
    }

    public void UnregisterEvents()
    {
        
    }

    public void Initialize()
    {
        throw new NotImplementedException();
    }
}
