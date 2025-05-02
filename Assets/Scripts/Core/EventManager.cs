using UnityEngine;
using System;

public class EventManager
{
    // Combat
    public static event Action<float, int, GameEnums.DamageElement, GameEnums.DamageType, EnemyEntity> EnemyDirectDamageTakenEvent;
    public static event Action<float, GameEnums.DamageElement, EnemyEntity> EnemyStatusDamageTakenEvent;
    public static event Action<GameEnums.DamageElement, EnemyEntity> EnemyStatusEffectAppliedEvent;
    public static event Action<HudCombatWarningElement.WarningType, Vector3> CombatWarningDisplayedEvent;
    public static event Action<PlayerEntity> PlayerStatsUpdatedEvent;
    public static event Action<PlayerEntity> PlayerSpawnedEvent;
    public static event Action StageStatsUpdatedEvent;
    public static event Action<Vector3> PlayerEvasionEvent;
    public static event Action PlayerWeaponOverheatedEvent;
    public static event Action<EnemyEntity> EnemyDefeatedEvent;
    public static event Action<EnemyEntity> EnemyDisabledEvent;
    public static event Action PlayerDefeatedEvent;
    public static event Action<float> PlayerDamageTakenEvent;
    public static event Action<float> PlayerDamageAbsorbedEvent;

    // Stage Status
    public static event Action CurrencyUpdateEvent;
    public static event Action<Interactable> InteractableDisabledEvent;
    public static event Action<StageStateBase.GameState> StageStateStartedEvent;
    public static event Action<StageStateBase.GameState> StageStateEndedEvent;
    public static event Action AllPlayersReadyEvent;

    // UI
    public static event Action<HudEnemyHealthBarElement> UiEnemyHealthBarDisabledEvent;
    public static event Action<bool> UiFocusChangedEvent;
    public static event Action<IGameMenu> UiMenuFocusChangedEvent;
    public static event Action<IGameMenu> UiMenuClosedEvent;

    // Game System 
    public static event Action GameSettingsChangedEvent;

    public static void OnUiMenuFocusChanged(IGameMenu m) 
    {
        UiMenuFocusChangedEvent?.Invoke(m);
    }
    public static void OnUiFocusChanged(bool uifocused)
    {
        UiFocusChangedEvent?.Invoke(uifocused);
    }
    public static void OnEnemyStatusApplied(GameEnums.DamageElement elem, EnemyEntity target) 
    {
        EnemyStatusEffectAppliedEvent?.Invoke(elem, target);
    }
    public static void OnEnemyStatusDamageTaken(float magnitude, GameEnums.DamageElement element, EnemyEntity target)
    {
        EnemyStatusDamageTakenEvent?.Invoke(magnitude, element, target);
    }
    public static void OnEnemyDirectDamageTaken(float magnitude, int critlevel, GameEnums.DamageElement element, GameEnums.DamageType dtype, EnemyEntity target) 
    {
        EnemyDirectDamageTakenEvent?.Invoke(magnitude, critlevel, element, dtype, target);
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
    public static void OnStageStateStarted(StageStateBase.GameState s) 
    {
        StageStateStartedEvent?.Invoke(s);
    }
    public static void OnStageStateEnded(StageStateBase.GameState s)
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
    public static void OnCombatWarningDisplayed(HudCombatWarningElement.WarningType wt, Vector3 pos) 
    {
        CombatWarningDisplayedEvent?.Invoke(wt, pos);
    }
    public static void OnPlayerStatsUpdated(PlayerEntity p) 
    {
        PlayerStatsUpdatedEvent?.Invoke(p);
    }
    public static void OnPlayerEvasion(Vector3 wpos) 
    {
        PlayerEvasionEvent?.Invoke(wpos);
    }
    public static void OnStageStatsUpdated() 
    {
        StageStatsUpdatedEvent?.Invoke();
    }
    public static void OnGameSettingsChanged() 
    {
        GameSettingsChangedEvent?.Invoke();
    }
    public static void OnUiMenuClosed(IGameMenu m) 
    {
        UiMenuClosedEvent?.Invoke(m);
    }
    public static void OnPlayerSpawned(PlayerEntity p) 
    {
        PlayerSpawnedEvent?.Invoke(p);
    }
    public static void OnPlayerDefeated()
    {
        PlayerDefeatedEvent?.Invoke();
    }
    public static void OnPlayerWeaponOverheated() 
    {
        PlayerWeaponOverheatedEvent?.Invoke();
    }
    public static void OnPlayerDamageTaken(float dmg)
    {
        PlayerDamageTakenEvent?.Invoke(dmg);
    }
    public static void OnPlayerDamageAbsorbed(float dmg)
    {
        PlayerDamageAbsorbedEvent?.Invoke(dmg);
    }
}
