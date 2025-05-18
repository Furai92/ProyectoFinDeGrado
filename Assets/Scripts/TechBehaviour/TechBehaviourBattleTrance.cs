using UnityEngine;

public class TechBehaviourBattleTrance : TechBase
{
    private const string BUFF_ID = "BATTLE_TRANCE";


    public override void OnTechAdded()
    {
        EventManager.EnemyDisabledEvent += OnEnemyDisabled;
        EventManager.StageStateEndedEvent += OnStageStateEnded;
    }

    public override void OnTechRemoved()
    {
        EventManager.EnemyDisabledEvent -= OnEnemyDisabled;
        EventManager.StageStateEndedEvent -= OnStageStateEnded;
    }

    private void OnEnemyDisabled(EnemyEntity e, float overkill, bool killcredit)
    {
        if (!killcredit) { return; }
        PlayerEntity.ActiveInstance.ChangeBuff(BUFF_ID, 1, Group.Level);
    }
    private void OnStageStateEnded(StageStateBase.GameState s) 
    {
        PlayerEntity.ActiveInstance.RemoveBuff(BUFF_ID);
    }

    public override void OnTechUpgraded()
    {

    }
}
