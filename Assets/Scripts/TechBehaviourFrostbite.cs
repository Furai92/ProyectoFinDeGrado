using UnityEngine;

public class TechBehaviourFrostbite : TechBase
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
        if (elem == GameEnums.DamageElement.Frost) { e.AddStatus(GameEnums.DamageElement.Frostbite); }
    }

    public override void OnTechUpgraded()
    {

    }
}
