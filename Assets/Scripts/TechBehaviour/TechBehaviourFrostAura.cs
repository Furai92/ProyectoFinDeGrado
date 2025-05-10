using UnityEngine;

public class TechBehaviourFrostAura : TechBase
{

    private int tickCount;
    private bool perkDisabled;

    private const float RADIUS = 5f;
    private const int TICKS_TO_TRIGGER = 3;
    private const float DAMAGE = 200f;

    public override void OnTechAdded()
    {
        EventManager.PlayerFixedTimeIntervalEvent += OnFixedTimeInterval;
        EventManager.StageStateStartedEvent += OnStageStateStarted;
        EventManager.StageStateEndedEvent += OnStageStateEnded;

        tickCount = 0;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerFixedTimeIntervalEvent -= OnFixedTimeInterval;
        EventManager.StageStateStartedEvent -= OnStageStateStarted;
        EventManager.StageStateEndedEvent -= OnStageStateEnded;
    }

    private void OnStageStateEnded(StageStateBase.GameState s) 
    {
        PlayerEntity.ActiveInstance.RemoveBuff("COOLHEADED");
    }
    private void OnStageStateStarted(StageStateBase.GameState s) 
    {
        PlayerEntity.ActiveInstance.RemoveBuff("COOLHEADED");
        perkDisabled = s == StageStateBase.GameState.Rest;
    }
    private void OnFixedTimeInterval() 
    {
        if (perkDisabled) { return; }

        tickCount++;
        if (tickCount < TICKS_TO_TRIGGER) { return; }
        if (PlayerEntity.ActiveInstance.StatusHeatMelee > 0 || PlayerEntity.ActiveInstance.StatusHeatRanged > 0) { return; }

        tickCount = 0;
        TechCombatEffect.TechCombatEffectSetupData sd = new TechCombatEffect.TechCombatEffectSetupData()
        {
            sizeMult = RADIUS,
            element = GameEnums.DamageElement.Frost,
            enemyIgnored = -1,
            magnitude = DAMAGE * Group.Level
        };

        ObjectPoolManager.GetTechCombatEffectFromPool("EXPLOSION").SetUp(PlayerEntity.ActiveInstance.transform.position, 0, sd);
    }

    public override void OnTechUpgraded()
    {

    }
}
