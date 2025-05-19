using UnityEngine;

public class TechBehaviourSecondWind : TechBase
{
    private int tickCount;
    private bool perkDisabled;

    private const float HEALTH_RESTORED = 1f;
    private const int TICKS_TO_TRIGGER = 5;
    private const float HEALTH_PERCENT_THS = 0.4f;

    public override void OnTechAdded()
    {
        EventManager.PlayerFixedTimeIntervalEvent += OnFixedTimeInterval;
        EventManager.StageStateStartedEvent += OnStageStateStarted;

        tickCount = 0;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerFixedTimeIntervalEvent -= OnFixedTimeInterval;
        EventManager.StageStateStartedEvent -= OnStageStateStarted;
    }

    private void OnStageStateStarted(StageStateBase.GameState s) 
    {
        perkDisabled = s == StageStateBase.GameState.Rest;
        tickCount = 0;
    }
    private void OnFixedTimeInterval() 
    {
        if (perkDisabled) { return; }

        tickCount++;
        if (tickCount < TICKS_TO_TRIGGER) { return; }
        if (PlayerEntity.ActiveInstance.StatusHeatMelee > 0 || PlayerEntity.ActiveInstance.StatusHeatRanged > 0) { return; }
        if (PlayerEntity.ActiveInstance.GetHealthPercent() >= HEALTH_PERCENT_THS) { return; }

        tickCount = 0;
        PlayerEntity.ActiveInstance.Heal(HEALTH_RESTORED * Group.Level);
    }

    public override void OnTechUpgraded()
    {

    }
}
