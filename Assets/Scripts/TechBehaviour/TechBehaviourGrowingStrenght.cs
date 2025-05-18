using UnityEngine;

public class TechBehaviourGrowingStrenght : TechBase
{
    private bool effectActive;

    private const string BUFF_ID = "GROWING_STRENGHT";


    public override void OnTechAdded()
    {
        EventManager.PlayerHealthOrbGatheredEvent += OnHealthOrbGathered;
        EventManager.StageStateEndedEvent += OnStageStateEnded;
        effectActive = StageManagerBase.GetCurrentStateType() != StageStateBase.GameState.Rest;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerHealthOrbGatheredEvent -= OnHealthOrbGathered;
        EventManager.StageStateEndedEvent -= OnStageStateEnded;
    }

    private void OnHealthOrbGathered(Vector3 wpos)
    {
        if (!effectActive) { return; }

        PlayerEntity.ActiveInstance.ChangeBuff(BUFF_ID, 1, Group.Level);
    }
    private void OnStageStateEnded(StageStateBase.GameState s) 
    {
        effectActive = StageManagerBase.GetCurrentStateType() != StageStateBase.GameState.Rest;
        PlayerEntity.ActiveInstance.RemoveBuff(BUFF_ID);
    }

    public override void OnTechUpgraded()
    {

    }
}
