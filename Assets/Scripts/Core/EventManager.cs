using UnityEngine;
using System;

public class EventManager
{
    public static event Action<bool> UiFocusChangedEvent;

    public static void OnUiFocusChanged(bool uifocused)
    {
        UiFocusChangedEvent?.Invoke(uifocused);
    }

}
