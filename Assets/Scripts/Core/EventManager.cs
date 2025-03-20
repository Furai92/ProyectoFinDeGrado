using UnityEngine;
using System;

public class EventManager
{
    public static event Action<bool> UiFocusChangedEvent;
    public static event Action CurrencyUpdateEvent;
    public static event Action CombatWaveStartedEvent;
    public static event Action CombatWaveEndedEvent;
    public static event Action<Vector3, float, int, GameEnums.DamageElement> EnemyDirectDamageTakenEvent;
    public static event Action<EnemyEntity> EnemyDefeatedEvent;
    public static event Action<Interactable> InteractableDisabledEvent;

    public static void OnUiFocusChanged(bool uifocused)
    {
        UiFocusChangedEvent?.Invoke(uifocused);
    }
    public static void OnEnemyDirectDamageTaken(Vector3 wpos, float magnitude, int critlevel, GameEnums.DamageElement element) 
    {
        EnemyDirectDamageTakenEvent?.Invoke(wpos, magnitude, critlevel, element);
    }
    public static void OnEnemyDefeated(EnemyEntity e) 
    {
        EnemyDefeatedEvent?.Invoke(e);
    }
    public static void OnCurrencyUpdated() 
    {
        CurrencyUpdateEvent?.Invoke();
    }
    public static void OnInteractableDisabled(Interactable i) 
    {
        InteractableDisabledEvent?.Invoke(i);
    }
    public static void OnCombatWaveStarted() 
    {
        CombatWaveStartedEvent?.Invoke();
    }
    public static void OnCombatWaveEnded()
    {
        CombatWaveEndedEvent?.Invoke();
    }
}
