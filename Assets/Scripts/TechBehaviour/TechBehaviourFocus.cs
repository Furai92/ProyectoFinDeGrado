using UnityEngine;

public class TechBehaviourFocus : TechBase
{
    private const float CHANCE = 25f;
    private const string BUFF_ID = "FOCUS";


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
        if (Random.Range(1, 101) > CHANCE * Group.Level) { return; }

        PlayerEntity.ActiveInstance.ChangeBuff(BUFF_ID, 1, 1);
    }
    private void OnStageStateEnded(StageStateBase.GameState s) 
    {
        PlayerEntity.ActiveInstance.RemoveBuff(BUFF_ID);
    }

    public override void OnTechUpgraded()
    {

    }
}
