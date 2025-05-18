using UnityEngine;

public class TechBehaviourProtectionCore : TechBase
{
    private const float SHIELD_PERCENT_RECHARGE = 0.4f;

    public override void OnTechAdded()
    {
        EventManager.StageStateStartedEvent += OnStageStateStarted;
    }

    public override void OnTechRemoved()
    {
        EventManager.StageStateStartedEvent -= OnStageStateStarted;
    }

    private void OnStageStateStarted(StageStateBase.GameState s) 
    {
        if (s == StageStateBase.GameState.BossFight || s == StageStateBase.GameState.EnemyWave) 
        {
            PlayerEntity.ActiveInstance.AddShield(PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.MaxHealth) * SHIELD_PERCENT_RECHARGE);
        }
    }

    public override void OnTechUpgraded()
    {

    }
}
