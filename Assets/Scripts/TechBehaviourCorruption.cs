using UnityEngine;

public class TechBehaviourCorruption : TechBase
{
    private const string BUFF_ID = "CORRUPTION";

    private bool techActive;

    public override void OnTechAdded()
    {
        EventManager.PlayerHealthRestoredEvent += OnPlayerHealingRecieved;
        EventManager.StageStateEndedEvent += OnStageStateEnded;
        EventManager.StageStateStartedEvent += OnStageStateStarted;
        techActive = StageManagerBase.GetCurrentStateType() != StageStateBase.GameState.Rest;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerHealthRestoredEvent -= OnPlayerHealingRecieved;
        EventManager.StageStateEndedEvent -= OnStageStateEnded;
        EventManager.StageStateStartedEvent -= OnStageStateStarted;
    }

    private void OnPlayerHealingRecieved(float hp)
    {
        if (!techActive) { return; }

        PlayerEntity.ActiveInstance.ChangeBuff(BUFF_ID, Group.Level, 1);
    }
    private void OnStageStateStarted(StageStateBase.GameState s) 
    {
        techActive = StageManagerBase.GetCurrentStateType() != StageStateBase.GameState.Rest;
    }
    private void OnStageStateEnded(StageStateBase.GameState s) 
    {
        PlayerEntity.ActiveInstance.RemoveBuff(BUFF_ID);
    }

    public override void OnTechUpgraded()
    {

    }
}
