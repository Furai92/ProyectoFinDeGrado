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
    public static event Action<WeaponSO.WeaponSlot> PlayerWeaponOverheatedEvent;
    public static event Action<EnemyEntity, float, GameEnums.EnemyRank, bool> EnemyDisabledEvent;
    public static event Action<EnemyEntity> EnemySpawnedEvent;
    public static event Action PlayerDefeatedEvent;
    public static event Action<float> PlayerDamageTakenEvent;
    public static event Action<float> PlayerDamageAbsorbedEvent;
    public static event Action<Vector3> PlayerHealthOrbGatheredEvent;
    public static event Action<float> PlayerHealthRestoredEvent;
    public static event Action<float> PlayerHealingNegatedEvent;
    public static event Action<Vector3, float> PlayerDashStartedEvent;
    public static event Action<Vector3, float> PlayerDashEndedEvent;
    public static event Action<Vector3> PlayerWallSlamEvent;
    public static event Action<Vector3, WeaponSO.WeaponSlot, float> PlayerAttackStartedEvent;
    public static event Action PlayerFixedTimeIntervalEvent;
    public static event Action<PlayerAttackBase, bool> PlayerProjectileBounceEvent;
    public static event Action PlayerShieldCappedEvent;
    public static event Action<float> PlayerOverhealedEvent;
    public static event Action<float> PlayerHeatNegatvedEvent;
    public static event Action<Vector3> PlayerDeflectEvent;

    // Stage Status
    public static event Action<int> CurrencyUpdateEvent;
    public static event Action<Interactable> InteractableDisabledEvent;
    public static event Action<StageStateBase.GameState> StageStateStartedEvent;
    public static event Action<StageStateBase.GameState> StageStateEndedEvent;
    public static event Action AllPlayersReadyEvent;
    public static event Action<int> ChestDisabledEvent;
    public static event Action ChestOpenedEvent;

    // UI
    public static event Action<HudEnemyHealthBarElement> UiEnemyHealthBarDisabledEvent;
    public static event Action<bool> UiFocusChangedEvent;
    public static event Action<IGameMenu> UiMenuClosedEvent;
    public static event Action<IGameMenu> UiMenuFocusChangedEvent;

    // Game System 
    public static event Action GameSettingsChangedEvent;

    public static void OnChestOpened() 
    {
        ChestOpenedEvent?.Invoke();
    }
    public static void OnEnemySpawned(EnemyEntity e) 
    {
        EnemySpawnedEvent?.Invoke(e);
    }
    public static void OnPlayerDeflect(Vector3 wpos) 
    {
        PlayerDeflectEvent?.Invoke(wpos);
    }
    public static void OnPlayerOverhealed(float h) 
    {
        PlayerOverhealedEvent?.Invoke(h);
    }
    public static void OnPlayerShieldCapped() 
    {
        PlayerShieldCappedEvent?.Invoke();
    }
    public static void OnPlayerHeatNegated(float h)
    {
        PlayerHeatNegatvedEvent?.Invoke(h);
    }
    public static void OnPlayerHealingNegated(float h) 
    {
        PlayerHealingNegatedEvent?.Invoke(h);
    }
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
    public static void OnEnemyDisabled(EnemyEntity e, float overkill, GameEnums.EnemyRank rank, bool killcredit)
    {
        EnemyDisabledEvent?.Invoke(e, overkill, rank, killcredit);
    }
    public static void OnCurrencyUpdated(int change) 
    {
        CurrencyUpdateEvent?.Invoke(change);
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
    public static void OnPlayerWeaponOverheated(WeaponSO.WeaponSlot s) 
    {
        PlayerWeaponOverheatedEvent?.Invoke(s);
    }
    public static void OnPlayerDamageTaken(float dmg)
    {
        PlayerDamageTakenEvent?.Invoke(dmg);
    }
    public static void OnPlayerDamageAbsorbed(float dmg)
    {
        PlayerDamageAbsorbedEvent?.Invoke(dmg);
    }
    public static void OnPlayerHealthOrbGathered(Vector3 pos) 
    {
        PlayerHealthOrbGatheredEvent?.Invoke(pos);
    }
    public static void OnPlayerHealthRestored(float amount) 
    {
        PlayerHealthRestoredEvent?.Invoke(amount);
    }
    public static void OnChestDisabled(int id) 
    {
        ChestDisabledEvent?.Invoke(id);
    }
    public static void OnPlayerDashStarted(Vector3 pos, float dir)
    {
        PlayerDashStartedEvent?.Invoke(pos, dir);
    }
    public static void OnPlayerDashEnded(Vector3 pos, float dir)
    {
        PlayerDashEndedEvent?.Invoke(pos, dir);
    }
    public static void OnPlayerWallSlam(Vector3 pos) 
    {
        PlayerWallSlamEvent?.Invoke(pos);
    }
    public static void OnPlayerAttackStarted(Vector3 pos, WeaponSO.WeaponSlot slot, float direction)
    {
        PlayerAttackStartedEvent?.Invoke(pos, slot, direction);
    }
    public static void OnPlayerFixedTimeInterval() 
    {
        PlayerFixedTimeIntervalEvent?.Invoke();
    }
    public static void OnProjectileBounce(PlayerAttackBase attack, bool entityBounce) 
    {
        PlayerProjectileBounceEvent?.Invoke(attack, entityBounce);
    }
}
