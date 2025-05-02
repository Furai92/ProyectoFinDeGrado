using UnityEngine;

public class TechBehaviourDisorder : TechBase
{
    private const float INT_TO_DAMAGE_RATIO = 10f;

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
        float baseDamage = PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.Intellect) * INT_TO_DAMAGE_RATIO * Group.Level;
        enem.DealDirectDamage(baseDamage * statuses, 0, 1, 0, GameEnums.DamageElement.NonElemental, GameEnums.DamageType.Tech);
    }

    public override void OnTechUpgraded()
    {

    }
}
