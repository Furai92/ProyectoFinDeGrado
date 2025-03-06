using UnityEngine;
using System;

public class EventManager
{
    public static event Action<bool> UiFocusChangedEvent;
    public static event Action<Vector3, float, int, GameEnums.DamageElement> EnemyDirectDamageTakenEvent;
    public static event Action CurrencyUpdateEvent;

    public static void OnUiFocusChanged(bool uifocused)
    {
        UiFocusChangedEvent?.Invoke(uifocused);
    }
    public static void OnEnemyDirectDamageTaken(Vector3 wpos, float magnitude, int critlevel, GameEnums.DamageElement element) 
    {
        EnemyDirectDamageTakenEvent?.Invoke(wpos, magnitude, critlevel, element);
    }
    public static void OnCurrencyUpdated() 
    {
        CurrencyUpdateEvent?.Invoke();
    }

}
