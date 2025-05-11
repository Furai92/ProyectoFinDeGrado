using UnityEngine;

public class TechBehaviourCoolheaded : TechBase
{

    private bool tickDisabled;
    private bool effectEnabled;

    public override void OnTechAdded()
    {
        effectEnabled = StageManagerBase.GetCurrentStateType() != StageStateBase.GameState.Rest;

        EventManager.PlayerFixedTimeIntervalEvent += OnFixedTimeInterval;
        EventManager.PlayerAttackStartedEvent += OnPlayerAttack;
        EventManager.StageStateStartedEvent += OnStageStateStarted;
        EventManager.StageStateEndedEvent += OnStageStateEnded;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerFixedTimeIntervalEvent -= OnFixedTimeInterval;
        EventManager.PlayerAttackStartedEvent -= OnPlayerAttack;
        EventManager.StageStateStartedEvent -= OnStageStateStarted;
        EventManager.StageStateEndedEvent -= OnStageStateEnded;
    }

    private void OnPlayerAttack(Vector3 pos, WeaponSO.WeaponSlot s, float direction) 
    {
        tickDisabled = true;
    }
    private void OnStageStateEnded(StageStateBase.GameState s) 
    {
        PlayerEntity.ActiveInstance.RemoveBuff("COOLHEADED");
    }
    private void OnStageStateStarted(StageStateBase.GameState s) 
    {
        PlayerEntity.ActiveInstance.RemoveBuff("COOLHEADED");
        effectEnabled = s != StageStateBase.GameState.Rest;
    }
    private void OnFixedTimeInterval() 
    {
        if (!effectEnabled) { return; }
        if (tickDisabled) { tickDisabled = false; return; }

        PlayerEntity.ActiveInstance.ChangeBuff("COOLHEADED", Group.Level, 1);
    }

    public override void OnTechUpgraded()
    {

    }
}
