using UnityEngine;

public class TechBehaviourVolatility : TechBase
{
    private const float DAMAGE = 300f;
    private const float RADIUS = 8f;

    public override void OnTechAdded()
    {
        EventManager.EnemyDisabledEvent += OnEnemyDisabled;
    }

    public override void OnTechRemoved()
    {
        EventManager.EnemyDisabledEvent -= OnEnemyDisabled;
    }

    private void OnEnemyDisabled(EnemyEntity e, float overkill, bool killcredit)
    {
        if (!killcredit) { return; }
        if (e.GetStatusDuration(GameEnums.DamageElement.Fire) <= 0) { return; }

        TechCombatEffect.TechCombatEffectSetupData sd = new TechCombatEffect.TechCombatEffectSetupData()
        {
            element = GameEnums.DamageElement.Fire,
            sizeMult = RADIUS,
            enemyIgnored = -1,
            magnitude = DAMAGE * Group.Level
        };
        ObjectPoolManager.GetTechCombatEffectFromPool("EXPLOSION").SetUp(e.transform.position, 0, sd);
    }

    public override void OnTechUpgraded()
    {

    }
}
