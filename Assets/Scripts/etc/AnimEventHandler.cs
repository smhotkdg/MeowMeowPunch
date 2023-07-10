using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventHandler : MonoBehaviour
{
    public enum EventType
    {
        KeyEnd
    }
    public EventType eventType = EventType.KeyEnd;

    public void Event()
    {
        switch(eventType)
        {
            case EventType.KeyEnd:
                GameManager.Instance.EndKeyEvent();
                break;
        }
    }
}
