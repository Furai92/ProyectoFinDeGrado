using UnityEngine;

public class TechBehaviourChaoticEnergy : TechBase
{
    private const float CHANCE_TO_TRIGGER = 25f;
    private const float DMG_TO_EXPLOSION_RATIO = 0.5f;
    private const float EXPLOSION_RADIUS = 8f;

    public override void OnTechAdded()
    {
        EventManager.EnemyDirectDamageTakenEvent += OnEnemyDamageTaken;
    }

    public override void OnTechRemoved()
    {
        EventManager.EnemyDirectDamageTakenEvent -= OnEnemyDamageTaken;
    }

    private void OnEnemyDamageTaken(float mag, int clevel, GameEnums.DamageElement elem, GameEnums.DamageType dtype, EnemyEntity target)
    {
        if (clevel < 1) { return; }
        if (Random.Range(0, 101) > CHANCE_TO_TRIGGER) { return; }

        TechCombatEffect.TechCombatEffectSetupData sd = new TechCombatEffect.TechCombatEffectSetupData()
        {
            element = elem,
            sizeMult = EXPLOSION_RADIUS,
            enemyIgnored = -1,
            magnitude = mag * DMG_TO_EXPLOSION_RATIO * Group.Level

        };
        ObjectPoolManager.GetTechCombatEffectFromPool("EXPLOSION").SetUp(target.transform.position, 0, sd);
    }

    public override void OnTechUpgraded()
    {

    }
}
