using UnityEngine;

public class TechBehaviourSurge : TechBase
{
    private const string BUFF_ID = "SURGE";


    public override void OnTechAdded()
    {
        EventManager.EnemyStatusEffectAppliedEvent += OnEnemyStatusEffectApplied;
    }

    public override void OnTechRemoved()
    {
        EventManager.EnemyStatusEffectAppliedEvent -= OnEnemyStatusEffectApplied;
    }

    private void OnEnemyStatusEffectApplied(GameEnums.DamageElement elem, EnemyEntity e)
    {
        if (elem != GameEnums.DamageElement.Thunder) { return; }

        PlayerEntity.ActiveInstance.ChangeBuff(BUFF_ID, Group.Level, 1);
    }

    public override void OnTechUpgraded()
    {

    }
}
