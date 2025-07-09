using UnityEngine;

public class TechBehaviourVoidShock : TechBase
{
    private const float DAMAGE = 200f;

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
        if (elem != GameEnums.DamageElement.Void) { return; }
        e.DealDirectDamage(DAMAGE * Group.Level, 0, 1, 0, GameEnums.DamageElement.NonElemental, GameEnums.DamageType.Tech);
    }

    public override void OnTechUpgraded()
    {

    }
}
