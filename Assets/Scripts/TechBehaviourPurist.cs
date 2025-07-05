using UnityEngine;

public class TechBehaviourPurist : TechBase
{
    private const string BUFF_ID = "PURIST";
    private GameEnums.DamageElement previousElement = GameEnums.DamageElement.NonElemental;


    public override void OnTechAdded()
    {
        previousElement = GameEnums.DamageElement.NonElemental;
        EventManager.EnemyStatusEffectAppliedEvent += OnEnemyStatusEffectApplied;
        EventManager.StageStateEndedEvent += OnStageStateEnded;
    }

    public override void OnTechRemoved()
    {
        EventManager.EnemyStatusEffectAppliedEvent -= OnEnemyStatusEffectApplied;
        EventManager.StageStateEndedEvent -= OnStageStateEnded;
    }

    private void OnEnemyStatusEffectApplied(GameEnums.DamageElement elem, EnemyEntity e)
    {
        if (elem != previousElement) { PlayerEntity.ActiveInstance.RemoveBuff(BUFF_ID); }

        previousElement = elem;
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
