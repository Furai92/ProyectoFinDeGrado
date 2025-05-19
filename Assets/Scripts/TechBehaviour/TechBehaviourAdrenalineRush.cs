using UnityEngine;

public class TechBehaviourAdrenalineRush : TechBase
{
    private const string BUFF_ID = "ADRENALINE_RUSH";
    private const int STACKS_REQ = 10;

    public override void OnTechAdded()
    {
        EventManager.PlayerHealthOrbGatheredEvent += OnHealthOrbGathered;
        EventManager.StageStateEndedEvent += OnStageStateEnded;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerHealthOrbGatheredEvent -= OnHealthOrbGathered;
        EventManager.StageStateEndedEvent -= OnStageStateEnded;
    }
    private void OnHealthOrbGathered(Vector3 wpos) 
    {
        PlayerEntity.ActiveInstance.ChangeBuff(BUFF_ID, 1, Group.Level);
        if (PlayerEntity.ActiveInstance.GetBuffStacks(BUFF_ID) > STACKS_REQ) 
        {
            PlayerEntity.ActiveInstance.RemoveBuff(BUFF_ID);
            TechCombatEffect.TechCombatEffectSetupData sd = new TechCombatEffect.TechCombatEffectSetupData();
            ObjectPoolManager.GetTechCombatEffectFromPool("BULLETSTORM").SetUp(PlayerEntity.ActiveInstance.transform.position, 0, sd);
        }
    }
    private void OnStageStateEnded(StageStateBase.GameState s) 
    {
        PlayerEntity.ActiveInstance.RemoveBuff(BUFF_ID);
    }

    public override void OnTechUpgraded()
    {

    }
}
