using UnityEngine;

public class TechBehaviourDisorder : TechBase
{
    private const float BASE_DAMAGE = 1000f;

    public override void OnTechAdded()
    {
        EventManager.EnemyStatusEffectAppliedEvent += OnEnemyStatusEffectApplied;
    }

    public override void OnTechRemoved()
    {
        EventManager.EnemyStatusEffectAppliedEvent -= OnEnemyStatusEffectApplied;
    }

    private void OnEnemyStatusEffectApplied(GameEnums.DamageElement elem, EnemyEntity enem)
    {
        int statuses = enem.GetStatusEffectCount();
        if (statuses < 2) { return; }
        enem.DealDirectDamage(BASE_DAMAGE * Group.Level * statuses, 0, 1, 0, GameEnums.DamageElement.NonElemental, GameEnums.DamageType.Tech);
    }

    public override void OnTechUpgraded()
    {

    }
}
