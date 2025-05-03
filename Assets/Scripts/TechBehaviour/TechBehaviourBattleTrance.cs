using UnityEngine;

public class TechBehaviourBattleTrance : TechBase
{
    private const string BUFF_ID = "BATTLE_TRANCE";


    public override void OnTechAdded()
    {
        EventManager.EnemyDefeatedEvent += OnEnemyDefeated;
        EventManager.StageStateEndedEvent += OnStageStateEnded;
    }

    public override void OnTechRemoved()
    {
        EventManager.EnemyDefeatedEvent -= OnEnemyDefeated;
        EventManager.StageStateEndedEvent -= OnStageStateEnded;
    }

    private void OnEnemyDefeated(EnemyEntity e)
    {
        PlayerEntity.ActiveInstance.ChangeBuff(BUFF_ID, Group.Level, 1);
    }
    private void OnStageStateEnded(StageStateBase.GameState s) 
    {
        PlayerEntity.ActiveInstance.RemoveBuff(BUFF_ID);
    }

    public override void OnTechUpgraded()
    {

    }
}
