using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineMoveController : MonoBehaviour, IController
{

    public void Dispose()
    {
        
    }

    public void HandleViewEvent(string eventName, object data)
    {
        EventCenter.Instance.BroadcastMessage(eventName, data);
    }

    public void Initialize()
    {
        
    }

    public void RegisterControllerEvents()
    {
        //EventCenter.Instance.AddEventListener();
    }
}
