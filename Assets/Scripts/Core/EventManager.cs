using UnityEngine;
using System;

public class EventManager
{
    // Combat
    public static event Action<float, int, GameEnums.DamageElement, EnemyEntity> EnemyDirectDamageTakenEvent;
    public static event Action<GameEnums.DamageElement, EnemyEntity> EnemyStatusEffectAppliedEvent;
    public static event Action<EnemyEntity> EnemyDefeatedEvent;
    public static event Action<EnemyEntity> EnemyDisabledEvent;

    // Stage Status
    public static event Action CurrencyUpdateEvent;
    public static event Action<Interactable> InteractableDisabledEvent;
    public static event Action<StageStateBase> StageStateStartedEvent;
    public static event Action<StageStateBase> StageStateEndedEvent;
    public static event Action AllPlayersReadyEvent;

    // UI
    public static event Action<HudEnemyHealthBarElement> UiEnemyHealthBarDisabledEvent;
    public static event Action<bool> UiFocusChangedEvent;

    public static void OnUiFocusChanged(bool uifocused)
    {
        UiFocusChangedEvent?.Invoke(uifocused);
    }
    public static void OnEnemyStatusApplied(GameEnums.DamageElement elem, EnemyEntity target) 
    {
        EnemyStatusEffectAppliedEvent?.Invoke(elem, target);
    }
    public static void OnEnemyDirectDamageTaken(float magnitude, int critlevel, GameEnums.DamageElement element, EnemyEntity target) 
    {
        EnemyDirectDamageTakenEvent?.Invoke(magnitude, critlevel, element, target);
    }
    public static void OnEnemyDefeated(EnemyEntity e) 
    {
        EnemyDefeatedEvent?.Invoke(e);
    }
    public static void OnEnemyDisabled(EnemyEntity e)
    {
        EnemyDisabledEvent?.Invoke(e);
    }
    public static void OnCurrencyUpdated() 
    {
        CurrencyUpdateEvent?.Invoke();
    }
    public static void OnInteractableDisabled(Interactable i) 
    {
        InteractableDisabledEvent?.Invoke(i);
    }
    public static void OnStageStateStarted(StageStateBase s) 
    {
        StageStateStartedEvent?.Invoke(s);
    }
    public static void OnStageStateEnded(StageStateBase s)
    {
        StageStateEndedEvent?.Invoke(s);
    }
    public static void OnAllPlayersReady() 
    {
        AllPlayersReadyEvent?.Invoke();
    }
    public static void OnUiEnemyHealthBarDisabled(HudEnemyHealthBarElement hpb) 
    {
        UiEnemyHealthBarDisabledEvent.Invoke(hpb);
    }
}
