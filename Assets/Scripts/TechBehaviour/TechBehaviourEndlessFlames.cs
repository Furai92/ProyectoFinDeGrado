using UnityEngine;

public class TechBehaviourEndlessFlames : TechBase
{
    private const float SEARCH_RADIUS = 20f;
    private const float CHANCE = 10f;

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
        if (Random.Range(0, 101) < CHANCE * Group.Level)
        {
            TechCombatEffect.TechCombatEffectSetupData sd = new TechCombatEffect.TechCombatEffectSetupData()
            {
                element = GameEnums.DamageElement.Fire,
                enemyIgnored = e.EnemyInstanceID,
                magnitude = 0,
                sizeMult = SEARCH_RADIUS
            };
            ObjectPoolManager.GetTechCombatEffectFromPool("STATUS_TRANSFER").SetUp(e.transform.position, 0, sd);
        }
    }

    public override void OnTechUpgraded()
    {

    }
}
