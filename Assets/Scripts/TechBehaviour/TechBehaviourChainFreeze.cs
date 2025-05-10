using UnityEngine;

public class TechBehaviourChainFreeze : TechBase
{
    private const float EXPLOSION_RADIUS = 8f;
    private const float DAMAGE = 500f;

    public override void OnTechAdded()
    {
        EventManager.EnemyStatusEffectAppliedEvent += OnEnemyStatusEffectApplied;
    }

    public override void OnTechRemoved()
    {
        EventManager.EnemyStatusEffectAppliedEvent -= OnEnemyStatusEffectApplied;
    }

    private void OnEnemyStatusEffectApplied(GameEnums.DamageElement elem, EnemyEntity target)
    {
        if (elem != GameEnums.DamageElement.Frost) { return; }

        TechCombatEffect.TechCombatEffectSetupData sd = new TechCombatEffect.TechCombatEffectSetupData()
        {
            element = elem,
            sizeMult = EXPLOSION_RADIUS,
            enemyIgnored = target.EnemyInstanceID,
            magnitude = DAMAGE * Group.Level

        };
        ObjectPoolManager.GetTechCombatEffectFromPool("EXPLOSION").SetUp(target.transform.position, 0, sd);
    }

    public override void OnTechUpgraded()
    {

    }
}
