using UnityEngine;

public class TechBehaviourIncineration : TechBase
{
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
        if (elem != GameEnums.DamageElement.Fire) { return; }
        if (e.GetStatusDuration(GameEnums.DamageElement.Fire) <= EnemyEntity.STATUS_DURATION_STANDARD) { return; }

        e.AddStatus(GameEnums.DamageElement.Incineration);
    }

    public override void OnTechUpgraded()
    {

    }
}
